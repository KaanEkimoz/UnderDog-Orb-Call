using com.game.entitysystem;
using com.game.testing;
using UnityEngine;

namespace com.game.player
{
    public class PlayerComponentHub : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_stats;
        [SerializeField] private PlayerOrbHandler_Test m_orbHandler;
        [SerializeField] private PlayerInventory m_inventory;
        [SerializeField] private Entity m_entity;

        public PlayerStats Stats => m_stats;
        public PlayerOrbHandler_Test OrbHandler => m_orbHandler;
        public PlayerInventory Inventory => m_inventory;
    }
}
