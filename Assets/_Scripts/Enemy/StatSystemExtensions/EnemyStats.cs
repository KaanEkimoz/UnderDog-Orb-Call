using com.absence.attributes;
using com.game.enemysystem.statsystemextensions;
using com.game.generics.interfaces;
using com.game.statsystem;
using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemyStats : MonoBehaviour, IStats<EnemyStatType>, IEntityStatProvider
    {
        [Header("Utilities")]

        [SerializeField, Required, Tooltip("Default values provided for the any initialization process.")]
        private EnemyDefaultStats m_defaultStats;

        [Header("Stats")]
        [SerializeField, Readonly] private EnemyStatHolder m_statHolder;

        public IStatManipulator<EnemyStatType> Manipulator => m_statHolder;

        #region Entity Stat Properties
        public float Health => GetStat(EnemyStatType.Health);
        public float Armor => GetStat(EnemyStatType.Armor);
        public float Damage => GetStat(EnemyStatType.Damage);
        #endregion

        private void Awake()
        {
            Initialize(m_defaultStats);
            //if (m_debugMode) ApplyCharacterProfile(m_defaultCharacterProfile);
        }

        #region Public API

        /// <summary>
        /// Use to initialize this component with default values for each stat.
        /// </summary>
        /// <param name="defaultValues">The default values provided.</param>
        public void Initialize(EnemyDefaultStats defaultValues)
        {
            m_statHolder = new EnemyStatHolder(defaultValues);
            Debug.Log("EnemyStats initialized.");
        }

        public float GetStat(EnemyStatType targetStat)
        {
            return m_statHolder.GetStat(targetStat);
        }

        #endregion
    }
}
