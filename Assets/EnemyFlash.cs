using com.absence.attributes;
using com.game.generics;
using System;
using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemyFlash : MonoBehaviour, ISpark
    {
        public enum Phase
        {
            [InspectorName("Idle (Not Active)")] Idle,
            Increasing,
            Decreasing,
        }

        public static readonly string DefaultEmissionProperty = "_Emission";

        [Header("Emission Settings")]
        [SerializeField, Readonly] private Phase m_flashPhase;
        [SerializeField] private Renderer m_enemyRenderer;
        [SerializeField] private string m_emissionProperty = DefaultEmissionProperty;
        [SerializeField] private float m_maxEmissionValue = 2f;
        [SerializeField] private float m_flashDuration = 0.5f;

        [Header("Timer Settings")]
        [SerializeField] private float m_minTime = 2f;
        [SerializeField] private float m_maxTime = 5f;

        public event Action OnEnd;

        Material m_material;
        float m_timer;
        bool m_isFlashing;
        float m_flashTimer;

        private void Start()
        {
            m_material = m_enemyRenderer.material;
            SetEmission(0f);

            m_timer = GetRandomTime();
        }

        private void Update()
        {
            if (!m_isFlashing)
            {
                if (m_timer > 0f) m_timer -= Time.deltaTime;
                else m_timer = 0f;

                return;
            }

            HandleFlash();
        }

        public void Spark()
        {
            if (m_isFlashing)
                return;

            if (m_timer > 0f)
                return;

            StartFlash();
        }

        public void ForceStop()
        {
            Stop();
        }

        private void StartFlash()
        {
            m_isFlashing = true;
            m_flashTimer = 0f;
            m_flashPhase = Phase.Increasing; // Start increasing
        }

        private void HandleFlash()
        {
            m_flashTimer += Time.deltaTime;
            float t = m_flashTimer / (m_flashDuration / 2f);

            if (m_flashPhase == Phase.Increasing) // Increasing
            {
                SetEmission(Mathf.Lerp(0, m_maxEmissionValue, t));
                if (m_flashTimer >= m_flashDuration / 2f)
                {
                    m_flashPhase = Phase.Decreasing;
                    m_flashTimer = 0f;
                }
            }
            else if (m_flashPhase == Phase.Decreasing) // Decreasing
            {
                SetEmission(Mathf.Lerp(m_maxEmissionValue, 0, t));
                if (m_flashTimer >= m_flashDuration / 2f)
                {
                    Stop();
                }
            }
        }

        void Stop()
        {
            if (!m_isFlashing)
                return;

            m_isFlashing = false;
            SetEmission(0f);

            m_timer = GetRandomTime();

            OnEnd?.Invoke();
        }

        void SetEmission(float value)
        {
            m_material.SetFloat(m_emissionProperty, value);
        }

        private float GetRandomTime()
        {
            return UnityEngine.Random.Range(m_minTime, m_maxTime);
        }
    }
}
