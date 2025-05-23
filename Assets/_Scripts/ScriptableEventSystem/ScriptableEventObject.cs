using System;

namespace com.game.scriptableeventsystem
{
    public class ScriptableEventObject : IDisposable
    {
        ScriptableEventProfileBase m_profile;

        Action<object[]> m_action;

        object[] m_args;

        public bool Bypass { get; set; }
        public object[] Arguments { get { return m_args; } set { m_args = value; } }

        public event Action<ScriptableEventObject> OnDispose;

        public ScriptableEventObject(ScriptableEventProfileBase profile)
        {
            Bypass = false;
            m_profile = profile;

            profile.OnInstantiation(this);

            RegenerateEventAction(m_args);
            SubscribeToEvents();
        }

        protected virtual void SubscribeToEvents()
        {
            m_profile.OnRuntimeEventSubscription(this);
        }

        public void RegenerateEventAction(params object[] args)
        {
            m_action = m_profile.GenerateAction(args);
        }

        public virtual void Update(params object[] args)
        {
            m_profile.OnUpdate(Game.Event, this);
        }

        public void Invoke()
        {
            Invoke(m_args);
        }

        public virtual void Invoke(params object[] args)
        {
            if (Bypass)
                return;

            m_action?.Invoke(m_args);
        }

        public virtual string GenerateDescription(bool richText = false)
        {
            return m_profile.GenerateDescription(richText, this);
        }

        public virtual void Dispose()
        {
            OnDispose?.Invoke(this);
        }
    }
}
