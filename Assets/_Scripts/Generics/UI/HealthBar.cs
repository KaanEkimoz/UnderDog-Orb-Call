using com.absence.attributes;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace com.game
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject m_damageableObject;
        [SerializeField] private Image m_background;
        [SerializeField] private Image m_foreground;

        [SerializeField] private bool m_useEffect = false;

        [SerializeField, ShowIf(nameof(m_useEffect))]
        private Image m_effectLayer;

        [SerializeField, ShowIf(nameof(m_useEffect))]
        private Ease m_effectEase;

        [SerializeField, Min(0f), ShowIf(nameof(m_useEffect))]
        private float m_effectDuration;

        [SerializeField, Min(0f), ShowIf(nameof(m_useEffect))]
        private float m_effectDelay;

        IDamageable m_target;
        Tween m_effectTween;

        private void Start()
        {
            try
            {
                m_target = m_damageableObject.GetComponent<IDamageable>();
            } 

            catch (Exception e)
            {
                Debug.LogException(e);
                this.enabled = false;
                return;
            }

            m_target.OnTakeDamage += OnTakeDamage;
            m_target.OnDie += OnDie;
        }

        private void OnDie()
        {
            // prolly nothing.
        }

        private void OnTakeDamage(float amount)
        {
            float newFillAmount = m_target.Health / m_target.MaxHealth;
            m_foreground.fillAmount = newFillAmount;

            if (!m_useEffect)
                return;

            if (m_effectTween != null)
                m_effectTween.Kill();

            m_effectTween = m_effectLayer.DOFillAmount(newFillAmount, m_effectDuration)
                .SetEase(m_effectEase)
                .SetDelay(m_effectDelay, false)
                .OnKill(OnTweenComplete)
                .OnComplete(OnTweenComplete);
        }

        private void OnTweenComplete()
        {
            m_effectTween = null;
        }
    }
}
