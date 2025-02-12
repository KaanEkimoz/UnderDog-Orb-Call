using com.game.player.statsystemextensions;
using System;
using UnityEngine;

namespace com.game.player
{
    public class PlayerCombatant : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject m_container;

        float _health;
        float _maxHealth;
        PlayerStats _playerStats;

        public bool IsAlive => _health > 0;
        public float Health => _health;
        public float MaxHealth => _maxHealth;

        public event Action<float> OnTakeDamage = delegate { };
        public event Action OnDie = delegate { };

        private void Awake()
        {
            _playerStats = GetComponent<PlayerStats>();
            _maxHealth = _playerStats.GetStat(PlayerStatType.Health);
            _health = _maxHealth;

            Debug.Log("Player Health: " + _health);
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
            if (m_container != null) Destroy(m_container);
            else Destroy(gameObject);

            OnDie?.Invoke();
        }
    }
}
