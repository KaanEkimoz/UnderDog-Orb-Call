using com.game.enemysystem.statsystemextensions;
using com.game.player.statsystemextensions;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

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
        public event Action<float> OnHeal = delegate { };
        public event Action OnDie = delegate { };

        [Inject] OrbController _orbController;

        private void Awake()
        {
            if(_playerStats == null)
                _playerStats = GetComponent<PlayerStats>();

            _maxHealth = _playerStats.GetStat(PlayerStatType.Health);
            _health = _maxHealth;
        }
        public void OnLifeSteal(float amount)
        {
            Heal(amount * (_playerStats.GetStat(PlayerStatType.LifeSteal) / 100));
        }
        public void TakeDamage(float damage)
        {
            if (damage == 0f)
                return;

            _health -= damage * (1 - (_playerStats.GetStat(PlayerStatType.Armor) / 100)); ;

            if (_health <= 0)
            {
                _health = 0;
                Die();
            }
            OnTakeDamage?.Invoke(damage);
        }
        public void TakeDamageInSeconds(float damage, float durationInSeconds, float intervalInSeconds)
        {
            StartCoroutine(TakeDamageOverTime(damage, durationInSeconds, intervalInSeconds));
        }
        private IEnumerator TakeDamageOverTime(float damage, float durationInSeconds, float intervalInSeconds)
        {
            float elapsedTime = 0f;
            float damageDivider = durationInSeconds / intervalInSeconds;

            while (elapsedTime < durationInSeconds)
            {
                TakeDamage(damage / damageDivider);
                elapsedTime += intervalInSeconds;
                yield return new WaitForSeconds(intervalInSeconds);
            }
        }
        public void HealWithLifeSteal(float amount)
        {
            Heal(amount * (_playerStats.GetStat(PlayerStatType.LifeSteal) / 100));
        }
        public void Heal(float amount)
        {
            _health += amount;

            if (_health > _maxHealth)
                _health = _maxHealth;

            OnHeal?.Invoke(amount);
        }
        public void Die()
        {
            if (m_container != null) Destroy(m_container);
            else Destroy(gameObject);

            OnDie?.Invoke();
        }
    }
}
