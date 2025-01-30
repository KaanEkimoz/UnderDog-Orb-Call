using com.absence.attributes;
using DG.Tweening;
using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game
{
    public class HealthBar : MonoBehaviour
    {
        public enum HealthBarTextMode
        {
            Seperate,
            Combined,
        }

        [SerializeField] private GameObject m_damageableObject;
        [SerializeField] private Image m_background;
        [SerializeField] private Image m_foreground;

        [Space]

        [SerializeField] 
        private bool m_useEffect = false;

        [SerializeField, ShowIf(nameof(m_useEffect))]
        private Image m_effectLayer;

        [SerializeField, ShowIf(nameof(m_useEffect))]
        private Ease m_effectEase;

        [SerializeField, ShowIf(nameof(m_useEffect)), Min(0f)]
        private float m_effectDuration;

        [SerializeField, ShowIf(nameof(m_useEffect)), Min(0f)]
        private float m_effectDelay;

        [Space]

        [SerializeField]
        private bool m_useText = false;

        [SerializeField, ShowIf(nameof(m_useText))]
        private HealthBarTextMode m_textMode = HealthBarTextMode.Combined;

        [SerializeField, ShowIf(nameof(m_useText)), HideIf(nameof(m_textMode), HealthBarTextMode.Seperate)]
        private TMP_Text m_healthText;

        [SerializeField, ShowIf(nameof(m_useText)), HideIf(nameof(m_textMode), HealthBarTextMode.Seperate)]
        private string m_combinerString = "/";

        [SerializeField, ShowIf(nameof(m_useText)), HideIf(nameof(m_textMode), HealthBarTextMode.Combined)]
        private TMP_Text m_maxHealthText;

        [SerializeField, ShowIf(nameof(m_useText)), HideIf(nameof(m_textMode), HealthBarTextMode.Combined)]
        private TMP_Text m_currentHealthText;

        IDamageable m_target;
        Tween m_effectTween;
        float m_currentHealthRatio;
        float m_currentHealth;
        float m_maxHealth;

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

            Refresh();
        }

        private void OnDie()
        {
            // prolly nothing.
        }

        private void OnTakeDamage(float amount)
        {
            Refresh();
        }

        void CacheVariables()
        {
            m_currentHealth = m_target.Health;
            m_maxHealth = m_target.MaxHealth;
            m_currentHealthRatio = m_currentHealth / m_maxHealth;
        }

        public void Refresh()
        {
            CacheVariables();
            UpdateBar();
            UpdateEffect();
            UpdateText();
        }

        public void UpdateBar()
        {
            m_foreground.fillAmount = m_currentHealthRatio;
        }

        public void UpdateEffect()
        {
            if (!m_useEffect)
                return;

            if (m_effectTween != null)
                m_effectTween.Kill();
;
            m_effectTween = m_effectLayer.DOFillAmount(m_currentHealthRatio, m_effectDuration)
                .SetEase(m_effectEase)
                .SetDelay(m_effectDelay, false)
                .OnKill(OnTweenComplete)
                .OnComplete(OnTweenComplete);
        }

        public void UpdateText()
        {
            if (!m_useText)
                return;

            switch (m_textMode)
            {
                case HealthBarTextMode.Seperate:
                    UpdateText_Seperate();
                    break;
                case HealthBarTextMode.Combined:
                    UpdateText_Combined();
                    break;
                default:
                    break;
            }

            return;

            void UpdateText_Seperate()
            {
                if (m_currentHealthText != null) m_currentHealthText.text = m_currentHealth.ToString("0");
                if (m_maxHealthText != null) m_maxHealthText.text = m_maxHealth.ToString("0");
            }

            void UpdateText_Combined()
            {
                StringBuilder sb = new(m_currentHealth.ToString("0"));
                sb.Append(m_combinerString);
                sb.Append(m_maxHealth.ToString("0"));

                m_healthText.text = sb.ToString();
            }
        }

        private void OnTweenComplete()
        {
            m_effectTween = null;
        }
    }
}
