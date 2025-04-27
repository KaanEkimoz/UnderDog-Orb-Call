using com.game.generics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game
{
    public class FireOrb : SimpleOrb, IElemental
    {
        [Space]
        [Header("Fire")]
        [SerializeField] float fireDurationInSeconds = 5f;
        [SerializeField] float fireDamageIntervalInSeconds = 1f;
        [SerializeField] float fireDamageRadius = 10f;
        [SerializeField] float fireDamageMultiplier = 1f;
        [Space]
        [Header("Collision Instant Fire Effect")]
        [SerializeField] private GameObject instantFireEffectPrefab;
        [Header("Continuos Fire Effect")]
        [SerializeField] private FollowTarget continuosFireEffect; // Prefab for the electric line
        [SerializeField] private float fireEffectDurationInSeconds = 0.2f;

        private List<IRenderedDamageable> affectedEnemies = new();
        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            base.ApplyCombatEffects(damageable, damage);

            damageable.TakeDamageInSeconds(damage * fireDamageMultiplier, fireDurationInSeconds, fireDamageIntervalInSeconds);

            Collider[] hitColliders = Physics.OverlapSphere(damageable.transform.position, fireDamageRadius);

            affectedEnemies.Clear();
            GameObject instantEffect = Instantiate(instantFireEffectPrefab, transform.position, Quaternion.identity);
            StartCoroutine(DestroyFireEffectAfterDelay(instantEffect, 0.3f));

            GameRuntimeEvent evt = Game.Event;
            Game.Event = GameRuntimeEvent.Null;

            foreach (var hitCollider in hitColliders)
            {
                if(hitCollider.gameObject.CompareTag("Player"))
                    continue;

                if (hitCollider.gameObject.TryGetComponent(out IRenderedDamageable hitDamageable))
                {
                    hitDamageable.TakeDamageInSeconds(damage * fireDamageMultiplier, fireDurationInSeconds, fireDamageIntervalInSeconds);
                    affectedEnemies.Add(hitDamageable);
                }

            }

            Game.Event = evt;

            if (continuosFireEffect == null)
            {
                Debug.LogWarning("FireEffect prefab is not assigned.");
                return;
            }

            CreateFireEffectOnEnemiesInRange();
        }
        private void CreateFireEffectOnEnemiesInRange()
        {

            for (int i = 0; i < affectedEnemies.Count; i++)
            {
                IRenderedDamageable currentEnemy = affectedEnemies[i];

                Vector3 topPos = currentEnemy.Renderer.bounds.center; // Top Position

                CreateFireEffect(topPos, currentEnemy.transform);
            }
        }
        private IEnumerator CreateFireEffectWithIntervals()
        {
            float elapsedTime = 0f;
            float damageDivider = fireEffectDurationInSeconds / fireDamageIntervalInSeconds;

            while (elapsedTime < fireEffectDurationInSeconds)
            {
                CreateFireEffectOnEnemiesInRange();
                elapsedTime += fireDamageIntervalInSeconds;
                yield return new WaitForSeconds(fireDamageIntervalInSeconds);
            }
        }
        private void CreateFireEffect(Vector3 effectPos, Transform followTarget = null)
        {
            FollowTarget fireEffectInstance = Instantiate(continuosFireEffect, effectPos, Quaternion.identity);

            if (followTarget != null)
            {
                fireEffectInstance.Target = followTarget;
                fireEffectInstance.KeepStartingOffset = true;
            }

            StartCoroutine(DestroyFireEffectAfterDelay(fireEffectInstance.gameObject, fireEffectDurationInSeconds));
        }
        private IEnumerator DestroyFireEffectAfterDelay(GameObject fireEffectInstance, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(fireEffectInstance);
        }
        protected override void ApplyOrbThrowCollisionEffects(Collision collisionObject)
        {
            base.ApplyOrbThrowCollisionEffects(collisionObject);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            // Draw the slow radius in the editor

            if (currentState != OrbState.OnEllipse)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, fireDamageRadius);
            }
        }

#endif

    }
}
