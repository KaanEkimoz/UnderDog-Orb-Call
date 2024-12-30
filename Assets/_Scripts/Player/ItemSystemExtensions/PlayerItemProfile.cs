using com.absence.attributes;
using com.game.itemsystem.scriptables;
using com.game.player.statsystemextensions;
using System.Collections.Generic;
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
    }
}
