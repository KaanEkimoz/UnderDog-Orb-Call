using com.game.player;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbDisplayGP : MonoBehaviour
    {
        public enum DisplayState
        {
            Ready,
            Thrown,
            Recalling,
        }

        [SerializeField] private Image m_image;

        public Image Image => m_image;

        Sprite m_initialSprite;
        bool m_isRotating = false;
        SimpleOrb m_orb;
        PlayerOrbContainer m_container;

        private void Start()
        {
            SetDisplay(OrbState.OnEllipse);
        }

        public void Initialize(SimpleOrb orb, PlayerOrbContainer container)
        {
            if (orb == null)
                throw new System.Exception("You can't initialize a gameplay orb display with a null orb reference.");

            m_initialSprite = m_image.sprite;

            if (m_orb != null && m_orb != orb)
            {
                m_orb.OnStateChanged -= OnOrbStateChanged;
            }

            m_orb = orb;
            m_orb.OnStateChanged += OnOrbStateChanged;
            m_container = container;

            OnOrbStateChanged(m_orb.currentState);
        }

        public void Refresh()
        {
            if (m_orb == null)
                return;

            Sprite iconFound = m_container.OrbInventoryEntries[m_orb].GetIcon();
            m_image.sprite = iconFound != null ? iconFound : m_initialSprite;
        }

        private void OnOrbStateChanged(OrbState state)
        {
            SetDisplay(state);
        }

        private void Update()
        {
            if (m_isRotating) 
                transform.up = Vector2.up;
        }

        public void SetRotating(bool rotating)
        {
            m_isRotating = rotating;
        }

        private void OnDestroy()
        {
            if (m_orb != null)
                m_orb.OnStateChanged -= OnOrbStateChanged;
        }

        void SetDisplay(OrbState state)
        {
            switch (state)
            {
                case OrbState.OnEllipse:
                    m_image.color = Color.cyan;
                    break;
                case OrbState.Throwing:
                    m_image.color = Color.red;
                    break;
                case OrbState.Sticked:
                    m_image.color = Color.red;
                    break;
                case OrbState.Returning:
                    m_image.color = Color.magenta;
                    break;
                default:
                    break;
            }
        }
    }
}
