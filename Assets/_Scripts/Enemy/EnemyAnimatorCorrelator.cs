using System.Collections.Generic;
using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemyAnimatorCorrelator : MonoBehaviour
    {
        [SerializeField] private EnemyAnimator m_animator;
        [SerializeField] private EnemyInstance m_instance;
        [SerializeField] private Enemy m_enemy;

        private void Start()
        {
            m_animator.SetStateByHash(EnemyAnimator.IDLE_HASH);
        }

        private void Update()
        {
            bool isMoving = m_instance.NavMeshAgent.speed > 0f;
            bool isAttacking = m_enemy.IsAttacking;

            bool animatorAttacking = m_animator.GetHash().Equals(EnemyAnimator.ATTACK_HASH);
            bool animatorWalking = m_animator.GetHash().Equals(EnemyAnimator.WALK_HASH);
            bool animatorIdle = m_animator.GetHash().Equals(EnemyAnimator.IDLE_HASH);

            if (animatorAttacking)
            {
                if (isAttacking)
                    return;
                else
                {
                    m_animator.SetStateByHash(EnemyAnimator.IDLE_HASH);
                    return;
                }
            }

            if (isAttacking && (!animatorAttacking))
            {
                m_animator.SetStateByHash(EnemyAnimator.ATTACK_HASH);
                return;
            }

            if (isMoving && (!animatorWalking))
            {
                m_animator.SetStateByHash(EnemyAnimator.WALK_HASH);
            }

            else if ((!isMoving) && (!animatorIdle))
            {
                m_animator.SetStateByHash(EnemyAnimator.IDLE_HASH);
            }
        }
    }
}
