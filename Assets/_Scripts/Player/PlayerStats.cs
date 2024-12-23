using com.absence.variablesystem.builtin;
using com.absence.variablesystem.mutations.internals;
using com.game.player.scriptables;
using com.game.statsystem;
using UnityEngine;
using com.absence.variablesystem.internals;

namespace com.game.player
{
    /// <summary>
    /// The PlayerComponent responsible for managing anything related to player stats.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private bool m_debugMode = false;
        [SerializeField] private PlayerCharacterProfile m_defaultCharacterProfile;

        [SerializeField] private Float m_exampleStat1;
        [SerializeField] private Float m_exampleStat2;

        private void Awake()
        {
            if (m_debugMode) Initialize(m_defaultCharacterProfile);
        }

        /// <summary>
        /// Use to initialize this component with a profile. This operation will clear any
        /// mutations made to any stats and re-create every variable object with the
        /// default value received from the profile provided. 
        /// It won't set your mutation object references to null (if you have any), tho.
        /// </summary>
        /// <param name="profile">The profile provided.</param>
        public void Initialize(PlayerCharacterProfile profile)
        {
            m_exampleStat1 = new("Example1", profile.DefaultStat1);
            m_exampleStat2 = new("Example2", profile.DefaultStat2);

            Debug.Log("PlayerStats successfully initialized!");
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
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            FloatAdditionMutation mutationObject = new(amount, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return mutationObject;
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
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            float realPercentage = (percentage / 100f);
            FloatMultiplicationMutation mutationObject = new(realPercentage, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return mutationObject;
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
            switch (targetStat)
            {
                case PlayerStatType.Example1:
                    return m_exampleStat1;
                case PlayerStatType.Example2:
                    return m_exampleStat2;
                default:
                    Debug.LogError("An error occurred determining a stat's desired variable." +
                        "It's entry may be forgotten.");
                    return null;
            }
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
            desiredStatVariable = GetDesiredStatVariable(targetStat);
            if (desiredStatVariable != null) return true;
            else return false;
        }
    }
}