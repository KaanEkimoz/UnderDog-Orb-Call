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
        [Space]
        [Header("Collision Instant Fire Effect")]
        [SerializeField] private GameObject instantFireEffectPrefab;
        [Header("Continuos Fire Effect")]
        [SerializeField] private GameObject continuosFireEffect; // Prefab for the electric line
        [SerializeField] private float fireEffectDurationInSeconds = 0.2f;


        private List<IDamageable> affectedEnemies = new List<IDamageable>();
        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            damageable.TakeDamageInSeconds(damage, fireDurationInSeconds, fireDamageIntervalInSeconds);

            // Find all colliders within the electric bounce radius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, fireDamageRadius);

            // Clear the list of affected enemies and add the initial target
            affectedEnemies.Clear();
            //affectedEnemies.Add(damageable);
            GameObject instantEffect = Instantiate(instantFireEffectPrefab, transform.position, Quaternion.identity);
            StartCoroutine(DestroyFireEffectAfterDelay(instantEffect, 0.3f));

            foreach (var hitCollider in hitColliders)
            {
                if(hitCollider.gameObject.CompareTag("Player"))
                    continue;

                if (hitCollider.gameObject.TryGetComponent(out IDamageable hitDamageable))
                {
                    hitDamageable.TakeDamageInSeconds(damage, fireDurationInSeconds, fireDamageIntervalInSeconds);
                    affectedEnemies.Add(hitDamageable);
                }

            }
            if (continuosFireEffect == null)
            {
                Debug.LogWarning("ElectricLine prefab is not assigned.");
                return;
            }

            CreateFireEffectOnEnemiesInRange();
        }
        private void CreateFireEffectOnEnemiesInRange()
        {

            for (int i = 0; i < affectedEnemies.Count; i++)
            {
                MonoBehaviour currentEnemy = affectedEnemies[i] as MonoBehaviour;

                Vector3 topPos = currentEnemy.GetComponentInChildren<MeshRenderer>().bounds.center; // Top Position

                CreateFireEffect(topPos);
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
        private void CreateFireEffect(Vector3 effectPos)
        {
            GameObject fireEffectInstance = Instantiate(continuosFireEffect, effectPos, Quaternion.identity);

            // Get the ElectricLine component
            //ElectricLine electricLine = electricLineInstance.GetComponent<ElectricLine>();


            StartCoroutine(DestroyFireEffectAfterDelay(fireEffectInstance, fireEffectDurationInSeconds));
        }
        private IEnumerator DestroyFireEffectAfterDelay(GameObject electricLineInstance, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(electricLineInstance);
        }
        protected override void ApplyCollisionEffects(Collision collisionObject)
        {
            base.ApplyCollisionEffects(collisionObject);
        }
    }
}
