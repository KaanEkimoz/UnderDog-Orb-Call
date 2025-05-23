using System;
using UnityEngine;

namespace com.game.scriptableeventsystem
{
    public abstract class ScriptableEventProfileBase : ScriptableObject
    {
        public abstract Action<object[]> GenerateAction(params object[] args);
        public abstract string GenerateDescription(bool richText = false, ScriptableEventObject instance = null);
        public abstract void OnInstantiation(ScriptableEventObject instance);
        public abstract void OnRuntimeEventSubscription(ScriptableEventObject instance);
        public abstract void OnUpdate(GameRuntimeEvent evt, ScriptableEventObject instance);
    }
}
