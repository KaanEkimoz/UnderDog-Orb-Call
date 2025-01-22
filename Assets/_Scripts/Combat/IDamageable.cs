namespace com.game
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void Die();
        bool IsAlive { get; }
    }
}
