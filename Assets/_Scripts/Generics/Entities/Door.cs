using com.absence.attributes;
using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.game.generics.entities
{
    public class Door : MonoBehaviour
    {
        const float k_gizmoSphereRadius = 0.4f;

        [SerializeField] private bool m_switchOnStart = false;
        [SerializeField, Required] private Transform m_root;
        [SerializeField] private Ease m_animationEase;
        [SerializeField] private float m_animationDuration;
        [SerializeField] private Vector3 m_openedPositionShift;
        [SerializeField, Readonly] private bool m_open;

        Vector3 m_closedPosition;
        Tween m_tween;

        private void Awake()
        {
            if (m_open) 
                m_closedPosition = m_root.transform.position - m_openedPositionShift;
            else
                m_closedPosition = m_root.position;

            if (m_switchOnStart)
                Switch();
        }

        public void Switch()
        {
            if (m_open)
                Close();
            else
                Open();
        }

        public void Open()
        {
            if (m_open)
                return;

            m_tween?.Kill();

            m_open = true;

            SetupTweenFor(m_openedPositionShift);
        }

        void SetupTweenFor(Vector3 shift)
        {
            Vector3 endPoint = m_closedPosition + shift;

            //if (!m_self)
            //    endPoint += transform.position;

            m_tween = m_root.DOMove(endPoint, m_animationDuration)
                .SetEase(m_animationEase)
                .OnKill(OnTweenEnds)
                .OnComplete(OnTweenEnds);
        }

        private void OnTweenEnds()
        {
            m_tween = null;
        }

        public void Close()
        {
            if (!m_open)
                return;

            m_tween?.Kill();

            m_open = false;

            SetupTweenFor(Vector3.zero);
        }

        [Button("Open")]
        void OpenInstantly()
        {
            if (m_root == null)
                return;

            if (m_open)
                return;

            m_closedPosition = m_root.transform.position;

            Vector3 position = m_closedPosition;
            position += m_openedPositionShift;

            m_root.transform.position = position;
            m_open = true;

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);

            EditorUtility.SetDirty(m_root);
            AssetDatabase.SaveAssetIfDirty(m_root);
#endif
        }

        [Button("Close")]
        void CloseInstantly()
        {
            if (m_root == null)
                return;

            if (m_root == transform)
                return;

            if (!m_open)
                return;

            m_root.transform.position = m_closedPosition;
            m_open = false;

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);

            EditorUtility.SetDirty(m_root);
            AssetDatabase.SaveAssetIfDirty(m_root);
#endif
        }

        private void OnDrawGizmosSelected()
        {
            if (m_root == null)
                return;

            Vector3 startPoint = m_root.transform.position;
            Vector3 endPoint = startPoint + m_openedPositionShift;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint, endPoint);
            Gizmos.DrawSphere(startPoint, k_gizmoSphereRadius);
            Gizmos.DrawSphere(endPoint, k_gizmoSphereRadius);
        }
    }
}
