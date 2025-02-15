using com.game.abilitysystem;
using com.game.abilitysystem.ui;
using com.game.player;
using com.game.player.statsystemextensions;
using com.game.statsystem;
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
        [SerializeField] private AbilityDisplayGP m_parryDisplay;
        [SerializeField] private Parry m_parry;

        Dictionary<PlayerStatType, ModifierObject<PlayerStatType>> m_additionalDict = new();
        Dictionary<PlayerStatType, ModifierObject<PlayerStatType>> m_percentageDict = new();

        PlayerStats m_playerStats;
        PlayerInventory m_playerInventory;
        PlayerLevelingLogic m_playerLevelingLogic;

        float m_additionButtonWidth;
        float m_percentageButtonWidth;
        float m_additionButtonAmount;
        float m_percentageButtonAmount;

        int m_enemiesKilled;

        private void Start()
        {
            m_enemiesKilled = 0;

            m_playerStats = Player.Instance.Hub.Stats;
            m_playerInventory = Player.Instance.Hub.Inventory;
            m_playerLevelingLogic = Player.Instance.Hub.Leveling;

            m_additionButtonWidth = k_totalButtonAreaWidth * 2 / 5 / 2;
            m_percentageButtonWidth = k_totalButtonAreaWidth * 3 / 5 / 2;

            m_additionButtonAmount = k_initialAdditionButtonAmount;
            m_percentageButtonAmount = k_initialPercentageButtonAmount;

            m_parryDisplay.Initialize(m_parry);
        }

        private void Update()
        {
            if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

            if (Game.Paused) Game.Resume();
            else Game.Pause();
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
            if (!Game.Paused)
            {
                if (!m_displayUnpaused) 
                    return;

                TestUnpausedGUI();
                return;
            }

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

            //GUI.enabled = false;
            //if (GUILayout.Button("Kill enemy"))
            //{
            //    TestEventChannel.ReceiveEnemyKill();
            //    m_enemiesKilled++;
            //}
            //GUI.enabled = true;

            //GUILayout.Label($"Enemies Killed: {m_enemiesKilled}");

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