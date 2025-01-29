using System;

namespace com.game
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        float Health { get; }
        float MaxHealth { get; }

        event Action<float> OnTakeDamage;
        event Action OnDie;

        void TakeDamage(float damage);
        void Die();
    }
}
