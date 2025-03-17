using com.game.events;
using System;
using UnityEngine;

namespace com.game.player
{
    public class PlayerParanoiaWaveDurationEffect : MonoBehaviour
    {
        [SerializeField] private PlayerParanoiaLogic m_target;
        [SerializeField] private float m_strength;
        [SerializeField] private float m_startTimeInSeconds;

        float m_timer = float.NaN;
        bool m_enabled = false;
        bool m_initialized = false;

        private void Start()
        {
            GameEventChannel.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            m_initialized = true;

            if (state != GameState.InWave)
            {
                m_enabled = false;
                m_timer = float.NaN;
                return;
            }

            m_timer = 0f;
            m_enabled = true;
        }

        private void Update()
        {
            if (!m_initialized)
                OnGameStateChanged(GameManager.Instance.State);

            if (!m_enabled)
                return;

            if (m_timer >= m_startTimeInSeconds)
            {
                m_target.Increase(m_strength);
            }

            else
            {
                m_timer += Time.deltaTime;
            }
        }
    }
}
