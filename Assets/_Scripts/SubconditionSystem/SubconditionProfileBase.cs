using com.absence.attributes;
using System;
using UnityEngine;

namespace com.game.subconditionsystem
{
    public abstract class SubconditionProfileBase : ScriptableObject
    {
        [SerializeField, Header1("Subcondition"), Readonly] private string m_guid = System.Guid.NewGuid().ToString();
         
        [SerializeField] private bool m_invert = false;
        [SerializeField] private bool m_bypassStateChangeBallbacks = false;

        [HideInInspector] public bool IsSubAsset = false;

        public string Guid => m_guid;

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

        [Button("Generate Description")]
        protected void PrintFullDescription()
        {
            Debug.Log(GenerateDescription(true, null));
        }
    }
}
