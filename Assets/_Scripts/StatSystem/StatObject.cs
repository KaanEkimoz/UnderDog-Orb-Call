using com.absence.variablesystem.builtin;
using UnityEngine;

namespace com.game.statsystem
{
    [System.Serializable]
    public class StatObject
    {
        [SerializeField] private Float m_variableObject;
        public float Value
        {
            get
            {
                return m_variableObject.Value;
            }
        }

        public StatObject(Float variableObject)
        {
            m_variableObject = variableObject;
        }
    }
}
