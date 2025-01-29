using com.absence.attributes;
using DG.Tweening;
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

        [SerializeField, Readonly] private DisplayState m_state = DisplayState.Ready;
        [SerializeField] private Image m_image;
        [SerializeField] private Transform m_swayPivot;
        [SerializeField] private float m_swayMagnitude;

        bool m_isRotating = false;
        bool m_isThrown = false;

        public bool IsThrown => m_isThrown;

        private void Start()
        {
            //transform.DOMove(m_swayPivot);
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

        public void SetState(DisplayState newState)
        {
            m_state = newState;
            switch (newState)
            {
                case DisplayState.Ready:
                    m_image.color = Color.cyan;
                    break;
                case DisplayState.Thrown:
                    m_image.color = Color.red;
                    break;
                case DisplayState.Recalling:
                    m_image.color = Color.magenta;
                    break;
                default:
                    break;
            }
        }
    }
}
