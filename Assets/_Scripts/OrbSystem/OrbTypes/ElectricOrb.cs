using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.game
{
    public class ElectricOrb : SimpleOrb, IElemental
    {
        [Space]
        [Header("Electric")]
        [SerializeField] private int maxElectricBounceCount = 3;
        [SerializeField] private float electricBounceRadius = 15f;
        [SerializeField] private float electricEffectDurationInSeconds = 5f;
        [SerializeField] private float electrictEffectIntervalInSeconds = 1f;
        [Space]
        [Header("Electric Line")]
        [SerializeField] private GameObject electricLinePrefab; // Prefab for the electric line
        [SerializeField] private float electricLineDuration = 0.2f;

        private List<IDamageable> affectedEnemies = new List<IDamageable>();
        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            damageable.TakeDamageInSeconds(damage, electricEffectDurationInSeconds, electrictEffectIntervalInSeconds);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, electricBounceRadius);

            affectedEnemies.Clear();
            affectedEnemies.Add(damageable);

            int count = 0;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.TryGetComponent(out IDamageable hitDamageable) && count < maxElectricBounceCount)
                {
                    hitDamageable.TakeDamageInSeconds(damage, electricEffectDurationInSeconds, electrictEffectIntervalInSeconds);
                    affectedEnemies.Add(hitDamageable);
                    count++;
                }
            }

            DrawElectricLines();
        }

        private void DrawElectricLines()
        {
            if (electricLinePrefab == null)
            {
                Debug.LogWarning("ElectricLine prefab is not assigned.");
                return;
            }

            // Draw lines between all affected enemies
            for (int i = 0; i < affectedEnemies.Count - 1; i++)
            {
                if (affectedEnemies[i] is Enemy startEnemy && affectedEnemies[i + 1] is Enemy endEnemy)
                {
                    // Instantiate the electric line prefab
                    GameObject electricLineInstance = Instantiate(electricLinePrefab, transform.position, Quaternion.identity);

                    // Get the ElectricLine component
                    ElectricLine electricLine = electricLineInstance.GetComponent<ElectricLine>();

                    if (electricLine != null)
                    {
                        // Set the start and end points
                        electricLine.transformPointA = startEnemy.transform;
                        electricLine.transformPointB = endEnemy.transform;

                        // Destroy the electric line after a delay
                        StartCoroutine(DestroyElectricLineAfterDelay(electricLineInstance, electricLineDuration));
                    }
                }
            }
        }

        private IEnumerator DestroyElectricLineAfterDelay(GameObject electricLineInstance, float delay)
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