using System;
using UnityEngine;

namespace com.game.subconditionsystem
{
    public abstract class SubconditionProfileBase : ScriptableObject
    {
        public abstract Func<object[], bool> GenerateConditionFormula(params object[] args);
        public abstract string GenerateDescription(bool richText = false, SubconditionObject instance = null);
        public abstract void OnInstantiation(SubconditionObject instance);
        public abstract void OnRuntimeEventSubscription(SubconditionObject instance);
        public abstract void OnUpdate(GameRuntimeEvent evt, SubconditionObject instance);
    }
}
