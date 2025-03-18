using com.game.entitysystem;
using com.game.orbsystem;
using UnityEngine;

namespace com.game.player
{
    public class PlayerComponentHub : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_stats;
        [SerializeField] private PlayerOrbHandler m_orbHandler;
        [SerializeField] private PlayerOrbContainer m_orbContainer;
        [SerializeField] private PlayerInventory m_inventory;
        [SerializeField] private PlayerLevelingLogic m_leveling;
        [SerializeField] private PlayerParanoiaLogic m_paranoia;
        [SerializeField] private PlayerMoneyLogic m_money;
        [SerializeField] private PlayerShop m_playerShop;
        [SerializeField] private OrbShop m_orbUpgradeShop;
        [SerializeField] private PlayerAbilitySet m_abilities;
        [SerializeField] private PlayerCombatant m_combatant;
        [SerializeField] private Entity m_entity;

        public PlayerStats Stats => m_stats;
        public PlayerOrbContainer OrbContainer => m_orbContainer;
        public PlayerOrbHandler OrbHandler => m_orbHandler;
        public PlayerInventory Inventory => m_inventory;
        public PlayerLevelingLogic Leveling => m_leveling;
        public PlayerParanoiaLogic Paranoia => m_paranoia;
        public PlayerMoneyLogic Money => m_money;
        public PlayerShop Shop => m_playerShop;
        public OrbShop OrbShop => m_orbUpgradeShop;
        public PlayerAbilitySet Abilities => m_abilities;
        public PlayerCombatant Combatant => m_combatant;
    }
}
