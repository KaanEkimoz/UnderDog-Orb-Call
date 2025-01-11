using com.absence.variablesystem.builtin;
using com.game.player.scriptables;
using com.game.statsystem;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player.statsystemextensions
{
    /// <summary>
    /// An example use case of <see cref="StatHolder{T}"/>.
    /// </summary>
    [System.Serializable]
    public sealed class PlayerStatHolder : StatHolder<PlayerStatType>
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

        protected override Dictionary<PlayerStatType, StatObject> GenerateDefaultEntries()
        {
            return new Dictionary<PlayerStatType, StatObject>()
            {
                { PlayerStatType.Health, new(m_health) },
                { PlayerStatType.Armor, new(m_armor) },
                { PlayerStatType.WalkSpeed, new(m_walkSpeed) },
                { PlayerStatType.LifeSteal, new(m_lifeSteal) },
                { PlayerStatType.Luck, new(m_luck) },
                { PlayerStatType.Gathering, new(m_gathering) },
                { PlayerStatType.Damage, new(m_damage) },
                { PlayerStatType.AttackSpeed, new(m_attackSpeed) },
                { PlayerStatType.CriticalHits, new(m_criticalHits) },
                { PlayerStatType.Range, new(m_range) },
                { PlayerStatType.Knockback, new(m_knockback) },
                { PlayerStatType.Penetration, new(m_penetration) },
                { PlayerStatType.CrowdControl, new(m_crowdControl) },
                { PlayerStatType.LightStrength, new(m_lightStrength) },
            };
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

    }
}
