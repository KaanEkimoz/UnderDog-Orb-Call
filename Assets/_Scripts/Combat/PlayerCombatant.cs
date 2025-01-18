using UnityEngine;
namespace com.game
{
    public class PlayerCombatant : MonoBehaviour, IDamageable
    {
        private float _health;
        private readonly int _maxHealth;
        public bool IsAlive => _health > 0;

        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;

            _health -= damage;

            if (_health <= 0)
            {
                _health = 0;
                Die();
            }
        }

        public void Die()
        {
            if (!IsAlive) return;
            Destroy(gameObject);
        }
    }
}
