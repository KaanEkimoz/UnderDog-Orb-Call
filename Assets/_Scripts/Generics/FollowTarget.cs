using com.absence.attributes;
using UnityEngine;

namespace com.game.generics
{
    public class FollowTarget : MonoBehaviour
    {
        public enum FollowingType
        {
            OnceAtStart,
            UpdateConstantly
        }

        [SerializeField] private FollowingType m_followingType = FollowingType.UpdateConstantly;
        [SerializeField] private Transform m_target;
        [SerializeField] private bool m_keepStartingOffset;
        [SerializeField, HideIf(nameof(m_keepStartingOffset))] private Vector3 m_offset;

        Vector3 m_realOffset;
        bool m_update = false;

        public Transform Target
        {
            get
            {
                return m_target;
            }

            set
            {
                m_target = value;
            }
        }

        private void Start()
        {
            RecalculateOffset();
            RefreshPosition();

            m_update = m_followingType == FollowingType.UpdateConstantly;
        }

        private void Update()
        {
            if (!m_update)
                return;

            RefreshPosition();
        }

        public void RecalculateOffset()
        {
            if (m_keepStartingOffset)
            {
                if (m_target == null)
                    m_realOffset = Vector3.zero;
                else
                    m_realOffset = transform.position - m_target.position;

                return;
            }

            m_realOffset = m_offset;
        }

        public void RefreshPosition()
        {
            transform.position = m_target.position + m_realOffset;
        }
    }
}
