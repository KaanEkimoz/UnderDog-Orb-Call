using com.absence.utilities;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class ItemDisplay : MonoBehaviour, IDisplay<ItemObject>, IDisplay<ItemProfileBase>
    {
        [SerializeField] private Image m_iconImage;
        [SerializeField] private TMP_Text m_nameText;
        [SerializeField] private TMP_Text m_typeText;
        [SerializeField] private TMP_Text m_descriptionText;
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private Button m_buyButton;
        [HideInInspector, SerializeField] private TMP_Text m_buyButtonText;

        public CanvasGroup CanvasGroup => m_canvasGroup;

        Func<bool> m_buyButtonInteractability = () => true;

        public void Initialize(ItemObject instance)
        {
            Refresh(instance, instance.Profile);
        }

        public void Initialize(ItemProfileBase profile)
        {
            Refresh(null, profile);
        }

        public void Refresh()
        {
            if (m_buyButton != null) 
                m_buyButton.interactable = m_buyButtonInteractability.Invoke();
        }

        public void SetBuyButtonText(string text, bool richText = false)
        {
            if (m_buyButton == null)
                return;

            m_buyButtonText.richText = richText;
            m_buyButtonText.text = text;
        }

        public void SetupBuyButton(Action onClick, Func<bool> interactability = null)
        {
            if (m_buyButton == null)
                return;

            if (interactability != null) m_buyButtonInteractability = interactability;
            else m_buyButtonInteractability = () => true;

            m_buyButton.onClick.AddListener(() => onClick?.Invoke());

            Refresh();
        }

        void Refresh(ItemObject instance, ItemProfileBase profile)
        {
            if (profile == null)
                return;

            if (profile.Icon != null) m_iconImage.sprite = profile.Icon;
            if (m_nameText != null) m_nameText.text = profile.DisplayName;
            if (m_typeText != null) m_typeText.text = profile.TypeName ?? Helpers.SplitCamelCase(profile.GetType().Name, " ");

            if (instance != null) m_descriptionText.text = ItemSystemHelpers.Text.GenerateDescription(instance, false);
            else m_descriptionText.text = ItemSystemHelpers.Text.GenerateDescription(profile, false);
        }

        private void OnValidate()
        {
            if (m_buyButton == null) m_buyButtonText = null;
            else m_buyButtonText = m_buyButton.GetComponentInChildren<TMP_Text>();
        }
    }
}
