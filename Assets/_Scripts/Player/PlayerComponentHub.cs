using UnityEngine;

namespace com.game.player
{
    public class PlayerComponentHub : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_stats;
        [SerializeField] private PlayerInventory m_inventory;

        public PlayerStats Stats => m_stats;
        public PlayerInventory Inventory => m_inventory;
    }
}
