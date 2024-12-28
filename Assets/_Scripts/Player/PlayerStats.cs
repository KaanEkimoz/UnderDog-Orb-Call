using com.game.player.scriptables;
using com.game.statsystem;
using UnityEngine;
using com.absence.attributes;
using com.game.player.statsystemextensions;

namespace com.game.player
{
    /// <summary>
    /// The PlayerComponent responsible for managing anything related to player stats.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Utilities")]

        [SerializeField, Tooltip("If enabled, this component with initialize itself, and also some additional console messages will take place.")] 
        private bool m_debugMode = false;

        [SerializeField, Required, Tooltip("Default values provided for the any initialization process.")] 
        private PlayerDefaultStats m_defaultStats;

        [SerializeField, ShowIf(nameof(m_debugMode)), Required, Tooltip("Profile provided for the self-initialization process.")]
        private PlayerCharacterProfile m_defaultCharacterProfile;

        [Header("Stats")]
        [SerializeField, Readonly] private PlayerStatHolder m_statHolder;

        public PlayerStatHolder StatHolder => m_statHolder;

        private void Awake()
        {
            Initialize(m_defaultStats);
            if (m_debugMode) ApplyCharacterProfile(m_defaultCharacterProfile);
        }

        #region Public API

        /// <summary>
        /// Use to initialize this component with default values for each stat.
        /// </summary>
        /// <param name="defaultValues">The default values provided.</param>
        public void Initialize(PlayerDefaultStats defaultValues)
        {
            m_statHolder = PlayerStatHolder.Create(defaultValues);

            Debug.Log("PlayerStats successfully initialized!");
        }

        /// <summary>
        /// Use to apply a character profile to the player stats.
        /// </summary>
        /// <param name="profile">The character profile provided.</param>
        public void ApplyCharacterProfile(PlayerCharacterProfile profile)
        {
            m_statHolder.ApplyCharacterProfile(profile);
        }

        /// <summary>
        /// Use to get the current value of a stat.
        /// </summary>
        /// <param name="targetStat">The stat to get value of.</param>
        /// <returns>Returns the value of the target stat.</returns>
        public float GetStat(PlayerStatType targetStat)
        {
            return m_statHolder.GetDesiredStatVariable(targetStat).Value;
        }

        #endregion
    }
}