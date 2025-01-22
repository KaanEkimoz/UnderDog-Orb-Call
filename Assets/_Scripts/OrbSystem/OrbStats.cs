using com.absence.attributes;
using System.Collections.Generic;
using UnityEngine;
namespace com.game.orbsystem.statsystemextensions
{
    public class OrbStats : MonoBehaviour
    {
        [Header("Utilities")]

        [SerializeField, Tooltip("If enabled, this component with initialize itself, and also some additional console messages will take place.")]
        private bool m_debugMode = false;

        [SerializeField, Required, Tooltip("Default values provided for the any initialization process.")]
        private OrbDefaultStats m_defaultStats;

        [Header("Stats")]
        [SerializeField, Readonly] private OrbStatHolder m_statHolder;

        Dictionary<OrbStatType, float> m_defaultValues;

        public OrbStatHolder StatHolder => m_statHolder;
        public Dictionary<OrbStatType, float> DefaultValues => m_defaultValues;

        private void Awake()
        {
            Initialize(m_defaultStats);
        }

        #region Public API

        /// <summary>
        /// Use to initialize this component with default values for each stat.
        /// </summary>
        /// <param name="defaultValues">The default values provided.</param>
        public void Initialize(OrbDefaultStats defaultValues)
        {
            m_statHolder = new OrbStatHolder(defaultValues);
            Debug.Log("OrbStats initialized.");
        }

        #endregion
    }
}
