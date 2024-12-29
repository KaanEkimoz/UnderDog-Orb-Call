using System;
using UnityEngine;

namespace com.game.statsystem.presetobjects
{
    [System.Serializable]
    public abstract class StatModification
    {
        public StatModificationType ModificationType;

        [SerializeField] private float m_incrementalValue;
        [SerializeField] private float m_percentageValue;

        public float Value
        {
            get
            {
                return ModificationType switch
                {
                    StatModificationType.Incremental => m_incrementalValue,
                    StatModificationType.Percentage => m_percentageValue,
                    _ => -1f,
                };
            }
        }

        public abstract Type GetEnumType();
    }

    [System.Serializable]
    public abstract class StatModification<T> : StatModification where T : System.Enum
    {
        public T TargetStatType;

        public override Type GetEnumType()
        {
            return typeof(T);
        }
    }
}
