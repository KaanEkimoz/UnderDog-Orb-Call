using com.absence.utilities;
using com.game.itemsystem.scriptables;
using com.game.player;
using com.game.player.statsystemextensions;
using UnityEngine;

namespace com.game.testing
{
    public class StatTester : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_playerStats;

        [SerializeField] private ItemProfile m_itemProfile;

        [SerializeField] private PlayerStatModification m_mod;

        private void OnGUI()
        {
            m_playerStats.StatHolder.ForAllStatEntries((key, value) =>
            {
                GUILayout.Label($"{Helpers.SplitCamelCase(key.ToString(), " ")}: {value}");
            });
        }
    }
}
