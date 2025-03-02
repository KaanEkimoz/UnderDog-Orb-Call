using com.game.abilitysystem.ui;
using com.game.enemysystem;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using com.game.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.game.testing
{
    public class GameTester : MonoBehaviour
    {
        private static readonly bool s_performanceMode = false;

        const float k_totalStatAreaWidth = 200f;
        const float k_totalButtonAreaWidth = 170f;

        const float k_initialAdditionButtonAmount = 1f;
        const float k_initialPercentageButtonAmount = 15f;

        const float k_minButtonAmount = 1f;

        const float k_maxPercentageButtonAmount = 50f;
        const float k_maxAdditionButtonAmount = 10f;

        const float k_utilityPanelWidth = 100f;

        [SerializeField] private bool m_displayUnpaused = false;
        [SerializeField] private bool m_displayPaused = false;
        [SerializeField] private AbilityDisplayGP m_parryDisplay;
        [SerializeField] private Parry m_parry;
        [SerializeField] OrbShopUI m_orbShopUI;
        [SerializeField] PlayerShopUI m_playerShopUI;
        [SerializeField] OrbContainerUI m_orbContainerUI;

        Dictionary<PlayerStatType, ModifierObject<PlayerStatType>> m_additionalDict = new();
        Dictionary<PlayerStatType, ModifierObject<PlayerStatType>> m_percentageDict = new();

        PlayerStats m_playerStats;
        PlayerInventory m_playerInventory;
        PlayerLevelingLogic m_playerLevelingLogic;
        OrbShop m_orbShop;
        PlayerShop m_playerShop;

        float m_additionButtonWidth;
        float m_percentageButtonWidth;
        float m_additionButtonAmount;
        float m_percentageButtonAmount;
        bool m_pausedGame;
        bool m_passGUI;

        int m_levelsGained;
        List<OrbItemProfile> m_orbUpgradeCache;

        private void Start()
        {
            m_passGUI = false;
            m_levelsGained = 0;
            m_orbUpgradeCache = new();

            m_playerStats = Player.Instance.Hub.Stats;
            m_playerInventory = Player.Instance.Hub.Inventory;
            m_playerLevelingLogic = Player.Instance.Hub.Leveling;
            m_orbShop = Player.Instance.Hub.OrbShop;
            m_playerShop = Player.Instance.Hub.Shop;

            m_playerLevelingLogic.OnLevelUp += OnPlayerLevelUp;

            m_additionButtonWidth = k_totalButtonAreaWidth * 2 / 5 / 2;
            m_percentageButtonWidth = k_totalButtonAreaWidth * 3 / 5 / 2;

            m_additionButtonAmount = k_initialAdditionButtonAmount;
            m_percentageButtonAmount = k_initialPercentageButtonAmount;

            m_parryDisplay.Initialize(m_parry);
        }

        private void OnPlayerLevelUp(PlayerLevelingLogic logic)
        {
            m_levelsGained++;
        }

        private void OnWaveEnd()
        {
            m_pausedGame = false;
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

            m_passGUI = true;
            if (m_levelsGained > 0) EnterLevelUpMenu();
            else EnterShop();
        }

        void EnterLevelUpMenu(bool reroll = true)
        {
            m_orbShopUI.Show(reroll);
            m_orbShopUI.Title.text = m_levelsGained > 1 ? $"Level Up! ({m_levelsGained})" : "Level Up!";
            m_orbShopUI.SetupButtons(EnterOrbInventoryTemporarily, PassOrbUpgrades);
            m_orbShopUI.OnItemBought -= OnOrbUpgradeBought;
            m_orbShopUI.OnItemBought += OnOrbUpgradeBought;
        }

        private void EnterOrbInventoryTemporarily()
        {
            //m_orbShopUI.Hide(false);
            //m_orbContainerUI.SetUpgradeCache(null);
            m_orbContainerUI.Show(false);
            m_orbContainerUI.SetupButtons(() =>
            {
                m_orbContainerUI.Hide(false);
                EnterLevelUpMenu(false);
            });
        }

        private void PassOrbUpgrades()
        {
            m_levelsGained = 0;
            EnterOrbInventory();
        }

        void OnOrbUpgradeBought(OrbItemProfile profile)
        {
            m_orbUpgradeCache.Add(profile);
            m_levelsGained--;

            if (m_levelsGained > 0)
                EnterLevelUpMenu();
            else
                EnterOrbInventory();
        }

        private void EnterOrbInventory()
        {
            m_orbShopUI.Hide(true);
            m_orbContainerUI.SetUpgradeCache(m_orbUpgradeCache);
            m_orbContainerUI.Show(true);

            m_orbContainerUI.SetupButtons(null, OnConfirmUpgrades);
        }

        private void OnConfirmUpgrades()
        {
            m_orbContainerUI.Hide(true);
            EnterShop();
        }

        private void EnterShop()
        {
            m_playerShopUI.Show(true);
            m_playerShopUI.SetupButtons(ExitShop);
        }

        private void ExitShop()
        {
            m_playerShopUI.Hide(true);

            m_pausedGame = true;
            m_passGUI = false;
            Game.Resume();
        }

        private void Update()
        {
            if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

            if (Game.Paused)
            {
                if (m_pausedGame) Game.Resume();
            }
            else 
            {
                m_pausedGame = true;
                Game.Pause();
            }
        }

        private void OnDestroy()
        {
            PlayerStatPipelineComponentBase rawComponent = m_playerStats.Pipeline.Query.
                FirstOrDefault(comp => comp is PlayerStatPipelineOrbCountEffect);

            bool exists = rawComponent != null;

            if (!exists) return;

            PlayerStatPipelineOrbCountEffect orbCountEffect =
                rawComponent as PlayerStatPipelineOrbCountEffect;

            StringBuilder sb = new("THESE WERE THE GRAPH DATA IF YOU'VE FORGOTTEN TO CHECK BEFORE QUITTING:\n\n");

            sb.Append($"Amplitude: {orbCountEffect.Amplitude}\n");
            sb.Append($"Shift: {orbCountEffect.Shift}\n");
            sb.Append($"General Coefficient: {orbCountEffect.GeneralCoefficient}\n");
            sb.Append($"Curve Type: {orbCountEffect.CurveType}\n");

            Debug.LogWarning(sb.ToString());
        }

        private void OnGUI()
        {
            if (m_passGUI)
                return;

            if (!Game.Paused)
            {
                if (!m_displayUnpaused) 
                    return;

                TestUnpausedGUI();
                return;
            }

            if (!m_displayPaused)
                return;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical("box");
            GUILayout.Label("Stats");
            TestStatGUI();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            GUILayout.Label("Items");
            m_playerInventory.OnTestGUI();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            GUILayout.Label("Utilities");
            TestUtilityGUI();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            GUILayout.Label("Player Stat Pipeline");
            m_playerStats.Pipeline.OnTestGUI();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            TestOverlayGUI();
        }

        public void TestUnpausedGUI()
        {
            m_playerStats.Manipulator.ForAllStatEntries((key, value) =>
            {
                float diff = value;
                float refinedValue = m_playerStats.GetStat(key);

                float refinedDiff = refinedValue;
                string colorName;

                if (diff > 0f) colorName = "green";
                else if (diff == 0f) colorName = "white";
                else colorName = "red";

                string valueLabel = utilities.Helpers.Text.Colorize(value.ToString("0"), colorName);

                if (refinedDiff > 0f) colorName = "green";
                else if (refinedDiff == 0f) colorName = "white";
                else colorName = "red";

                string refinedValueLabel = utilities.Helpers.Text.Colorize($" ({refinedValue.ToString("0.00")})", colorName);

                GUILayout.Label(utilities.Helpers.Text.Bold($"{StatSystemHelpers.Text.GetDisplayName(key, true)}: " +
                valueLabel + refinedValueLabel), GUILayout.Width(k_totalStatAreaWidth));
            });
        }

        public void TestOverlayGUI()
        {
            DrawPausedLabel();

            return;

            void DrawPausedLabel()
            {
                if (!Game.Paused) return;

                const string labelText = "<b>Game Paused</b>";
                const float padding = 5f;

                GUIContent labelContent = new GUIContent()
                {
                    text = labelText,
                };

                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    richText = true
                };

                Vector2 labelSizeRaw = style.CalcSize(labelContent);
                Vector2 labelSize = style.CalcScreenSize(labelSizeRaw);

                Rect pausedTextRect = new Rect(Screen.width - labelSize.x - padding, padding,
                    labelSize.x, labelSize.y);

                GUI.Label(pausedTextRect, labelContent);
            }
        }

        public void TestStatGUI()
        {
            GUILayout.BeginVertical();

            m_playerStats.Manipulator.ForAllStatEntries((key, value) =>
            {
                GUILayout.BeginHorizontal();

                //float defaultValue = m_playerStats.DefaultValues[key];
                //float diff = value - defaultValue;
                float diff = value;
                float refinedValue = m_playerStats.GetStat(key);

                float refinedDiff = refinedValue;
                string colorName;

                if (diff > 0f) colorName = "green";
                else if (diff == 0f) colorName = "white";
                else colorName = "red";

                string valueLabel = utilities.Helpers.Text.Colorize(value.ToString("0"), colorName);

                if (refinedDiff > 0f) colorName = "green";
                else if (refinedDiff == 0f) colorName = "white";
                else colorName = "red";

                string refinedValueLabel = utilities.Helpers.Text.Colorize($" ({refinedValue.ToString("0.00")})", colorName);

                GUILayout.Label(utilities.Helpers.Text.Bold($"{StatSystemHelpers.Text.GetDisplayName(key, true)}: " +
                    valueLabel +
                    refinedValueLabel ), GUILayout.Width(k_totalStatAreaWidth));

                if (GUILayout.Button($"-{m_additionButtonAmount}", GUILayout.Width(m_additionButtonWidth)))
                {
                    ChangeModifierValue(key, -m_additionButtonAmount, false);
                }

                if (GUILayout.Button($"+{m_additionButtonAmount}", GUILayout.Width(m_additionButtonWidth)))
                {
                    ChangeModifierValue(key, m_additionButtonAmount, false);
                }

                if (GUILayout.Button($"-%{m_percentageButtonAmount}", GUILayout.Width(m_percentageButtonWidth)))
                {
                    ChangeModifierValue(key, -m_percentageButtonAmount, true);
                }

                if (GUILayout.Button($"+%{m_percentageButtonAmount}", GUILayout.Width(m_percentageButtonWidth)))
                {
                    ChangeModifierValue(key, m_percentageButtonAmount, true);
                }

                GUILayout.EndHorizontal();
            });

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.Label("Settings");

            GUILayout.Label("Addition amount: ");
            m_additionButtonAmount = Mathf.Ceil(GUILayout.HorizontalSlider(m_additionButtonAmount,
                k_minButtonAmount, k_maxAdditionButtonAmount));

            GUILayout.Label("Percentage amount: ");
            m_percentageButtonAmount = Mathf.Ceil(GUILayout.HorizontalSlider(m_percentageButtonAmount,
                k_minButtonAmount, k_maxPercentageButtonAmount));     

            GUILayout.EndVertical();
        }

        public void TestUtilityGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(k_utilityPanelWidth));

            int experienceAmount = Mathf.FloorToInt(m_additionButtonAmount);
            if (GUILayout.Button($"Gain {experienceAmount} Experience"))
            {
                m_playerLevelingLogic.GainExperience(experienceAmount);
            }

            if (GUILayout.Button("Level Up"))
            {
                m_playerLevelingLogic.LevelUp();
            }

            if (GUILayout.Button("End Wave"))
            {
                OnWaveEnd();
            }

            GUILayout.EndVertical();
        }

        void ChangeModifierValue(PlayerStatType key, float amountToAdd, bool percentage)
        {
            if (s_performanceMode)
            {
                Dictionary<PlayerStatType, ModifierObject<PlayerStatType>> m_targetDict =
                    percentage ? m_percentageDict : m_additionalDict;

                float amount =
                    percentage ? m_playerStats.GetStat(key) * amountToAdd / 100f : amountToAdd;

                if (!m_targetDict.ContainsKey(key))
                {
                    if (!percentage) m_targetDict.Add(key, m_playerStats.Manipulator.ModifyIncremental(key, amountToAdd));
                    else m_targetDict.Add(key, m_playerStats.Manipulator.ModifyPercentage(key, amountToAdd));
                }

                else
                {
                    m_targetDict[key].Value += amount;
                    m_playerStats.Manipulator.Refresh(key);
                }
            }

            else
            {
                if (!percentage) m_playerStats.Manipulator.ModifyIncremental(key, amountToAdd);
                else m_playerStats.Manipulator.ModifyPercentage(key, amountToAdd);
            }
        }
    }
}