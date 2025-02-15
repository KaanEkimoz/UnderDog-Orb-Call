using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    public class PlayerLevelingLogic : MonoBehaviour
    {
        [SerializeField] private List<int> m_placeholder = new();

        public int CurrentLevel => m_currentLevel;
        public int CurrentExperience => m_currentExperience;
        public int ExperienceNeededForNextLevel => m_targetExperience;
        public float CurrentExperienceRatio => m_currentExperience / m_targetExperience;

        public event Action<PlayerLevelingLogic> OnLevelUp;
        public event Action<int> OnGainExperience;

        int m_currentLevel;
        int m_currentExperience;
        int m_targetExperience;

        private void Awake()
        {
            m_currentLevel = 1;
            m_targetExperience = m_placeholder[0];
        }

        public bool GainExperience(int amount)
        {
            if (amount <= 0)
                return false;

            m_currentExperience += amount;

            int m_levelsGained = 0;
            while (m_currentExperience >= m_targetExperience)
            {
                m_currentExperience -= m_targetExperience;
                m_levelsGained++;
            }

            OnGainExperience?.Invoke(amount);
            LevelUp(m_levelsGained);

            return m_levelsGained > 0;
        }

        public void LevelUp(int amount)
        {
            if (amount <= 0)
                return;

            m_currentLevel += amount;
            m_targetExperience = m_placeholder[m_currentLevel - 1];
            OnLevelUp?.Invoke(this);
        }

        public void LevelUp()
        {
            LevelUp(1);
        }
    }
}
