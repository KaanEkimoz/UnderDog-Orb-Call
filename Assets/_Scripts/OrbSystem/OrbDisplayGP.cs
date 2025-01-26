using UnityEngine;
using UnityEngine.UI;

namespace com.game.orbsystem.ui
{
    public class OrbDisplayGP : MonoBehaviour
    {
        [SerializeField] private GameObject m_selectionBorder;
        [SerializeField] private Image m_image;

        bool m_isRotating = false;
        bool m_isThrown = false;

        public bool IsThrown => m_isThrown;

        private void Update()
        {
            if (m_isRotating) 
                transform.up = Vector2.up;
        }

        public void SetRotating(bool rotating)
        {
            m_isRotating = rotating;
        }

        public void SetSelected(bool selected)
        {
            m_selectionBorder.SetActive(selected);
        }

        public void SetThrown(bool thrown)
        {
            m_isThrown = thrown;

            m_image.color = thrown ? 
                Color.red : Color.white;
        }
    }
}
