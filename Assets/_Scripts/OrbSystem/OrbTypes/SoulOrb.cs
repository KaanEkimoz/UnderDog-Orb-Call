using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.game
{
    public class SoulOrb : SimpleOrb, IElemental
    {
        [Header("Soul Seal Settings")]
        [SerializeField, Range(1, 5)]
        private int maxSeals = 3;
        [SerializeField, Range(0.1f, 2f)]
        private float damageMultiplier = 0.8f;
        [SerializeField]
        private float sealDuration = 15f;

        [Header("Visual Effects")]
        [SerializeField]
        private GameObject sealEffectPrefab;

        private List<IDamageable> sealedEnemies = new List<IDamageable>();

        protected override void ApplyCombatEffects(IDamageable damageable, float damage, bool penetrationCompleted, bool recall)
        {
            base.ApplyCombatEffects(damageable, damage, penetrationCompleted, recall);

            if (recall || (m_latestDamageEvt.CausedDeath && !penetrationCompleted))
                return;

            if (!sealedEnemies.Contains(damageable))
            {
                if (sealedEnemies.Count >= maxSeals)
                {
                    sealedEnemies.RemoveAt(0);
                }
                sealedEnemies.Add(damageable);
                StartCoroutine(RemoveSealAfterDelay(damageable));

                if (sealEffectPrefab != null)
                {
                    CreateEffect(damageable);
                }
            }

            DamageAllSealedEnemies(damage);
        }

        private IEnumerator RemoveSealAfterDelay(IDamageable enemy)
        {
            yield return new WaitForSeconds(sealDuration);
            sealedEnemies.Remove(enemy);
        }

        private void CreateEffect(IDamageable enemy)
        {
            if (enemy is MonoBehaviour enemyMono)
            {
                var effect = Instantiate(sealEffectPrefab,
                                      enemyMono.transform.position,
                                      Quaternion.identity,
                                      enemyMono.transform);
                StartCoroutine(DestroyEffect(effect, sealDuration));
            }
        }

        private IEnumerator DestroyEffect(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (effect != null)
            {
                Destroy(effect);
            }
        }

        private void DamageAllSealedEnemies(float baseDamage)
        {
            foreach (var enemy in sealedEnemies)
            {
                enemy.TakeDamage(baseDamage * damageMultiplier);
            }
        }
    }
}