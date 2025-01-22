using com.absence.attributes;
using com.game.player.statsystemextensions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.player
{
    [RequireComponent(typeof(PlayerStatPipeline))]
    public abstract class PlayerStatPipelineComponentBase : MonoBehaviour
    {
        [SerializeField] protected int m_order = 0;
        [Space, SerializeField]
        protected List<PlayerStatPipelineStatEntry> m_statEntries = new();

        Dictionary<PlayerStatType, float> m_coefficientPairs;

        public int Order => m_order;

        public void Initialize()
        {
            RefreshCoefficients();
            Initialize_Internal();
        }

        public float Process(PlayerStatType statType, float rawValue)
        {
            if (!m_coefficientPairs.TryGetValue(statType, out float statCoefficient))
                return rawValue;

            return Process_Internal(statType, statCoefficient, rawValue);
        }

        protected abstract void Initialize_Internal();
        protected abstract float Process_Internal(PlayerStatType statType, float statCoefficient, float rawValue);

        public virtual void OnTestGUI()
        {
        }

        [Button("Validate Entries")]
        public void Validate()
        {
            StringBuilder sb = new("Result of the validation:\n\n");

            bool error = false;
            foreach(PlayerStatType enumValue in Enum.GetValues(typeof(PlayerStatType)))
            {
                List<PlayerStatPipelineStatEntry> entryList = 
                    m_statEntries.FindAll(entry => entry.TargetStat.Equals(enumValue));

                int count = entryList.Count;
                    
                if (count > 1)
                {
                    sb.Append($"There are multiple ({count}) entries of the stat: '{enumValue}' found.\n");
                    error = true;
                }
            }

            if (error)
            {
                Debug.LogWarning(sb.ToString());
                return;
            }

            Debug.LogWarning("No problems found.");
        }

        [Button("Refresh Coefficients")]
        public virtual void RefreshCoefficients()
        {
            if (!Application.isPlaying) return;

            m_coefficientPairs = new();
            m_statEntries.ForEach(entry =>
            {
                if (!m_coefficientPairs.ContainsKey(entry.TargetStat))
                    m_coefficientPairs.Add(entry.TargetStat, entry.Coefficient);
            });
        }
    }
}
