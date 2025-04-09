using UnityEngine;

namespace com.game.enemysystem.ai
{
    public class EnemyAIPolarith : MonoBehaviour, IEnemyAI
    {
        public EnemyAIState State { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public EnemyMovementData MovementData => throw new System.NotImplementedException();
        public bool Enabled
        {
            get
            {
                return false;
            }

            set
            {

            }
        }
        public float Speed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float DefaultSpeed => throw new System.NotImplementedException();

        public void Initialize(Transform target, EnemyMovementData movementData)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize(Transform target)
        {
            throw new System.NotImplementedException();
        }

        public void Refresh(Transform target)
        {
            throw new System.NotImplementedException();
        }
    }
}
