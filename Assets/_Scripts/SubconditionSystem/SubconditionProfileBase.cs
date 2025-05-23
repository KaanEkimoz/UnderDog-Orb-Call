using System;
using UnityEngine;

namespace com.game.subconditionsystem
{
    public abstract class SubconditionProfileBase : ScriptableObject
    {
        [SerializeField] private bool m_invert = false;

        [HideInInspector] public bool IsSubAsset = false;

        public bool Invert => m_invert;

        public abstract Func<object[], bool> GenerateConditionFormula(SubconditionObject instance);
        public abstract string GenerateDescription(bool richText = false, SubconditionObject instance = null);
        public abstract void OnInstantiation(SubconditionObject instance);
        public abstract void OnRuntimeEventSubscription(SubconditionObject instance);
        public abstract void OnUpdate(GameRuntimeEvent evt, SubconditionObject instance);
    }
}
