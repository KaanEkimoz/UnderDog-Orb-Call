using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.abilitysystem.ui
{
    public class AbilityDisplayGP : MonoBehaviour
    {
        public enum TestAbilityType
        {
            None,
            Dash,
        }

        public const bool COOLDOWN_FILL = false;

        [SerializeField] private ThirdPersonController m_playerController;
        [SerializeField] private TestAbilityType m_testAbilityType = TestAbilityType.Dash;
        [SerializeField] private CanvasGroup m_graphic;
        [SerializeField] private GameObject m_cooldownPanel;
        [SerializeField] private Image m_semiCooldownFillImage;
        [SerializeField] private Image m_cooldownFillImage;
        [SerializeField] private TMP_Text m_useCountText;
        [SerializeField] private TMP_Text m_cooldownText;

        bool m_readyToUse;
        float m_totalCooldown;
        float m_totalDuration;
        int m_totalUseCount;

        static string CreateCooldownString(float seconds)
        {
            if (seconds < 60f)
                return seconds.ToString("0");

            string mins = (seconds / 60f).ToString("0");
            string secs = (seconds % 60f).ToString("0");

            StringBuilder sb = new(mins);
            sb.Append(".");
            sb.Append(secs);

            return sb.ToString();
        }

        private void Start()
        {
            if (m_testAbilityType == TestAbilityType.None)
            {
                m_graphic.alpha = 0f;
                enabled = false;
            }

            m_totalCooldown = m_playerController.DashCooldown;
            m_totalDuration = m_playerController.DashDuration;
            m_totalUseCount = m_playerController.MaxDashCount;
        }

        private void Update()
        {
            float timerValue = m_playerController.DashCooldownTimer;

            if (m_useCountText != null)
                m_useCountText.text = m_playerController.DashCount.ToString();

            if (!m_readyToUse)
            {
                UpdateCooldown();
                return;
            }

            if (m_semiCooldownFillImage) m_semiCooldownFillImage.fillAmount = timerValue / m_totalCooldown;

            if (m_playerController.DashCount == 0 && timerValue > 0f)
            {
                SetReady(false);
            }
        }

        void UpdateCooldown()
        {
            float timerValue = m_playerController.DashCooldownTimer;

            if (timerValue <= 0f)
            {
                SetReady(true);
                return;
            }

            if (COOLDOWN_FILL && m_cooldownFillImage != null) 
                m_cooldownFillImage.fillAmount = timerValue / m_totalCooldown;

            if (m_cooldownText != null)
                m_cooldownText.text = CreateCooldownString(timerValue);
        }

        public void SetReady(bool ready)
        {
            m_readyToUse = ready;
            m_cooldownPanel.SetActive(!ready);
            m_semiCooldownFillImage.gameObject.SetActive(ready);
            m_useCountText?.gameObject.SetActive(ready);
        }
    }
}
