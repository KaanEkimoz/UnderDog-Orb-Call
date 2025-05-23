using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.game
{
    public class SoulOrb : SimpleOrb, IElemental
    {
        [Header("Soul Seal Settings")]
        [SerializeField, Range(1, 10)] private int maxSeals = 3;
        [SerializeField, Range(0.1f, 2f)] private float sealedDamageMultiplier = 0.8f;
        [SerializeField] private float sealDuration = 15f;

        [Header("Visual Effects")]
        [SerializeField] private GameObject sealEffectPrefab;

        private List<IRenderedDamageable> sealedEnemies = new List<IRenderedDamageable>();

        protected override void ApplyCombatEffects(IDamageable damageable, float damage, bool penetrationCompleted, bool recall)
        {
            base.ApplyCombatEffects(damageable, damage, penetrationCompleted, recall);

            if (recall || (m_latestDamageEvt.CausedDeath && !penetrationCompleted))
                return;

            if (!sealedEnemies.Contains(damageable as IRenderedDamageable))
            {
                if (sealedEnemies.Count >= maxSeals)
                    sealedEnemies.RemoveAt(0);

                sealedEnemies.Add(damageable as IRenderedDamageable);

                StartCoroutine(RemoveSealAfterDelay(damageable as IRenderedDamageable));

                if (sealEffectPrefab != null)
                    CreateEffect(damageable as IRenderedDamageable);
            }

            DamageAllSealedEnemies(damage);
        }

        private IEnumerator RemoveSealAfterDelay(IRenderedDamageable enemy)
        {
            yield return new WaitForSeconds(sealDuration);
            sealedEnemies.Remove(enemy);
        }

        private void CreateEffect(IRenderedDamageable enemy)
        {
            var effect = Instantiate(sealEffectPrefab, enemy.Renderer.bounds.max, Quaternion.identity);
            StartCoroutine(DestroyEffect(effect, sealDuration));
        }

        private IEnumerator DestroyEffect(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (effect != null)
                Destroy(effect);
        }

        private void DamageAllSealedEnemies(float baseDamage)
        {
            foreach (var enemy in sealedEnemies)
                enemy.TakeDamage(baseDamage * sealedDamageMultiplier);
        }
    }
}