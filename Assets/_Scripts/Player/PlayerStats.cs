using com.absence.variablesystem.builtin;
using com.absence.variablesystem.mutations.internals;
using com.game.player.scriptables;
using com.game.statsystem;
using UnityEngine;
using com.absence.variablesystem.internals;
using com.absence.attributes;
using com.game.statsystem.extensions;

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
        /// Use to add a custom-logic modification to a stat of Player.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="mutationObject">Mutation object created</param>
        /// <returns>Returns the mutation object passed as an argument.</returns>
        public Mutation<float> ModifyCustom(PlayerStatType targetStat, Mutation<float> mutationObject)
        {
            return StatHolder.ModifyCustom(targetStat, mutationObject);
        }

        /// <summary>
        /// Use to add an incremental modification (eg. +5, -2) to a stat of Player.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="amount">Amount of modification.</param>
        /// <returns>
        /// Returns the mutation applied to desired stat's variable object. You can
        /// remove this modification from the desired stat via the 
        /// <see cref="Demodify(PlayerStatType, Mutation{float})"/> function.
        /// </returns>
        public FloatAdditionMutation ModifyIncremental(PlayerStatType targetStat, float amount, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            return StatHolder.ModifyIncremental(targetStat, amount, affectionMethod);
        }

        /// <summary>
        /// Use to modify a stat variable with a <see cref="PlayerStatModification"/>.
        /// </summary>
        /// <param name="mod">The modification object.</param>
        /// <returns>Returns the mutation object created from this operation.</returns>
        public Mutation<float> ModifyWith(PlayerStatModification mod)
        {
            return StatHolder.ModifyWith(mod);
        }

        /// <summary>
        /// Use to cap a stat variable with a <see cref="PlayerStatCap"/>.
        /// </summary>
        /// <param name="cap">The modification object.</param>
        /// <returns>Returns the mutation object created from this operation.</returns>
        public FloatCapMutation CapWith(PlayerStatCap cap)
        {
            return StatHolder.CapWith(cap);
        }

        /// <summary>
        /// Use to override a stat variable with a <see cref="PlayerStatOverride"/>.
        /// </summary>
        /// <param name="ovr">The modification object.</param>
        /// <returns>Returns the new value of the stat variable.</returns>
        public float OverrideWith(PlayerStatOverride ovr)
        {
            return StatHolder.OverrideWith(ovr);
        }

        /// <summary>
        /// Use to add a percentage based modification (eg. +25%, -100%) to a stat of Player.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="percentage">Amount of modification. (in the following form: <i>percentage</i>%).</param>
        /// <returns>
        /// Returns the mutation applied to desired stat's variable object. You can
        /// remove this modification from the desired stat via the 
        /// <see cref="Demodify(PlayerStatType, Mutation{float})"/> function.
        /// </returns>
        public FloatMultiplicationMutation ModifyPercentage(PlayerStatType targetStat, float percentage, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            return StatHolder.ModifyPercentage(targetStat, percentage, affectionMethod);
        }

        /// <summary>
        /// Use to de-modify a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to de-modify.</param>
        /// <param name="mutationObject">The mutation object created when
        /// the modification took place.</param>
        public void Demodify(PlayerStatType targetStat, Mutation<float> mutationObject)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return;

            try
            {
                desiredStatVariable.Immutate(mutationObject);
            }

            catch
            {
                Debug.LogError("An error occurred while trying to demodify a stat. An " +
                    "Mutation object used may be invalid.");
            }
        }

        /// <summary>
        /// Use to get the corresponding variable object of a player stat.
        /// </summary>
        /// <param name="targetStat">The target enumeration value corresponds to a variable object.</param>
        /// <returns>Returns the desired variable if the enumeration entry is valid. Returns null otherwise.</returns>
        public Float GetDesiredStatVariable(PlayerStatType targetStat)
        {
            return StatHolder.GetDesiredStatVariable(targetStat);   
        }

        /// <summary>
        /// Use to check-n-get a corresponding variable object of a player stat.
        /// </summary>
        /// <param name="targetStat">The target enumeration value corresponds to a variable object.</param>
        /// <param name="desiredStatVariable">The variable object received from the getting operation. 
        /// Null if the checking operation fails.</param>
        /// <returns>Returns true if the checking operation succeeds. False otherwise.</returns>
        public bool TryGetDesiredStatVariable(PlayerStatType targetStat, out Float desiredStatVariable)
        {
            bool success = StatHolder.TryGetDesiredStatVariable(targetStat, out desiredStatVariable);
            return success;
        }

        /// <summary>
        /// Use to get the current value of a stat.
        /// </summary>
        /// <param name="targetStat">The stat to get value of.</param>
        /// <returns>Returns the value of the target stat.</returns>
        public float GetStat(PlayerStatType targetStat)
        {
            return GetDesiredStatVariable(targetStat).Value;
        }

        #endregion
    }
}