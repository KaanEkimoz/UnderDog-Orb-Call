using UnityEngine;
using com.game.testing;
using System;
using com.absence.attributes;
using com.game.enemysystem.statsystemextensions;
using com.game.generics.interfaces;
using com.game.generics;
using Zenject;
using com.game.player;
using System.Collections;
using com.game.miscs;

namespace com.game.enemysystem
{
    public class EnemyCombatant : MonoBehaviour, IDamageable, IVisible
    {
        private const float k_popupPositionRandomization = 0.3f;
        private const float k_dropSpawnForceMagnitude = 2f;
        private const float k_dropSpawnForceYAddition = 0.1f;

        private const int k_maxMoneyDropAmount = 5;
        private const int k_maxExperienceDropAmount = 4;

        [SerializeField] private GameObject m_container;
        [SerializeField, Required] private EnemyStats m_stats;
        [SerializeField] private SparkLight m_sparkLight;
        [SerializeField] private Enemy enemy;
        public SparkLight Spark => m_sparkLight;

        private float _health;
        private float _maxHealth;

        public bool IsAlive => _health > 0;
        public float Health { get => _health; set => _health = value; }
        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public Enemy Owner => enemy;

        public event Action<float> OnTakeDamage = delegate { };
        public event Action OnDie = delegate { };

        PlayerCombatant _playerCombatant;
        [Inject] OrbController _orbController;

        private void Awake()
        {
            if(enemy == null)
               enemy = GetComponent<Enemy>();

            _maxHealth = m_stats.GetStat(EnemyStatType.Health);
            _health = _maxHealth;
        }
        public void ProvidePlayerCombatant(PlayerCombatant playerCombatant)
        {
            _playerCombatant = playerCombatant;
        }
        public void TakeDamage(float damage)
        {
            if (damage == 0f)
                return;

            float realDamage = damage * (1 - (m_stats.GetStat(EnemyStatType.Armor) / 100));

            _health -= realDamage;
            PopupManager.Instance.CreateDamagePopup(realDamage, transform.position 
                + transform.localToWorldMatrix.MultiplyVector(new Vector3(0f, 0.5f, 0f))
                + ((Vector3)UnityEngine.Random.insideUnitCircle * k_popupPositionRandomization)
                , true); // !!!

            if (_health <= 0)
            {
                _health = 0;
                Die();
            }
            enemy.ApplySlowForOrbsOnEnemy(GetOrbsCountOnEnemy());

            OnTakeDamage?.Invoke(damage);
            _playerCombatant.OnLifeSteal(realDamage);
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

        public void Die()
        {
            Debug.Log("difh");
            SimpleOrb[] orbsOnEnemy = GetOrbsOnEnemy();

            foreach (SimpleOrb orb in orbsOnEnemy)
            {
                orb.SetNewDestination(new Vector3(orb.transform.position.x, 0, orb.transform.position.z));
                orb.ResetParent();
            }

            // !!!

            int experienceAmount = UnityEngine.Random.Range(1, k_maxExperienceDropAmount + 1);
            DropManager.Instance.SpawnIndividualExperienceDrops(experienceAmount, transform.position)
                .ForEach(d => d.SetSpawnForce(GetRandomDirectionForDrop(), k_dropSpawnForceMagnitude));

            int moneyAmount = UnityEngine.Random.Range(1, k_maxMoneyDropAmount + 1);
            DropManager.Instance.SpawnIndividualMoneyDrops(moneyAmount, transform.position)
                .ForEach(d => d.SetSpawnForce(GetRandomDirectionForDrop(), k_dropSpawnForceMagnitude));

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

        Vector3 GetRandomDirectionForDrop(float yAddition = k_dropSpawnForceYAddition)
        {
            Vector2 randomUnitCircle = UnityEngine.Random.insideUnitCircle;
            Vector3 result = new Vector3(randomUnitCircle.x, yAddition, randomUnitCircle.y);
            return result;
        }
    }
}
