using com.absence.attributes;
using com.absence.utilities;
using com.game.itemsystem.scriptables;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.player.itemsystemextensions
{
    [CreateAssetMenu(fileName = "New Player Item", menuName = "Game/Item System/ Player Item Profile", order = int.MinValue)]
    public class PlayerItemProfile : ItemProfile
    {
        [Space, Header3("Effects on Player Stats")]

        [SerializeField] private List<PlayerStatOverride> m_playerStatOverrides;
        [SerializeField] private List<PlayerStatModification> m_playerStatModifications;
        [SerializeField] private List<PlayerStatCap> m_playerStatCaps;

        public override string GenerateStatDescription(bool richText)
        {
            StringBuilder sb = new();

            m_playerStatOverrides.ForEach(ovr => 
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(ovr, richText));
                sb.Append("\n");
            });

            m_playerStatModifications.ForEach(mod =>
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                sb.Append("\n");
            });

            m_playerStatCaps.ForEach(cap =>
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(cap, richText));
                sb.Append("\n");
            });

            return sb.ToString();
        }
    }
}
