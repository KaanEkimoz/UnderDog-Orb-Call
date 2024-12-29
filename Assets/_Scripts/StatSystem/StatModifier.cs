using com.absence.variablesystem.mutations.internals;
using UnityEngine;

namespace com.game.statsystem
{
    [System.Serializable]
    public class StatModifier
    {
        [SerializeField] private Mutation<float> m_mutationObject;

        public float Value
        {
            get
            {
                return m_mutationObject.Value;
            }

            set
            {
                m_mutationObject.Value = value;
            }
        }

        public StatModifier(Mutation<float> mutationObject)
        {
            m_mutationObject = mutationObject;
        }
    }
}
