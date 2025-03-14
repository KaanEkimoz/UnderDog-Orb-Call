using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [Header("Electric Line Effect")]
        [SerializeField] private GameObject electricLineEffectPrefab; // Prefab for the electric line
        [SerializeField] private float electricLineEffectDuration = 0.2f;

        private List<IDamageable> affectedEnemies = new List<IDamageable>();
        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            damageable.TakeDamageInSeconds(damage, electricEffectDurationInSeconds, electrictEffectIntervalInSeconds);

            // Find all colliders within the electric bounce radius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, electricBounceRadius);

            // Clear the list of affected enemies and add the initial target
            affectedEnemies.Clear();
            //affectedEnemies.Add(damageable);

            // Filter and sort the colliders by distance
            var sortedDamageables = hitColliders
                .Where(c => c.gameObject.TryGetComponent(out IDamageable _)) // Filter colliders with IDamageable
                .Select(c => new
                {
                    Collider = c,
                    Distance = Vector3.Distance(transform.position, c.transform.position) // Calculate distance
                })
                .OrderBy(x => x.Distance) // Sort by distance
                .Take(maxElectricBounceCount) // Take the closest 'maxElectricBounceCount' enemies
                .ToList();

            // Apply damage to the closest enemies
            foreach (var item in sortedDamageables)
            {
                if (item.Collider.gameObject.TryGetComponent(out IDamageable hitDamageable))
                {
                    hitDamageable.TakeDamageInSeconds(damage, electricEffectDurationInSeconds, electrictEffectIntervalInSeconds);
                    affectedEnemies.Add(hitDamageable);
                }
            }

            DrawElectricLines();
        }

        private void DrawElectricLines()
        {
            if (electricLineEffectPrefab == null)
            {
                Debug.LogWarning("ElectricLine prefab is not assigned.");
                return;
            }

            // Ensure there are at least two enemies to draw lines
            if (affectedEnemies.Count < 2)
            {
                Debug.LogWarning("Not enough enemies to draw electric lines.");
                return;
            }

            // Get the first enemy (initial target)
            MonoBehaviour firstEnemy = affectedEnemies[0] as MonoBehaviour;
            if (firstEnemy == null)
            {
                Debug.LogWarning("First enemy is not a MonoBehaviour.");
                return;
            }

            // Get the center position of the first enemy
            Vector3 firstEnemyPosition = firstEnemy.GetComponentInChildren<MeshRenderer>().bounds.center;

            // Draw lines from the first enemy to all other affected enemies
            for (int i = 1; i < affectedEnemies.Count; i++)
            {
                MonoBehaviour currentEnemy = affectedEnemies[i] as MonoBehaviour;
                if (currentEnemy == null)
                {
                    Debug.LogWarning($"Enemy at index {i} is not a MonoBehaviour.");
                    continue;
                }

                // Get the center position of the current enemy
                Vector3 currentEnemyPosition = currentEnemy.GetComponentInChildren<MeshRenderer>().bounds.center;

                // Instantiate the electric line prefab
                GameObject electricLineInstance = Instantiate(electricLineEffectPrefab, firstEnemyPosition, Quaternion.identity);

                // Get the ElectricLine component
                ElectricLine electricLine = electricLineInstance.GetComponent<ElectricLine>();

                if (electricLine != null)
                {
                    // Set the start and end points
                    electricLine.pointAposition = firstEnemyPosition;
                    electricLine.pointBposition = currentEnemyPosition;

                    // Destroy the electric line after a delay
                    StartCoroutine(DestroyElectricLineAfterDelay(electricLineInstance, electricLineEffectDuration));
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