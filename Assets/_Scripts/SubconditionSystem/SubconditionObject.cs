using System;

namespace com.game.subconditionsystem
{
    public class SubconditionObject : IDisposable
    {
        SubconditionProfileBase m_profile;
        Func<object[], bool> m_conditionFormula;

        object[] m_args;

        public object[] Arguments { get { return m_args; } set { m_args = value; } }

        public event Action<SubconditionObject> OnDispose;

        public SubconditionObject(SubconditionProfileBase profile)
        {
            m_profile = profile;

            profile.OnInstantiation(this);

            RegenerateConditionFormula(m_args);
            SubscribeToEvents();
        }

        protected virtual void SubscribeToEvents()
        {
            m_profile.OnRuntimeEventSubscription(this);
        }

        public virtual void Update(params object[] args)
        {
            m_profile.OnUpdate(Game.Event, this);
        }

        public virtual bool GetResult(params object[] args)
        {
            return m_conditionFormula.Invoke(args);
        }

        public virtual string GenerateDescription(bool richText = false)
        {
            return m_profile.GenerateDescription(richText, this);
        }

        public void RegenerateConditionFormula(params object[] args)
        {
            m_conditionFormula = m_profile.GenerateConditionFormula(args);
        }

        public virtual void Dispose()
        {
            OnDispose?.Invoke(this);
        }
    }
}
