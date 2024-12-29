using com.absence.variablesystem.builtin;
using UnityEngine;

namespace com.game.statsystem
{
    /// <summary>
    /// A reference to a stat.
    /// </summary>
    [System.Serializable]
    public class StatObject
    {
        [SerializeField] private Float m_variableObject;

        /// <summary>
        /// Current value of the stat this reference points to.
        /// </summary>
        public float Value
        {
            get
            {
                return m_variableObject.Value;
            }
        }

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to create a stat object with a <see cref="Float"/>.
        /// </summary>
        /// <param name="variableObject">The variable object of this stat.</param>
        public StatObject(Float variableObject)
        {
            m_variableObject = variableObject;
        }

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to get the varialbe object of this stat object.
        /// </summary>
        /// <returns>Returns the variable object.</returns>
        public Float GetVariableObject()
        {
            return m_variableObject;
        }
    }
}
