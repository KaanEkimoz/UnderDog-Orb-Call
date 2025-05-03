using com.game.enemysystem;
using com.game.player;
using System.Collections;
using UnityEngine;
using Zenject;

namespace com.game
{
    public class LifestealOrb : SimpleOrb, IElemental
    {
        [Header("Life Steal Settings")]
        [SerializeField] private float maxStealableHealthPerEnemy = 100f;
        [SerializeField] private float stealInterval = 2f;
        [SerializeField] private float healthPerSteal = 10f;

        [Header("Visual Effects")]
        [SerializeField] private GameObject instantStealEffect;
        [SerializeField] private GameObject continousStealEffect;

        private IDamageable currentTarget;
        private float totalStolenHealth;
        private Coroutine stealCoroutine;

        [Inject] private PlayerCombatant playerCombatant;

        private void OnEnable()
        {
            OnCalled += StopStealFromEnemy;
        }
        private void OnDisable()
        {
            OnCalled -= StopStealFromEnemy;
        }

        protected override void ApplyCombatEffects(IDamageable damageable, float damage, bool penetrationCompleted, bool recall)
        {
            base.ApplyCombatEffects(damageable, damage, penetrationCompleted, recall);

            if (ShouldSkipEffects(recall, penetrationCompleted))
                return;

            if (currentTarget == null)
                StartStealFromEnemy(damageable);
        }

        private bool ShouldSkipEffects(bool recall, bool penetrationCompleted)
        {
            return recall || (m_latestDamageEvt.CausedDeath && !penetrationCompleted);
        }

        private void StartStealFromEnemy(IDamageable enemyDamageable)
        {
            currentTarget = enemyDamageable;
            totalStolenHealth = 0f;

            if (stealCoroutine != null)
                StopCoroutine(stealCoroutine);

            stealCoroutine = StartCoroutine(StealHealthRoutine());

            if (continousStealEffect != null && enemyDamageable is MonoBehaviour enemyMono)
                Instantiate(continousStealEffect, enemyMono.transform);
        }

        private IEnumerator StealHealthRoutine()
        {
            while (currentTarget != null && totalStolenHealth < maxStealableHealthPerEnemy)
            {
                yield return new WaitForSeconds(stealInterval);

                if (currentTarget != null)
                {
                    float actualSteal = Mathf.Min(healthPerSteal, maxStealableHealthPerEnemy - totalStolenHealth);
                    currentTarget.TakeDamage(actualSteal);
                    playerCombatant.Heal(actualSteal);
                    totalStolenHealth += actualSteal;
                }
            }

            StopStealFromEnemy();
        }

        private void StopStealFromEnemy()
        {
            currentTarget = null;
            if (stealCoroutine != null)
            {
                StopCoroutine(stealCoroutine);
                stealCoroutine = null;
            }
        }
    }
}