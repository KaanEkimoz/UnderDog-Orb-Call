using com.absence.variablesystem.builtin;
using com.absence.variablesystem.mutations.internals;
using com.game.player.scriptables;
using com.game.statsystem;
using UnityEngine;
using com.absence.variablesystem.internals;
using System.Collections.Generic;
using com.absence.attributes;

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

        [SerializeField, ShowIf(nameof(m_debugMode)), Required, Tooltip("Profile provided for the self-initialization process.")] 
        private PlayerCharacterProfile m_defaultCharacterProfile;

        [Header("Stats")]

        [SerializeField] private Float m_health;
        [SerializeField] private Float m_armor;
        [SerializeField] private Float m_walkSpeed;
        [SerializeField] private Float m_lifeSteal;
        [SerializeField] private Float m_luck;
        [SerializeField] private Float m_gathering;
        [SerializeField] private Float m_damage;
        [SerializeField] private Float m_attackSpeed;
        [SerializeField] private Float m_criticalHits;
        [SerializeField] private Float m_range;
        [SerializeField] private Float m_knockback;
        [SerializeField] private Float m_penetration;
        [SerializeField] private Float m_crowdControl;
        [SerializeField] private Float m_lightStrength;

        Dictionary<PlayerStatType, Float> p_defaultEntries => new()
        {
            { PlayerStatType.Health, m_health },
            { PlayerStatType.Armor, m_armor },
            { PlayerStatType.WalkSpeed, m_walkSpeed },
            { PlayerStatType.LifeSteal, m_lifeSteal },
            { PlayerStatType.Luck, m_luck },
            { PlayerStatType.Gathering, m_gathering },
            { PlayerStatType.Damage, m_damage },
            { PlayerStatType.AttackSpeed, m_attackSpeed },
            { PlayerStatType.CriticalHits, m_criticalHits },
            { PlayerStatType.Range, m_range },
            { PlayerStatType.Knockback, m_knockback },
            { PlayerStatType.Penetration, m_penetration },
            { PlayerStatType.CrowdControl, m_crowdControl },
            { PlayerStatType.LightStrength, m_lightStrength },
        };

        Dictionary<PlayerStatType, Float> m_variableObjectEntries;

        private void Awake()
        {
            if (m_debugMode) Initialize(m_defaultCharacterProfile);
        }

        #region Public API

        /// <summary>
        /// Use to initialize this component with a profile. This operation will clear any
        /// mutations made to any stats and re-create every variable object with the
        /// default value received from the profile provided. 
        /// It won't set your mutation object references to null (if you have any), tho.
        /// </summary>
        /// <param name="profile">The profile provided.</param>
        public void Initialize(PlayerCharacterProfile profile)
        {
            m_health = new("Health", profile.DefaultStat2);
            m_armor = new("Armor", profile.DefaultStat2);
            m_walkSpeed = new("Walk Speed", profile.DefaultStat2);
            m_lifeSteal = new("Life Steal", profile.DefaultStat2);
            m_luck = new("Luck", profile.DefaultStat2);
            m_gathering = new("Gathering", profile.DefaultStat2);
            m_damage = new("Damage", profile.DefaultStat2);
            m_attackSpeed = new("Attack Speed", profile.DefaultStat2);
            m_criticalHits = new("Crit", profile.DefaultStat2);
            m_range = new("Range", profile.DefaultStat2);
            m_knockback = new("Knockback", profile.DefaultStat2);
            m_penetration = new("Penetration", profile.DefaultStat2);
            m_crowdControl = new("CC", profile.DefaultStat2);
            m_lightStrength = new("Light Strength", profile.DefaultStat2);

            m_variableObjectEntries = p_defaultEntries;

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

            float realPercentage = 1f + (percentage / 100f);
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
            if (!m_variableObjectEntries.TryGetValue(targetStat, out Float value))
            {
                Debug.LogError("An error occurred determining a stat's desired variable. " +
                    "It's entry may not exist.");

                return null;
            }

            return value;    
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

        #endregion
    }
}