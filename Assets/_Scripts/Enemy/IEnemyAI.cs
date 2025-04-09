using UnityEngine;

namespace com.game.enemysystem.ai
{
    public interface IEnemyAI
    {
        EnemyAIState State { get; set; }
        EnemyMovementData MovementData { get; }
        bool Enabled { get; set; }
        float Speed { get; set; }
        float DefaultSpeed { get; }

        void Initialize(Transform target);
        void Refresh(Transform target);
    }
}
