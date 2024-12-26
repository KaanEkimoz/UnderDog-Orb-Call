using UnityEngine;

namespace com.game.statsystem
{
    [System.Serializable]
    public class PlayerStatModification
    {
        public PlayerStatType TargetStatType;
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
    }
}
