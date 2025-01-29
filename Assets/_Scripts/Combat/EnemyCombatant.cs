using UnityEngine;
using com.game.testing;
using System;

namespace com.game.enemysystem
{
    public class EnemyCombatant : MonoBehaviour, IDamageable
    {
        float _health;
        float _maxHealth;

        public bool IsAlive => _health > 0;
        public float Health => _health;
        public float MaxHealth => _maxHealth;

        public event Action<float> OnTakeDamage = delegate { };
        public event Action OnDie = delegate { };

        private void Start()
        {
            _maxHealth = 20f;
            _health = _maxHealth;
        }

        private void Update()
        {
            //Debug.Log("Enemy Health: " + _health);
        }

        public void TakeDamage(float damage)
        {
            if (damage == 0f)
                return;

            _health -= damage;

            if (_health <= 0)
            {
                _health = 0;
                Die();
            }

            OnTakeDamage?.Invoke(damage);
        }

        public void Die()
        {
            SimpleOrb[] orbsOnEnemy = GetComponentsInChildren<SimpleOrb>();

            foreach (SimpleOrb orb in orbsOnEnemy)
            {
                orb.SetNewDestination(new Vector3(orb.transform.position.x, 0, orb.transform.position.z));
                orb.ResetParent();
            }

            TestEventChannel.ReceiveEnemyKill();
            Destroy(gameObject);

            OnDie?.Invoke();
        }
    }
}
