using com.absence.attributes;
using UnityEngine;
using UnityEngine.AI;

namespace com.game.enemysystem.ai
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAINavMeshAgent : MonoBehaviour, IEnemyAI
    {
        public EnemyAIState State
        {
            get
            {
                return m_state;
            }

            set
            {
                m_state = value;
                Refresh(m_target);
            }
        }
        public EnemyMovementData MovementData
        {
            get
            {
                return m_movementData;
            }
        }
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }

            set
            {
                m_enabled = value;
                m_navMeshAgent.enabled = value;
            }
        }
        public float Speed
        {
            get => m_navMeshAgent.speed;
            set => m_navMeshAgent.speed = value;
        }
        public float DefaultSpeed => m_defaultSpeed;

        [SerializeField, Readonly] private EnemyAIState m_state;
        [SerializeField, Readonly] private NavMeshAgent m_navMeshAgent;
        [SerializeField] private EnemyMovementDataNavMeshAgent m_movementData;

        float m_defaultSpeed;
        bool m_initialized;
        bool m_enabled = true;
        Transform m_target;

        private void Update()
        {
            if (!m_initialized)
                return;

            if (!Enabled)
                return;

            if (!m_navMeshAgent.hasPath) 
                return;

            RotateTowardsTarget();
        }

        public void Initialize(Transform target)
        {
            m_defaultSpeed = m_movementData.speed;
            m_navMeshAgent.updateRotation = false;

            Speed = m_movementData.speed;
            m_navMeshAgent.angularSpeed = m_movementData.angularSpeed;
            m_navMeshAgent.acceleration = m_movementData.acceleration;
            m_navMeshAgent.stoppingDistance = m_movementData.stoppingDistance;

            Refresh(target);

            m_initialized = true;
        }

        public void Refresh(Transform target)
        {
            m_target = target;
            m_navMeshAgent.SetDestination(target.position);
        }

        void RotateTowardsTarget()
        {
            Vector3 direction = (m_navMeshAgent.steeringTarget - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * m_navMeshAgent.angularSpeed);
            }
        }

        private void Reset()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }
}
