using com.absence.variablesystem.mutations.internals;
using UnityEngine;

namespace com.game.statsystem
{
    [System.Serializable]
    public class ModifierObject
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

        public ModifierObject(Mutation<float> mutationObject)
        {
            m_mutationObject = mutationObject;
        }

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to get the mutation object of this modifier object.
        /// </summary>
        /// <returns>Returns the mutation object.</returns>
        public Mutation<float> GetMutationObject() 
        {
            return m_mutationObject;
        }
    }
}
