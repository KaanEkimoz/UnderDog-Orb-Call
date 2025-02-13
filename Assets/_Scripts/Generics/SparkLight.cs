using com.absence.attributes;
using com.absence.timersystem;
using DG.Tweening;
using UnityEngine;

namespace com.game.generics
{
    public class SparkLight : MonoBehaviour
    {
        [SerializeField] private bool m_startOnAwake;
        [SerializeField] private bool m_destroyOnEnd;
        [SerializeField, Required] private Light m_light;
        [SerializeField] private Ease m_ease;
        [SerializeField, MinMaxSlider(0f, 1f)] private Vector2 m_durationRange;
        [SerializeField, MinMaxSlider(0f, 5f)] private Vector2 m_cooldownRange;
        [SerializeField] private int m_loops;
        [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 m_intensityRange;

        public bool InCooldown => m_inCooldown;

        bool m_inCooldown;
        Tween m_tween;
        Timer m_cooldownTimer;

        private void Awake()
        {
            ForceStop();
            if (m_startOnAwake) Spark();
        }

        public void Spark()
        {
            if (m_tween != null)
                return;

            if (InCooldown)
                return;

            m_light.intensity = m_intensityRange.y;
            m_tween = m_light.DOIntensity(m_intensityRange.x, Random.Range(m_durationRange.x, m_durationRange.y))
                .SetLoops(m_loops, LoopType.Yoyo)
                .SetEase(m_ease)
                .OnComplete(OnTweenComplete)
                .OnKill(OnTweenKill);
        }

        public void ForceStop()
        {
            if (m_tween != null)
                m_tween.Kill();

            if (InCooldown)
                return;

            m_light.intensity = 0f;
        }

        void OnTweenKill()
        {
            m_light.intensity = 0f;
            m_tween = null;
        }

        void OnTweenComplete()
        {
            m_light.intensity = 0f;
            m_tween = null;

            if (m_destroyOnEnd)
            {
                Destroy(gameObject);
                return;
            }

            m_inCooldown = true;
            m_cooldownTimer = Timer.Create(Random.Range(m_cooldownRange.x, m_cooldownRange.y), null, OnTimerComplete);
            m_cooldownTimer.Start();
        }

        private void OnTimerComplete(TimerCompletionContext context)
        {
            m_cooldownTimer = null;
            m_inCooldown = false;
        }
    }
}
