using com.game.orbsystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_iconImage;
        [SerializeField] private RectTransform m_descriptionPanel;
        [SerializeField] private GameObject m_outline;
        [SerializeField] private TMP_Text m_descriptionText;

        public RectTransform DescriptionPanel => m_descriptionPanel;

        bool m_showDescriptionOnlyOnHover;
        Sprite m_initialSprite;

        private void Awake()
        {
            m_showDescriptionOnlyOnHover = InternalSettings.UI.ORB_DESCRIPTIONS_ONLY_ON_HOVER;
            m_initialSprite = m_iconImage != null ? m_iconImage.sprite : null;
            SetDescriptionPanelVisibility(!m_showDescriptionOnlyOnHover);
            if (m_outline != null) m_outline.SetActive(false);
        }

        public void Initialize(SimpleOrb orb, OrbInventory inventory)
        {
            if (m_descriptionText != null)
                m_descriptionText.text = inventory.GenerateDescription();

            if (m_iconImage != null)
            {
                Sprite icon = inventory.GetIcon();
                m_iconImage.sprite = icon != null ? icon : m_initialSprite;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_showDescriptionOnlyOnHover)
                ShowDescriptionPanel();

            if (m_outline != null) 
                m_outline.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_showDescriptionOnlyOnHover)
                HideDescriptionPanel();

            if (m_outline != null) 
                m_outline.SetActive(false);
        }

        void SetDescriptionPanelVisibility(bool visibility)
        {
            if (m_descriptionPanel == null)
                return;

            m_descriptionPanel.gameObject.SetActive(visibility);
        }

        void ShowDescriptionPanel()
        {
            SetDescriptionPanelVisibility(true);
        }

        void HideDescriptionPanel()
        {
            SetDescriptionPanelVisibility(false);
        }
    }
}
