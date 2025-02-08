using UnityEngine;
using UnityEngine.UI;

namespace com.game.orbsystem.ui
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
        [SerializeField] private Transform m_swayPivot;
        [SerializeField] private float m_swayMagnitude;

        bool m_isRotating = false;
        SimpleOrb m_orb;

        private void Start()
        {
            SetDisplay(OrbState.OnEllipse);
        }

        public void Initialize(SimpleOrb orb)
        {
            if (m_orb != null && m_orb != orb)
            {
                m_orb.OnStateChanged -= OnOrbStateChanged;
            }

            m_orb = orb;
            m_orb.OnStateChanged += OnOrbStateChanged;
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
