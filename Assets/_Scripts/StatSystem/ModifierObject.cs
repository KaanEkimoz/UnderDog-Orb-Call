using com.absence.variablesystem.mutations.internals;
using UnityEngine;

namespace com.game.statsystem
{
    /// <summary>
    /// A reference to a stat modifier.
    /// </summary>
    [System.Serializable]
    public class ModifierObject
    {
        [SerializeField] private Mutation<float> m_mutationObject;

        /// <summary>
        /// Current value of the modifier this reference points to.
        /// </summary>
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

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to create a modifier object with a <see cref="Mutation{T}"/>.
        /// </summary>
        /// <param name="mutationObject">The mutation object of this stat.</param>
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
