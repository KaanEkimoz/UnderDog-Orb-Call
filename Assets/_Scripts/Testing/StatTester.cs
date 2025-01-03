using com.game.player;
using UnityEngine;

namespace com.game.testing
{
    public class StatTester : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_playerStats;

        private void OnGUI()
        {
            m_playerStats.StatHolder.ForAllStatEntries((key, value) =>
            {
                GUILayout.Label($"{key}: {value.Value}");
            });
        }
    }
}
