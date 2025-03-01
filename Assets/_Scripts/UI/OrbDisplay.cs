using com.game.orbsystem;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_iconImage;
        [SerializeField] private GameObject m_outline;
        [SerializeField] private bool m_handleSelfOutline;

        public event Action<SimpleOrb> onPointerEnter;
        public event Action<SimpleOrb> onPointerExit;

        Sprite m_initialSprite;
        SimpleOrb m_target;

        public SimpleOrb Target => m_target;

        private void Awake()
        {
            m_initialSprite = m_iconImage != null ? m_iconImage.sprite : null;
            if (m_outline != null) m_outline.SetActive(false);
        }

        public void Initialize(SimpleOrb orb, OrbInventory inventory)
        {
            m_target = orb;

            if (m_iconImage != null)
            {
                Sprite icon = inventory.GetIcon();
                m_iconImage.sprite = icon != null ? icon : m_initialSprite;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_handleSelfOutline) DoSetOutlineVisibility(true);

            onPointerEnter?.Invoke(m_target);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_handleSelfOutline) DoSetOutlineVisibility(false);

            onPointerExit?.Invoke(m_target);
        }

        public void SetOutlineVisibility(bool visibility)
        {
            if (m_handleSelfOutline) 
                return;

            DoSetOutlineVisibility(visibility);
        }

        void DoSetOutlineVisibility(bool visibility)
        {
            if (m_outline != null)
                m_outline.SetActive(visibility);
        }
    }
}
