using com.absence.attributes;
using com.game.player;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game
{
    [System.Serializable]
    public class Cost
    {
        [Min(0f)] public int MoneyCost = 0;
        [Min(0f)] public int ExperienceCost = 0;
        [Min(0f)] public int HealthCost = 0;
        public bool ConstantStatCosts = false;
        [HideIf(nameof(ConstantStatCosts)), Min(0f)] public float StatCostDuration = 0f;

        public List<PlayerStatModification> StatModificationCosts = new();
        public List<PlayerStatCap> StatCapCosts = new();
        public List<PlayerStatOverride> StatOverrideCosts = new();

        public bool CanAfford(Player target)
        {
            bool canAffordMoney = target.Hub.Money.Money >= MoneyCost;
            bool canAffordExperience = target.Hub.Leveling.CurrentExperience >= ExperienceCost;
            bool canAffordHealth = target.Hub.Combatant.Health >= HealthCost;

            return canAffordMoney && canAffordExperience && canAffordHealth;
        }

        public void Perform(Player target)
        {
            target.Hub.Money.Spend(MoneyCost);
            target.Hub.Leveling.LoseExperience(ExperienceCost);

            foreach (var mod in StatModificationCosts)
            {
                target.Hub.Stats.Manipulator.ModifyWith(mod);
            }

            foreach (var mod in StatCapCosts)
            {
                target.Hub.Stats.Manipulator.CapWith(mod);
            }

            foreach (var mod in StatOverrideCosts)
            {
                target.Hub.Stats.Manipulator.OverrideWith(mod);
            }
        }

        public string GenerateDescription(bool richText = false)
        {
            bool noMoneyCost = MoneyCost == 0;
            bool noExperienceCost = ExperienceCost == 0;
            bool noHealthCost = HealthCost == 0;
            bool noStatCost = StatCostDuration == 0 && (!ConstantStatCosts);

            if (noMoneyCost && noExperienceCost && noStatCost && noHealthCost)
                return "No cost.";

            StringBuilder sb = new("Cost: \n");

            if (!noHealthCost)
            {
                if (noExperienceCost && noMoneyCost && noStatCost)
                    return $"Costs {HealthCost} Health.";

                sb.Append($"\t{HealthCost} Health\n");
            }

            if (!noMoneyCost)
            {
                if (noExperienceCost && noHealthCost && noStatCost)
                    return $"Costs {MoneyCost}$.";

                sb.Append($"\t{MoneyCost}$\n");
            }

            if (!noExperienceCost)
            {
                if (noMoneyCost && noHealthCost && noStatCost)
                    return $"Costs {ExperienceCost} Exp.";

                sb.Append($"\t{ExperienceCost} Exp\n");
            }

            if (!noStatCost)
            {
                foreach (var mod in StatModificationCosts)
                {
                    sb.Append("\t");
                    sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                    sb.Append("\n");
                }

                foreach (var mod in StatCapCosts)
                {
                    sb.Append("\t");
                    sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                    sb.Append("\n");
                }

                foreach (var mod in StatOverrideCosts)
                {
                    sb.Append("\t");
                    sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                    sb.Append("\n");
                }

                if (!ConstantStatCosts)
                    sb.Append($"for {StatCostDuration} seconds.");
            }

            return sb.ToString();
        }
    }
}
