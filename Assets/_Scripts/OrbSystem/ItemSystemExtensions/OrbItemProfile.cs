using com.absence.attributes;
using com.game.itemsystem.scriptables;
using com.game.orbsystem.statsystemextensions;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.orbsystem.itemsystemextensions
{
    [CreateAssetMenu(fileName = "New Orb Item", menuName = "Game/Item System/ Orb Item Profile", order = int.MinValue)]
    public class OrbItemProfile : ItemProfile
    {
        [Space, Header3("Effects on Orb Stats")]

        [SerializeField] private List<OrbStatOverride> m_orbStatOverrides;
        [SerializeField] private List<OrbStatModification> m_orbStatModifications;
        [SerializeField] private List<OrbStatCap> m_orbStatCaps;
    }
}
