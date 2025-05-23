using System;
using UnityEngine;

namespace com.game.subconditionsystem
{
    public abstract class SubconditionProfileBase : ScriptableObject
    {
        [SerializeField] private bool m_invert = false;
        [SerializeField] private bool m_bypassStateChangeBallbacks = false;

        [HideInInspector] public bool IsSubAsset = false;

        public bool Invert => m_invert;
        public bool BypassStateChangeCallbacks => m_bypassStateChangeBallbacks;

        public abstract Func<object[], bool> GenerateConditionFormula(SubconditionObject instance);
        public abstract string GenerateDescription(bool richText = false, SubconditionObject instance = null);

        public virtual void OnInstantiation(SubconditionObject instance)
        {
        }

        public virtual void OnRuntimeEventSubscription(SubconditionObject instance)
        {
        }

        public virtual void OnUpdate(GameRuntimeEvent evt, SubconditionObject instance)
        {
        }
    }
}
