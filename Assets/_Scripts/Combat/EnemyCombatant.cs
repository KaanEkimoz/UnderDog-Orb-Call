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
using System.Collections.Generic;
using System.Linq;
using com.game.events;

namespace com.game.enemysystem
{
    public class EnemyCombatant : MonoBehaviour, IRenderedDamageable, IVisible
    {
        private const bool k_randomizeDropDirections = false;

        private const float k_popupPositionRandomization = 0.3f;
        private const float k_dropSpawnForceMagnitude = 4.2f;
        private const float k_dropSpawnForceYAddition = 0.1f;

        private const int k_maxMoneyDropAmount = 5;
        private const int k_maxExperienceDropAmount = 4;

        [SerializeField] private GameObject m_container;
        [SerializeField, Required] private Renderer m_renderer;
        [SerializeField, Required] private EnemyStats m_stats;
        [SerializeField] private InterfaceReference<ISpark, MonoBehaviour> m_spark;
        [SerializeField] private Enemy enemy;

        public ISpark Spark => m_spark.Value;

        private float _health;
        private float _maxHealth;

        public bool IsAlive => _health > 0;
        public float Health { get => _health; set => _health = value; }
        public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public Enemy Owner => enemy;
        public Renderer Renderer => m_renderer;

        public event Action<float> OnTakeDamage = delegate { };
        public event Action<float> OnHeal = delegate { };
        public event Action<DeathCause> OnDie = delegate { };

        bool _deathFlag;
        PlayerCombatant _playerCombatant;
        [Inject] PlayerOrbController _orbController;

        private void Awake()
        {
            enemy.OnDeathRequest += Die;

            _maxHealth = m_stats.GetStat(EnemyStatType.Health);
            _health = _maxHealth;
        }
        private void Start()
        {
            if (_playerCombatant == null) ProvidePlayerCombatant(Player.Instance.Hub.Combatant);
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

            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.CreateDamagePopup(realDamage, transform.position
                    + transform.localToWorldMatrix.MultiplyVector(new Vector3(0f, 0.5f, 0f))
                    + ((Vector3)UnityEngine.Random.insideUnitCircle * k_popupPositionRandomization)
                    , true); // !!!
            }

            if (_health <= 0)
            {
                _health = 0;

                DeathCause deathCause = DeathCause.Default;
                if (Game.Event == GameRuntimeEvent.OrbThrow) deathCause = DeathCause.OrbThrow;
                else if (Game.Event == GameRuntimeEvent.OrbCall) deathCause = DeathCause.OrbCall;

                Die(deathCause);
            }
            enemy.ApplySlowForOrbsOnEnemy(GetOrbsCountOnEnemy());
            OnTakeDamage?.Invoke(damage);
            _playerCombatant.OnLifeSteal(realDamage);
        }
        public void ApplyKnockback(Vector3 forceDirection, float forceMagnitude)
        {
            enemy.ApplyKnockbackForce(forceDirection, forceMagnitude);
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
        public void Heal(float amount)
        {
            if (amount == 0f)
                return;

            _health += amount;
            OnHeal?.Invoke(amount);
        }
        public void Die(DeathCause cause)
        {
            if (_deathFlag)
                return;

            _deathFlag = true;

            SimpleOrb[] orbsOnEnemy = GetOrbsOnEnemy();

            foreach (SimpleOrb orb in orbsOnEnemy)
            {
                orb.SetNewDestination(new Vector3(orb.transform.position.x, 0, orb.transform.position.z));
                orb.ResetParent();
            }

            // !!!

            List<bool> conditions = new()
            {
                DropManager.Instance != null,
                cause != DeathCause.Self,
                !enemy.IsVirtual,
                (!enemy.IsFake) || (enemy.IsFake && (!InternalSettings.FAKE_ENEMIES_DONT_DROP)),
            };

            if (conditions.All(c => c))
            {
                int experienceAmount = UnityEngine.Random.Range(1, k_maxExperienceDropAmount + 1);
                DropManager.Instance.SpawnIndividualExperienceDrops(experienceAmount, transform.position,
                    d => d.SetSpawnForce(GetRandomDirectionForDrop(cause), k_dropSpawnForceMagnitude));

                int moneyAmount = UnityEngine.Random.Range(1, k_maxMoneyDropAmount + 1);
                DropManager.Instance.SpawnIndividualMoneyDrops(moneyAmount, transform.position,
                    d => d.SetSpawnForce(GetRandomDirectionForDrop(cause), k_dropSpawnForceMagnitude));
            }

            TestEventChannel.ReceiveEnemyKill();

            if (cause != DeathCause.Self || cause != DeathCause.Internal)
                PlayerEventChannel.CommitEnemyKill();

            if (m_container != null) Destroy(m_container);
            else Destroy(gameObject);

            OnDie?.Invoke(cause);

            Debug.Log("Dusman bayildi");
        }
        public int GetOrbsCountOnEnemy()
        {
            return GetOrbsOnEnemy().Length;
        }
        public SimpleOrb[] GetOrbsOnEnemy()
        {
            return GetComponentsInChildren<SimpleOrb>();
        }

        Vector3 GetRandomDirectionForDrop(DeathCause deathCause, float yAddition = k_dropSpawnForceYAddition)
        {
            Vector3 playerPosition = Vector3.zero;
            Vector3 initialVector = Vector3.zero;

            if (Player.Instance != null) 
                playerPosition = Player.Instance.transform.position;

            if (deathCause == DeathCause.OrbThrow) 
                initialVector = transform.position - playerPosition;
            else if (deathCause == DeathCause.OrbCall) 
                initialVector = playerPosition - transform.position;

            initialVector.Normalize();

            Vector3 resultVector = initialVector;
            Vector2 randomUnitCircle = UnityEngine.Random.insideUnitCircle;
            Vector3 random = new Vector3(randomUnitCircle.x, yAddition, randomUnitCircle.y);

#pragma warning disable CS0162 // Unreachable code detected
            if (k_randomizeDropDirections || deathCause == DeathCause.Default)
                resultVector += random;
#pragma warning restore CS0162 // Unreachable code detected

            resultVector.Normalize();

            return resultVector;
        }
    }
}
