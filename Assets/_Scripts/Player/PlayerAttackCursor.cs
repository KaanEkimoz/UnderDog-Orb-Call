using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace com.game.player
{
    public class PlayerAttackCursor : MonoBehaviour
    {
        [SerializeField] private LayerMask m_mask;
        [SerializeField] private Image m_image;
        [SerializeField] private Canvas m_canvas;

        Camera m_camera;

        private void Start()
        {
            m_camera = Camera.main;

            m_canvas.worldCamera = m_camera;
            UpdateAlignment();
        }

        private void Update()
        {
            if (Game.Paused)
            {
                m_image.color = Color.white;
                return;
            }

            UpdatePosition();
            UpdateColor();
        }

        private void UpdatePosition()
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            mousePosition.z = m_camera.nearClipPlane;
            
            Vector3 mouseWorldPosition = m_camera.ScreenToWorldPoint(mousePosition);
            m_image.transform.position = mouseWorldPosition;
        }

        void UpdateColor()
        {
            if (Game.Paused)
            {
                m_image.color = Color.white;
                return;
            }

            var ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, m_mask))
            {
                m_image.color = Color.red;
            }

            else
            {
                m_image.color = Color.white;
            }
        }

        void UpdateAlignment()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
