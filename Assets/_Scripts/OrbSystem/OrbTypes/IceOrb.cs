using com.game.utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.game
{
    public class IceOrb : SimpleOrb, IElemental
    {
        [Header("Ice Settings")]
        [Range(0f, 100f), SerializeField] private float iceSlowPercent = 100f;
        [SerializeField] private float iceSlowDurationInSeconds = 1f;
        [SerializeField] private float iceSlowRadius = 10f;

        [Header("Ice Effects")]
        [SerializeField] private GameObject instantIceEffect;
        [SerializeField] private GameObject continuousIceEffect;
        [SerializeField] private float iceEffectDurationInSeconds = 1f;

        private List<IRenderedDamageable> affectedEnemies = new();
        private Coroutine iceEffectCoroutine;

        protected override void ApplyCombatEffects(IDamageable damageable, float damage, bool penetrationCompleted, bool recall)
        {
            base.ApplyCombatEffects(damageable, damage, penetrationCompleted, recall);

            if (ShouldSkipEffects(recall, penetrationCompleted))
                return;

            using (new GameEventScope(GameRuntimeEvent.Null))
            {
                ApplySlowEffectInRadius();
                CreateInstantIceEffect();
                CacheAffectedEnemies();
            }

            if (continuousIceEffect != null)
                CreateIceEffectsOnEnemies();
            else
                Debug.LogWarning("Continuous ice effect prefab is not assigned.");
        }

        private bool ShouldSkipEffects(bool recall, bool penetrationCompleted)
        {
            return recall || (m_latestDamageEvt.CausedDeath && !penetrationCompleted);
        }

        private void ApplySlowEffectInRadius()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, iceSlowRadius);

            foreach (var collider in hitColliders)
            {
                if (collider.gameObject.TryGetComponent(out ISlowable slowable))
                    slowable.SlowForSeconds(iceSlowPercent, iceSlowDurationInSeconds);
            }
        }

        private void CreateInstantIceEffect()
        {
            if (instantIceEffect != null)
            {
                GameObject effect = Instantiate(instantIceEffect, transform.position, Quaternion.identity);
                StartCoroutine(DestroyEffectAfterDelay(effect, 0.3f));
            }
        }

        private void CacheAffectedEnemies()
        {
            affectedEnemies.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, iceSlowRadius);

            foreach (var collider in hitColliders)
            {
                if (!collider.gameObject.CompareTag("Player") &&
                    collider.gameObject.TryGetComponent(out IRenderedDamageable enemy))
                {
                    affectedEnemies.Add(enemy);
                }
            }
        }
        private void CreateIceEffectsOnEnemies()
        {
            foreach (var enemy in affectedEnemies)
                CreateContinuousIceEffect(enemy.Renderer.bounds.center + (Vector3.up * 0.5f));
        }

        private void CreateContinuousIceEffect(Vector3 position)
        {
            GameObject effect = Instantiate(continuousIceEffect, position, Quaternion.identity);
            StartCoroutine(DestroyEffectAfterDelay(effect, iceEffectDurationInSeconds));
        }

        private IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (effect != null)
            {
                Destroy(effect);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (currentState != OrbState.OnEllipse)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, iceSlowRadius);
            }
        }
#endif
    }
}