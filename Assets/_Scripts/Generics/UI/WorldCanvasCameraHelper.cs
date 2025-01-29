using com.absence.attributes;
using UnityEngine;

namespace com.game
{
    public class WorldCanvasCameraHelper : MonoBehaviour
    {
        [SerializeField, Readonly]
        private Canvas m_canvas;

        Camera m_camera;

        private void Start()
        {
            m_camera = Camera.main;
            m_canvas.worldCamera = m_camera;
        }

        private void Update()
        {
            transform.forward = m_camera.transform.forward;
        }

        private void Reset()
        {
            m_canvas = GetComponent<Canvas>();
        }
    }
}
