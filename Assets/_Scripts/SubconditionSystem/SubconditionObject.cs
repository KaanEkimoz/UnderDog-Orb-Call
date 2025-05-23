using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.GPUSort;

namespace com.game.subconditionsystem
{
    public class SubconditionObject : IDisposable
    {
        SubconditionProfileBase m_profile;
        Func<object[], bool> m_conditionFormula;

        object[] m_args;

        public List<SubconditionObject> Children;

        public object[] Arguments { get { return m_args; } set { m_args = value; } }

        public event Action<SubconditionObject> OnDispose;

        public SubconditionObject(SubconditionProfileBase profile)
        {
            Children = new();
            m_profile = profile;

            profile.OnInstantiation(this);

            DoRegenerateConditionFormula();
            SubscribeToEvents();
        }

        protected virtual void SubscribeToEvents()
        {
            m_profile.OnRuntimeEventSubscription(this);
        }

        public virtual void Update(params object[] args)
        {
            Children?.ForEach(child => child.Update(args));
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

        public void RegenerateConditionFormula()
        {
            Children?.ForEach(child => child.RegenerateConditionFormula());
            DoRegenerateConditionFormula();
        }

        void DoRegenerateConditionFormula()
        {
            m_conditionFormula = m_profile.GenerateConditionFormula(this);
        }

        public virtual void Dispose()
        {
            Children?.ForEach(child => child.Dispose());
            Children = null;
            OnDispose?.Invoke(this);
        }
    }
}
