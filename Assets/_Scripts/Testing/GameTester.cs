using com.absence.utilities;
using com.game.player;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.testing
{
    public class GameTester : MonoBehaviour
    {
#if UNITY_EDITOR
        private static readonly bool s_performanceMode = false;

        private static readonly float s_totalButtonAreaWidth = 150f;

        private static readonly float s_initialAdditionButtonAmount = 1f;
        private static readonly float s_initialPercentageButtonAmount = 15f;

        private static readonly float s_minButtonAmount = 1f;

        private static readonly float s_maxPercentageButtonAmount = 50f;
        private static readonly float s_maxAdditionButtonAmount = 10f;

        Dictionary<PlayerStatType, ModifierObject<PlayerStatType>> m_additionalDict = new();
        Dictionary<PlayerStatType, ModifierObject<PlayerStatType>> m_percentageDict = new();

        PlayerStats m_playerStats;
        PlayerInventory m_playerInventory;

        float m_additionButtonWidth;
        float m_percentageButtonWidth;
        float m_additionButtonAmount;
        float m_percentageButtonAmount;

        private void Start()
        {
            m_playerStats = Player.Instance.Hub.Stats;
            m_playerInventory = Player.Instance.Hub.Inventory;

            m_additionButtonWidth = s_totalButtonAreaWidth * 2 / 5 / 2;
            m_percentageButtonWidth = s_totalButtonAreaWidth * 3 / 5 / 2;
            m_additionButtonAmount = s_initialAdditionButtonAmount;
            m_percentageButtonAmount = s_initialPercentageButtonAmount;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            TestStatGUI();

            GUILayout.Space(20);

            m_playerInventory.OnTestGUI();

            GUILayout.Space(20);

            TestUtilityGUI();

            GUILayout.EndHorizontal();
        }

        public void TestStatGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            m_playerStats.StatHolder.ForAllStatEntries((key, value) =>
            {
                GUILayout.BeginHorizontal();

                //float defaultValue = m_playerStats.DefaultValues[key];
                //float diff = value - defaultValue;
                float diff = value;
                string colorName;

                if (diff > 0f) colorName = "green";
                else if (diff == 0f) colorName = "white";
                else colorName = "red";

                GUILayout.Label(utilities.Helpers.Text.Bold($"{StatSystemHelpers.Text.GetDisplayName(key, true)}: " +
                    $"{utilities.Helpers.Text.Colorize(value.ToString("0.00"), colorName)}"));

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

            GUILayout.Label("Addition amount: ");
            m_additionButtonAmount = Mathf.Ceil(GUILayout.HorizontalSlider(m_additionButtonAmount,
                s_minButtonAmount, s_maxAdditionButtonAmount));

            GUILayout.Label("Percentage amount: ");
            m_percentageButtonAmount = Mathf.Ceil(GUILayout.HorizontalSlider(m_percentageButtonAmount,
                s_minButtonAmount, s_maxPercentageButtonAmount));

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        public void TestUtilityGUI()
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("Kill enemy"))
            {
                TestEventChannel.ReceiveEnemyKill();
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
                    percentage ? m_playerStats.StatHolder.GetStat(key) * amountToAdd / 100f : amountToAdd;

                if (!m_targetDict.ContainsKey(key))
                {
                    if (!percentage) m_targetDict.Add(key, m_playerStats.StatHolder.ModifyIncremental(key, amountToAdd));
                    else m_targetDict.Add(key, m_playerStats.StatHolder.ModifyPercentage(key, amountToAdd));
                }

                else
                {
                    m_targetDict[key].Value += amount;
                    m_playerStats.StatHolder.Refresh(key);
                }
            }

            else
            {
                if (!percentage) m_playerStats.StatHolder.ModifyIncremental(key, amountToAdd);
                else m_playerStats.StatHolder.ModifyPercentage(key, amountToAdd);
            }
        }
#endif
    }
}