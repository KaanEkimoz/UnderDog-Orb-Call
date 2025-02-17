using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class ItemDisplay : MonoBehaviour
    {
        [SerializeField] private Image m_iconImage;
        [SerializeField] private TMP_Text m_nameText;
        [SerializeField] private TMP_Text m_descriptionText;

        ItemObject m_instance;
        ItemProfileBase m_profile;

        public void Initialize(ItemObject instance)
        {
            m_instance = instance;
            m_profile = instance.Profile;
            Refresh();
        }

        public void Initialize(ItemProfileBase profile)
        {
            m_instance = null;
            m_profile = profile;
            Refresh();
        }

        public void Refresh()
        {
            if (m_profile == null)
                return;

            m_iconImage.sprite = m_profile.Icon;
            m_nameText.text = m_profile.DisplayName;

            if (m_instance != null) m_descriptionText.text = ItemSystemHelpers.Text.GenerateDescription(m_instance, false);
            else m_descriptionText.text = ItemSystemHelpers.Text.GenerateDescription(m_profile, false);
        }
    }
}
