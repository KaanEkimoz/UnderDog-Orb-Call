using com.absence.attributes;
using com.game.orbsystem.statsystemextensions;
using com.game.player.statsystemextensions;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.itemsystem.scriptables
{
    [CreateAssetMenu(fileName = "New ItemProfile", menuName = "Game/Item Profile", order = int.MinValue)]
    public class ItemProfile : ScriptableObject
    {
        [Header1("Item Profile")]

        [Space]
        [Header3("Effects on Player Stats")]

        public List<PlayerStatOverride> PlayerStatOverrides;
        public List<PlayerStatModification> PlayerStatModifications;
        public List<PlayerStatCap> PlayerStatCaps;

        [Space]
        [Header3("Effects on Orb Stats")]

        public List<OrbStatOverride> OrbStatOverrides;
        public List<OrbStatModification> OrbStatModifications;
        public List<OrbStatCap> OrbStatCaps;

        [Space]
        [Header3("Custom Actions")]
        [SerializeField] private float m_notReadyYet;
    }
}
