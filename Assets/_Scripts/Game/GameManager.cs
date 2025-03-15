using com.absence.utilities;
using com.game.enemysystem;
using com.game.events;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.ui;
using System.Collections.Generic;
using UnityEngine;

namespace com.game
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] OrbShopUI m_orbShopUI;
        [SerializeField] PlayerShopUI m_playerShopUI;
        [SerializeField] OrbContainerUI m_orbContainerUI;
        [SerializeField] OrbUIUpdater m_orbUIUpdater;

        int m_levelsGained;
        List<OrbItemProfile> m_orbUpgradeCache;

        PlayerStats m_playerStats;
        PlayerInventory m_playerInventory;
        PlayerLevelingLogic m_playerLevelingLogic;
        PlayerOrbContainer m_orbContainer;
        OrbShop m_orbShop;
        PlayerShop m_playerShop;

        private void Start()
        {
            m_playerStats = Player.Instance.Hub.Stats;
            m_playerInventory = Player.Instance.Hub.Inventory;
            m_playerLevelingLogic = Player.Instance.Hub.Leveling;
            m_orbShop = Player.Instance.Hub.OrbShop;
            m_playerShop = Player.Instance.Hub.Shop;
            m_orbContainer = Player.Instance.Hub.OrbContainer;

            GameEventChannel.OnWaveEnded += OnWaveEnd;
            PlayerEventChannel.OnLevelUp += OnPlayerLevelUp;
        }

        private void OnPlayerLevelUp(PlayerLevelingLogic logic)
        {
            m_levelsGained++;
        }

        private void OnWaveEnd()
        {
            Game.Pause();

            foreach (EnemyCombatant enemy in GameObject.FindObjectsByType<EnemyCombatant>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                enemy.Die();
            }

            m_orbShopUI.Hide(true);
            m_orbContainerUI.Hide(true);
            m_playerShopUI.Hide(true);

            m_orbContainerUI.SoftRefresh();

            if (m_levelsGained > 0) EnterLevelUpMenu();
            else if (m_orbContainer.UpgradeCache != null && m_orbContainer.UpgradeCache.Count > 0) EnterOrbInventory();
            else EnterShop();
        }

        void EnterLevelUpMenu(bool reroll = true)
        {
            m_orbShopUI.Show(reroll);
            m_orbShopUI.Title.text = m_levelsGained > 1 ? $"Level Up! ({m_levelsGained})" : "Level Up!";
            m_orbShopUI.InventoryButton.OnClick += EnterOrbInventoryTemporarily;
            m_orbShopUI.PassButton.OnClick += PassOrbUpgrades;
            m_orbShopUI.OnItemBought -= OnOrbUpgradeBought;
            m_orbShopUI.OnItemBought += OnOrbUpgradeBought;
        }

        private void EnterOrbInventoryTemporarily()
        {
            m_orbContainerUI.Show(false);
            m_orbContainerUI.BackButton.OnClick += () => m_orbContainerUI.Hide(false);
            m_orbContainerUI.ConfirmButton.OnClick += OnConfirmUpgrades;
        }

        private void PassOrbUpgrades()
        {
            m_levelsGained = 0;
            if (m_orbContainer.UpgradeCache != null && m_orbContainer.UpgradeCache.Count > 0) EnterOrbInventory();
            else EnterShop();
        }

        void OnOrbUpgradeBought(OrbItemProfile profile)
        {
            m_orbUpgradeCache.Add(profile);
            m_levelsGained--;

            if (m_levelsGained > 0)
                EnterLevelUpMenu();
            else if (m_orbUpgradeCache.Count > 0)
                EnterOrbInventory();
            else
                EnterShop();
        }

        private void EnterOrbInventory()
        {
            m_orbShopUI.Hide(true);
            m_orbContainerUI.SetUpgradeCache(m_orbUpgradeCache);
            m_orbContainerUI.Show(true);
            m_orbContainerUI.ConfirmButton.OnClick += () =>
            {
                OnConfirmUpgrades();
                EnterShop();
            };
        }

        private void OnConfirmUpgrades()
        {
            m_orbUpgradeCache = new(m_orbContainer.UpgradeCache);
            m_orbContainerUI.SoftRefresh();
            m_orbContainerUI.Hide(false);
        }

        private void EnterShop()
        {
            m_orbShopUI.Hide(true);
            m_playerShopUI.Show(true);
            m_playerShopUI.InventoryButton.OnClick += EnterOrbInventoryTemporarily;
            m_playerShopUI.ProceedButton.OnClick += ExitShop;
        }

        private void ExitShop()
        {
            m_playerShopUI.Hide(true);

            Game.Resume();

            m_orbUIUpdater.Redraw();
        }
    }
}
