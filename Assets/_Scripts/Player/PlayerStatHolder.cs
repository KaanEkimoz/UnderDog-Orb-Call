using com.absence.variablesystem.builtin;
using com.absence.variablesystem.internals;
using com.absence.variablesystem.mutations.internals;
using com.game.player.scriptables;
using com.game.statsystem;
using com.game.statsystem.extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    [System.Serializable]
    public class PlayerStatHolder
    {
        [SerializeField] private Float m_health = new();
        [SerializeField] private Float m_armor = new();
        [SerializeField] private Float m_walkSpeed = new();
        [SerializeField] private Float m_lifeSteal = new();
        [SerializeField] private Float m_luck = new();
        [SerializeField] private Float m_gathering = new();
        [SerializeField] private Float m_damage = new();
        [SerializeField] private Float m_attackSpeed = new();
        [SerializeField] private Float m_criticalHits = new();
        [SerializeField] private Float m_range = new();
        [SerializeField] private Float m_knockback = new();
        [SerializeField] private Float m_penetration = new();
        [SerializeField] private Float m_crowdControl = new();
        [SerializeField] private Float m_lightStrength = new();

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

        private PlayerStatHolder()
        {
            m_variableObjectEntries = p_defaultEntries;
        }

        public static PlayerStatHolder Create(PlayerDefaultStats defaultValues)
        {
            PlayerStatHolder holder = new();

            holder.m_health.Value = defaultValues.Health;
            holder.m_armor.Value = defaultValues.Armor;
            holder.m_lifeSteal.Value = defaultValues.LifeSteal;
            holder.m_luck.Value = defaultValues.Luck;
            holder.m_gathering.Value = defaultValues.Gathering;
            holder.m_damage.Value = defaultValues.Damage;
            holder.m_attackSpeed.Value = defaultValues.AttackSpeed;
            holder.m_criticalHits.Value = defaultValues.CriticalHits;
            holder.m_range.Value = defaultValues.Range;
            holder.m_knockback.Value = defaultValues.Knockback;
            holder.m_penetration.Value = defaultValues.Penetration;
            holder.m_crowdControl.Value = defaultValues.CrowdControl;
            holder.m_lightStrength.Value = defaultValues.LightStrength;

            return holder;
        }

        public void ForAllStats(Action<Float> action)
        {
            action?.Invoke(m_health);
            action?.Invoke(m_armor);
            action?.Invoke(m_lifeSteal);
            action?.Invoke(m_luck);
            action?.Invoke(m_gathering);
            action?.Invoke(m_damage);
            action?.Invoke(m_attackSpeed);
            action?.Invoke(m_criticalHits);
            action?.Invoke(m_range);
            action?.Invoke(m_knockback);
            action?.Invoke(m_penetration);
            action?.Invoke(m_crowdControl);
            action?.Invoke(m_lightStrength);
        }

        public void RefreshAll()
        {
            ForAllStats(stat =>
            {
                stat.Refresh();
            });
        }

        public void ClearAllMutations()
        {
            ForAllStats(stat =>
            {
                stat.ClearMutations();
            });

            RefreshAll();
        }

        public void ApplyCharacterProfile(PlayerCharacterProfile profile)
        {
            List<PlayerStatOverride> overrides = profile.Overrides;
            List<PlayerStatModification> modifications = profile.Modifications;
            List<PlayerStatCap> caps = profile.Caps;

            overrides.ForEach(ovr =>
            {
                OverrideWith(ovr);
            });

            modifications.ForEach(mod =>
            {
                ModifyWith(mod);
            });

            caps.ForEach(cap =>
            {
                CapWith(cap);
            });
        }

        public Mutation<float> ModifyWith(PlayerStatModification mod)
        {
            Mutation<float> result = null;

            if (mod.ModificationType == StatModificationType.Incremental)
            {
                result = ModifyIncremental(mod.TargetStatType, mod.Value, AffectionMethod.InOrder);
            }

            else if (mod.ModificationType == StatModificationType.Percentage)
            {
                bool onTop = StatSystemSettings.PERCENTAGE_MODS_ON_TOP;
                AffectionMethod affectionMethod = onTop ? AffectionMethod.Overall : AffectionMethod.InOrder;

                result = ModifyPercentage(mod.TargetStatType, mod.Value, affectionMethod);
            }

            return result;
        }

        public FloatCapMutation CapWith(PlayerStatCap cap)
        {
            FloatCapMutation result = new FloatCapMutation()
            {
                CapLow = cap.CapLow,
                CapHigh = cap.CapHigh,
                MinValue = cap.MinValue,
                MaxValue = cap.MaxValue,
            };

            ModifyCustom(cap.TargetStatType, result);

            return result;
        }

        public float OverrideWith(PlayerStatOverride ovr)
        {
            Float targetVariable = GetDesiredStatVariable(ovr.TargetStatType);
            bool clear = StatSystemSettings.OVERRIDES_CLEAR_MUTATIONS;
            float newValue = ovr.NewValue;

            if (clear)
            {
                targetVariable.ClearMutations();
                targetVariable.Value = newValue;
                return newValue;
            }

            bool root = StatSystemSettings.OVERRIDES_AFFECT_FROM_ROOT;
            SetType setType = root ? SetType.Raw : SetType.Baked;

            targetVariable.Set(newValue, setType);
            return targetVariable.Value;
        }

        public Mutation<float> ModifyCustom(PlayerStatType targetStat, Mutation<float> mutationObject)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            desiredStatVariable.Mutate(mutationObject);
            return mutationObject;
        }

        public FloatAdditionMutation ModifyIncremental(PlayerStatType targetStat, float amount, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            FloatAdditionMutation mutationObject = new(amount, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return mutationObject;
        }

        public FloatMultiplicationMutation ModifyPercentage(PlayerStatType targetStat, float percentage, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            if (StatSystemSettings.PERCENTAGE_MODS_ON_TOP) affectionMethod = AffectionMethod.Overall;

            if (!TryGetDesiredStatVariable(targetStat, out Float desiredStatVariable)) return null;

            float realPercentage = 1f + (percentage / 100f);
            FloatMultiplicationMutation mutationObject = new(realPercentage, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return mutationObject;
        }

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

        public bool TryGetDesiredStatVariable(PlayerStatType targetStat, out Float desiredStatVariable)
        {
            desiredStatVariable = GetDesiredStatVariable(targetStat);
            if (desiredStatVariable != null) return true;
            else return false;
        }
    }
}
