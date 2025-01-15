using com.game.itemsystem;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using System.Text;
using UnityEngine;

namespace com.game.generics.itembehaviours
{
    public class EnemiesKilledInWaveItemBehaviour : ItemBehaviour
    {
        [SerializeField, Min(1)] private int m_amountOfEnemies = 1;
        [SerializeField] private PlayerStatModification m_modification = new();

        public override string GenerateDescription(bool richText)
        {
            float value = m_modification.Value;

            string sign = value > 0 ? "+" : "";
            string mode = m_modification.ModificationType ==
                statsystem.presetobjects.StatModificationType.Percentage ? "%" : "";

            StringBuilder sb = new();

            if (richText) sb.Append("<b>");

            sb.Append(sign);
            sb.Append(value);
            sb.Append(mode);

            if (richText) sb.Append("</b>");

            sb.Append(" ");

            sb.Append(StatSystemHelpers.Text.GetDisplayName(m_modification.TargetStatType, richText));

            sb.Append(" for every ");

            if (richText) sb.Append("<b>");

            sb.Append(m_amountOfEnemies);

            if (richText) sb.Append("</b>");

            sb.Append(" enemies killed in a wave.");

            return sb.ToString();
        }
    }
}
