using com.absence.attributes;
using com.absence.utilities;
using com.game.enemysystem;
using com.game.events;
using com.game.miscs;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.ui;
using System.Collections.Generic;
using UnityEngine;

namespace com.game
{
    public class SceneManager : Singleton<SceneManager>
    {
        public const FindObjectsInactive INCLUDE_INACTIVES_AUTO_FILL = FindObjectsInactive.Include;

        [Header1("Scene Manager")]

        [Space, Header2("Settings")]

        [SerializeField] private bool m_waveCycleEnabled;
        [SerializeField] private bool m_lightSystemEnabled;

        [Space, Header2("Fields")]

        [SerializeField, ShowIf(nameof(m_waveCycleEnabled)), Required] OrbShopUI m_orbShopUI;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled)), Required] PlayerShopUI m_playerShopUI;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled)), Required] OrbContainerUI m_orbContainerUI;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled))] OrbUIUpdater m_orbUIUpdater;
        [SerializeField, ShowIf(nameof(m_waveCycleEnabled))] OrbUIUpdater m_orbUIUpdater2;
        [SerializeField] GameObject m_defaultEnvironmentLight;
        [SerializeField] GameObject m_lightSystemEnvironmentLight;
        [SerializeField] private Transform m_altar;

        public int LevelsGainedCurrentWave => m_levelsGained;
        public int EndedWaveCount => m_wavesEnded;
        public float StartTimeOfCurrentWave => m_waveStartTime;

        public bool WaveCycleEnabled => m_waveCycleEnabled;
        public bool LightSystemInUse => m_lightSystemEnabled;

        public Transform AltarTransform => m_altar;

        PlayerParanoiaLogic m_playerParanoiaLogic;
        int m_levelsGained;
        int m_wavesEnded;
        float m_waveStartTime;
        List<OrbItemProfile> m_orbUpgradeCache;

        protected override void Awake()
        {
            base.Awake();

            if (GameManager.Instance == null)
            {
                Debug.LogError("A SceneManager needs a GameManager to work!");
                enabled = false;
                return;
            }

            Initialize();
            SubscribeToEvents();
            GameManager.Instance.SetState(GameState.NotStarted, true); // starts the game.
        }

        void Initialize()
        {
            m_playerParanoiaLogic = Player.Instance != null ? Player.Instance.Hub.Paranoia : null;

            if (m_defaultEnvironmentLight != null) 
                m_defaultEnvironmentLight.SetActive(!m_lightSystemEnabled);

            if (m_lightSystemEnvironmentLight != null) 
                m_lightSystemEnvironmentLight.SetActive(m_lightSystemEnabled);
        }

        void SubscribeToEvents()
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
            GameEventChannel.OnWaveEnded += OnWaveEnd;
            PlayerEventChannel.OnLevelUp += OnPlayerLevelUp;
        }

        private void OnGameStateChanged(GameState prevState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Stateless:
                    break;
                case GameState.RunSelection:
                    break;
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
            if (GameManager.Instance.State != GameState.InWave)
                return;

            m_levelsGained += logic.LastLevelGain;
        }
        void OnWaveEnd()
        {
            GameManager.Instance.SetState(GameState.BetweenWaves);
        }

        #endregion

        #region State: Not Started

        void DoNotStarted()
        {
            m_orbUpgradeCache = new();
            m_levelsGained = 0;
            m_wavesEnded = 0;

            GameManager.Instance.SetState(GameState.InWave);
        }

        #endregion

        #region State: Between Waves

        void DoBetweenWaves()
        {
            m_wavesEnded++;
            Game.Pause();

            if (!m_waveCycleEnabled)
            {
                GameManager.Instance.SetState(GameState.InWave);
                return;
            }

            m_orbUpgradeCache = m_orbContainerUI.Container.UpgradeCache;

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

            if (m_orbContainerUI.Container.UpgradeCache != null && m_orbContainerUI.Container.UpgradeCache.Count > 0) 
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

            if ((m_orbContainerUI.Container.UpgradeCache != null && m_orbContainerUI.Container.UpgradeCache.Count > 0) || 
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
            m_orbUpgradeCache = new(m_orbContainerUI.Container.UpgradeCache);
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
            GameManager.Instance.SetState(GameState.InWave);
        }

        #endregion

        #region State: In Wave

        void DoInWave()
        {
            Game.Resume();
            m_waveStartTime = Time.time;

            if (!m_waveCycleEnabled)
                return;

            m_playerShopUI.Hide(true);

            if (m_orbUIUpdater != null) m_orbUIUpdater.Refresh();
            if (m_orbUIUpdater2 != null) m_orbUIUpdater2.Refresh();
            m_playerParanoiaLogic.SetToSegment(m_wavesEnded);
        }

        #endregion

        [Button("Search and Auto-Fill")]
        void AutoFill()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;

            UnityEditor.Undo.RecordObject(this, "Scene Manager Auto-Fill (Inspector)");
#endif

            if (m_waveCycleEnabled)
            {
                m_orbShopUI = FindFirstObjectByType<OrbShopUI>(INCLUDE_INACTIVES_AUTO_FILL);
                m_playerShopUI = FindFirstObjectByType<PlayerShopUI>(INCLUDE_INACTIVES_AUTO_FILL);
                m_orbContainerUI = FindFirstObjectByType<OrbContainerUI>(INCLUDE_INACTIVES_AUTO_FILL);

                OrbUIUpdater[] orbUIUpdaters = FindObjectsByType<OrbUIUpdater>(FindObjectsSortMode.None);
                bool hasUIUpdaters = !(orbUIUpdaters == null || orbUIUpdaters.Length == 0);

                if (hasUIUpdaters)
                {
                    m_orbUIUpdater = orbUIUpdaters[0];
                    if (orbUIUpdaters.Length > 1) m_orbUIUpdater2 = orbUIUpdaters[1];
                }
            }

            AltarInteraction altar = FindFirstObjectByType<AltarInteraction>();
            m_altar = altar != null ? altar.transform : null;

#if UNITY_EDITOR
            if (Application.isPlaying)
                return;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        [Button("Toggle Light System Lights")]
        void ToggleLights()
        {
            if (Application.isPlaying)
                return;

#if UNITY_EDITOR
            UnityEditor.Undo.IncrementCurrentGroup();
            UnityEditor.Undo.SetCurrentGroupName("Scene Manager Toggle Lights (Inspector)");
            int undoGroupIndex = UnityEditor.Undo.GetCurrentGroup();

            UnityEditor.Undo.RecordObject(this, "0");
#endif

            m_lightSystemEnabled = !m_lightSystemEnabled;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif

            if (m_lightSystemEnvironmentLight != null)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(m_lightSystemEnvironmentLight, "1");
#endif

                m_lightSystemEnvironmentLight.SetActive(m_lightSystemEnabled);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(m_lightSystemEnvironmentLight);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(m_lightSystemEnvironmentLight);
#endif
            }

            if (m_defaultEnvironmentLight != null)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(m_defaultEnvironmentLight, "2");
#endif

                m_defaultEnvironmentLight.SetActive(!m_lightSystemEnabled);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(m_defaultEnvironmentLight);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(m_defaultEnvironmentLight);
#endif
            }

#if UNITY_EDITOR
            UnityEditor.Undo.CollapseUndoOperations(undoGroupIndex);
#endif
        }
    }
}
