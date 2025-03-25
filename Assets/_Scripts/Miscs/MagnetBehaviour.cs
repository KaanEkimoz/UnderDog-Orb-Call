using UnityEngine;

namespace com.game.miscs
{
    public class MagnetBehaviour : MonoBehaviour
    {
        [SerializeField] private LayerMask m_layerMask;
        [SerializeField] private ForceMode m_forceMode;
        [SerializeField] private float m_maxAffectionRadius;
        [SerializeField] private float m_forceMagnitude;

        private void FixedUpdate()
        {
            var found = Physics.OverlapSphere(transform.position, m_maxAffectionRadius, m_layerMask);

            foreach (Collider collider in found)
            {
                if (!collider.attachedRigidbody.TryGetComponent(out IMagnetable magnetable))
                    continue;

                if (!magnetable.IsMagnetable)
                    continue;

                Rigidbody target = collider.attachedRigidbody;

                Vector3 rawDirection = transform.position - target.transform.position;
                rawDirection.y = 0f;
                Vector3 forceDirection = (rawDirection).normalized;
                float multiplier = 1 / Mathf.Pow(Vector2.Distance(transform.position, target.transform.position) + magnetable.MagnetResistance, 2);
                target.AddForce(forceDirection * m_forceMagnitude * multiplier, m_forceMode);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, m_maxAffectionRadius);
        }
    }
}
