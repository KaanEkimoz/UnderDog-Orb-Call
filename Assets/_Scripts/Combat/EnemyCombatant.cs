using UnityEngine;
using com.game.testing;
using System;
using com.absence.attributes;
using com.game.enemysystem.statsystemextensions;

namespace com.game.enemysystem
{
    public class EnemyCombatant : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject m_container;
        [SerializeField, Required] private EnemyStats m_stats;

        float _health;
        float _maxHealth;

        public bool IsAlive => _health > 0;
        public float Health => _health;
        public float MaxHealth => _maxHealth;

        public event Action<float> OnTakeDamage = delegate { };
        public event Action OnDie = delegate { };

        private void Start()
        {
            _maxHealth = m_stats.GetStat(EnemyStatType.Health);
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
            if (m_container != null) Destroy(m_container);
            else Destroy(gameObject);

            OnDie?.Invoke();
        }
    }
}
