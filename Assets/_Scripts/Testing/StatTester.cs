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

        private void OnGUI()
        {
            m_playerStats.StatHolder.ForAllStatEntries((key, value) =>
            {
                GUILayout.Label($"{key}: {value.Value}");
            });
        }

        //m_itemProfile
    }
}
