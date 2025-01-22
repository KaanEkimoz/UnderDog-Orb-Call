using com.game.player;
using com.game.player.statsystemextensions;
using UnityEngine;
using Zenject;
namespace com.game
{
    public class PlayerCombatant : MonoBehaviour, IDamageable
    {
        private float _health;
        private readonly int _maxHealth;
        private PlayerStats _playerStats;
        public bool IsAlive => _health > 0;
        private void Start()
        {
            _playerStats = GetComponent<PlayerStats>();
            _health = _playerStats.StatHolder.GetStat(PlayerStatType.Health);
        }
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
