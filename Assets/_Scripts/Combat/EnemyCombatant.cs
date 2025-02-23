using UnityEngine;
using com.game.testing;
using System;
using com.absence.attributes;
using com.game.enemysystem.statsystemextensions;
using com.game.generics.interfaces;
using com.game.generics;
using Zenject;
using com.game.player;

namespace com.game.enemysystem
{
    public class EnemyCombatant : MonoBehaviour, IDamageable, IVisible
    {
        [SerializeField] private GameObject m_container;
        [SerializeField, Required] private EnemyStats m_stats;
        [SerializeField] private SparkLight m_sparkLight;
        [SerializeField] private Enemy enemy;
        public SparkLight Spark => m_sparkLight;

        float _health;
        float _maxHealth;

        public bool IsAlive => _health > 0;
        public float Health => _health;
        public float MaxHealth => _maxHealth;

        public event Action<float> OnTakeDamage = delegate { };
        public event Action OnDie = delegate { };

        [Inject] PlayerCombatant _playerCombatant;

        private void Start()
        {
            if(enemy == null)
               enemy = GetComponent<Enemy>();

            _maxHealth = m_stats.GetStat(EnemyStatType.Health);
            _health = _maxHealth;
        }
        public void TakeDamage(float damage)
        {
            if (damage == 0f)
                return;

            float realDamage = damage * (1 - (m_stats.GetStat(EnemyStatType.Armor) / 100));

            _health -= realDamage;

            if (_health <= 0)
            {
                _health = 0;
                Die();
            }
            enemy.ApplySlowForOrbs(GetOrbsCountOnEnemy());
            OnTakeDamage?.Invoke(realDamage);
        }

        public void Die()
        {
            SimpleOrb[] orbsOnEnemy = GetOrbsOnEnemy();

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
        public int GetOrbsCountOnEnemy()
        {
            return GetOrbsOnEnemy().Length;
        }
        public SimpleOrb[] GetOrbsOnEnemy()
        {
            return GetComponentsInChildren<SimpleOrb>();
        }
    }
}
