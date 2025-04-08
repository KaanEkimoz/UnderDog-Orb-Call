using com.absence.attributes;
using com.absence.utilities;
using com.game.enemysystem;
using com.game.events;
using com.game.miscs;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.ui;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField, Readonly] private GameState m_state = GameState.NotStarted;

        [SerializeField] OrbShopUI m_orbShopUI;
        [SerializeField] PlayerShopUI m_playerShopUI;
        [SerializeField] OrbContainerUI m_orbContainerUI;
        [SerializeField] OrbUIUpdater m_orbUIUpdater;
        [SerializeField] OrbUIUpdater m_orbUIUpdater2;

        [SerializeField] private Transform m_altar;

        public GameState State => m_state;
        public int LevelsGainedCurrentWave => m_levelsGained;
        public int EndedWaveCount => m_wavesEnded;
        public float StartTimeOfCurrentWave => m_waveStartTime;

        public Transform AltarTransform => m_altar;

        PlayerStats m_playerStats;
        PlayerInventory m_playerInventory;
        PlayerLevelingLogic m_playerLevelingLogic;
        PlayerParanoiaLogic m_playerParanoiaLogic;
        PlayerOrbContainer m_orbContainer;
        OrbShop m_orbShop;
        PlayerShop m_playerShop;

        int m_levelsGained;
        int m_wavesEnded;
        float m_waveStartTime;
        List<OrbItemProfile> m_orbUpgradeCache;

        private void Start()
        {
            Initialize();
            SubscribeToEvents();
            SetState(GameState.NotStarted, true); // starts the game.
        }

        void Initialize()
        {
            m_playerStats = Player.Instance.Hub.Stats;
            m_playerInventory = Player.Instance.Hub.Inventory;
            m_playerLevelingLogic = Player.Instance.Hub.Leveling;
            m_playerParanoiaLogic = Player.Instance.Hub.Paranoia;
            m_orbShop = Player.Instance.Hub.OrbShop;
            m_playerShop = Player.Instance.Hub.Shop;
            m_orbContainer = Player.Instance.Hub.OrbContainer;
        }
        void SubscribeToEvents()
        {
            GameEventChannel.OnWaveEnded += OnWaveEnd;
            PlayerEventChannel.OnLevelUp += OnPlayerLevelUp;
        }

        public bool SetState(GameState newState, bool force = false)
        {
            if ((!force) && m_state.Equals(newState))
                return false;

            m_state = newState;
            DoSetState();
            return true;
        }

        void DoSetState()
        {
            switch (m_state)
            {
                case GameState.NotStarted:
                    DoNotStarted();
                    break;
                case GameState.InWave:
                    DoInWave();
                    break;
                case GameState.BetweenWaves:
                    DoBetweenWaves();
                    break;
                default:
                    break;
            }
        }

        #region Callbacks

        void OnPlayerLevelUp(PlayerLevelingLogic logic)
        {
            if (m_state != GameState.InWave)
                return;

            m_levelsGained += logic.LastLevelGain;
        }
        void OnWaveEnd()
        {
            SetState(GameState.BetweenWaves);
        }

        #endregion

        #region State: Not Started

        void DoNotStarted()
        {
            m_orbUpgradeCache = new();
            m_levelsGained = 0;
            m_wavesEnded = 0;

            SetState(GameState.InWave);
        }

        #endregion

        #region State: Between Waves

        void DoBetweenWaves()
        {
            m_wavesEnded++;

            Game.Pause();

            m_orbUpgradeCache = m_orbContainer.UpgradeCache;

            foreach (EnemyCombatant enemy in GameObject.FindObjectsByType<EnemyCombatant>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (DropManager.Instance != null) DropManager.Instance.Enabled = false;
                enemy.Die(DeathCause.Internal);
                if (DropManager.Instance != null) DropManager.Instance.Enabled = true;
            }

            m_orbShopUI.Hide(true);
            m_orbContainerUI.Hide(true);
            m_playerShopUI.Hide(true);

            m_orbContainerUI.SoftRefresh();

            if (m_levelsGained > 0)
            {
                EnterLevelUpMenu();
                return;
            }

            EnterShop();

            if (m_orbContainer.UpgradeCache != null && m_orbContainer.UpgradeCache.Count > 0) 
                EnterOrbInventoryTemporarily();
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
        void EnterOrbInventoryTemporarily()
        {
            m_orbContainerUI.Show(false, true);
            m_orbContainerUI.RefreshButtons();
            m_orbContainerUI.BackButton.OnClick += () => m_orbContainerUI.Hide(false);
            m_orbContainerUI.ConfirmButton.OnClick += OnConfirmUpgrades;
        }
        void PassOrbUpgrades()
        {
            m_levelsGained = 0;

            EnterShop();

            if ((m_orbContainer.UpgradeCache != null && m_orbContainer.UpgradeCache.Count > 0) || 
                (m_orbUpgradeCache != null && m_orbUpgradeCache.Count > 0))
                EnterOrbInventoryTemporarily();
        }
        void OnOrbUpgradeBought(OrbItemProfile profile)
        {
            if (m_orbUpgradeCache == null) m_orbUpgradeCache = new();
            m_orbUpgradeCache.Add(profile);
            m_orbContainerUI.SetUpgradeCache(m_orbUpgradeCache);
            m_orbContainerUI.SoftRefresh();
            m_levelsGained--;

            if (m_levelsGained > 0)
            {
                EnterLevelUpMenu();
                return;
            }

            EnterShop();

            if (m_orbUpgradeCache.Count > 0)
                EnterOrbInventoryTemporarily();
        }
        void OnConfirmUpgrades()
        {
            m_orbUpgradeCache = new(m_orbContainer.UpgradeCache);
            m_orbContainerUI.SoftRefresh();
            m_orbContainerUI.Hide(false);
        }
        void EnterShop()
        {
            m_orbShopUI.Hide(true);
            m_playerShopUI.Show(true);
            m_playerShopUI.InventoryButton.OnClick += EnterOrbInventoryTemporarily;
            m_playerShopUI.ProceedButton.OnClick += ExitShop;
        }
        void ExitShop()
        {
            SetState(GameState.InWave);
        }

        #endregion

        #region State: In Wave

        void DoInWave()
        {
            m_playerShopUI.Hide(true);

            Game.Resume();

            m_waveStartTime = Time.time;

            if (m_orbUIUpdater != null) m_orbUIUpdater.Refresh();
            if (m_orbUIUpdater2 != null) m_orbUIUpdater2.Refresh();
            m_playerParanoiaLogic.SetToSegment(m_wavesEnded);
        }

        #endregion
    }
}
