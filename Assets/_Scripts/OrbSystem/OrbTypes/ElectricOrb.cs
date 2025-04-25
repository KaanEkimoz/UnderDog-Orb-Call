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
        [SerializeField] private float electricEffectIntervalInSeconds = 1f;
        [SerializeField] private float electricDamageMultiplier = 1f;
        [Space]
        [Header("Collision Instant Electric Effect")]
        [SerializeField] private GameObject electricEffectPrefab;
        [Header("Electric Line Effect")]
        [SerializeField] private GameObject electricChainEffectPrefab; // Prefab for the electric line
        [SerializeField] private float electricChainEffectDuration = 0.2f;

        private List<IRenderedDamageable> affectedEnemies = new();
        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            damageable.TakeDamageInSeconds(damage * electricDamageMultiplier, electricEffectDurationInSeconds, electricEffectIntervalInSeconds);

            // Find all colliders within the electric bounce radius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, electricBounceRadius);

            // Clear the list of affected enemies and add the initial target
            affectedEnemies.Clear();
            //affectedEnemies.Add(damageable);

            // Filter and sort the colliders by distance
            var sortedDamageables = hitColliders
                .Where(c => c.gameObject.TryGetComponent(out IRenderedDamageable _)) // Filter colliders with IDamageable
                .Select(c => new
                {
                    Collider = c,
                    Distance = Vector3.Distance(transform.position, c.transform.position) // Calculate distance
                })
                .OrderBy(x => x.Distance) // Sort by distance
                .Take(maxElectricBounceCount) // Take the closest 'maxElectricBounceCount' enemies
                .ToList();

            GameRuntimeEvent evt = Game.Event;
            Game.Event = GameRuntimeEvent.Null;

            // Apply damage to the closest enemies
            foreach (var item in sortedDamageables)
            {
                if (item.Collider.gameObject.CompareTag("Player"))
                    continue;

                if (item.Collider.gameObject.TryGetComponent(out IRenderedDamageable hitDamageable))
                {
                    hitDamageable.TakeDamageInSeconds(damage * electricDamageMultiplier, electricEffectDurationInSeconds, electricEffectIntervalInSeconds);
                    affectedEnemies.Add(hitDamageable);
                }
            }

            Game.Event = evt;

            if (electricChainEffectPrefab == null)
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

            StartCoroutine(nameof(CreateChainElectricEffectWithIntervals));
        }
        private void CreateElectricLineBetweenInRangeEnemies()
        {
            IRenderedDamageable firstEnemy = affectedEnemies[0];
            Vector3 firstEnemyPosition = firstEnemy.Renderer.bounds.center; //Center Position

            for (int i = 1; i < affectedEnemies.Count; i++)
            {
                IRenderedDamageable nextEnemy = affectedEnemies[i];
                
                Vector3 nextEnemyPosition = nextEnemy.Renderer.bounds.center; // Center Position

                CreateElectricLine(firstEnemyPosition, nextEnemyPosition);
            }
        }
        private IEnumerator CreateChainElectricEffectWithIntervals()
        {
            float elapsedTime = 0f;
            float damageDivider = electricEffectDurationInSeconds / electricEffectIntervalInSeconds;

            while (elapsedTime < electricEffectDurationInSeconds)
            {
                CreateElectricLineBetweenInRangeEnemies();
                elapsedTime += electricEffectIntervalInSeconds;
                yield return new WaitForSeconds(electricEffectIntervalInSeconds);
            }
        }
        private void CreateElectricLine(Vector3 startPos, Vector3 endPos)
        {
            GameObject electricLineInstance = Instantiate(electricChainEffectPrefab, startPos, Quaternion.identity);

            // Get the ElectricLine component
            ElectricLine electricLine = electricLineInstance.GetComponent<ElectricLine>();

            if (electricLine != null)
            {
                // Set the start and end points
                electricLine.pointAposition = startPos;
                electricLine.pointBposition = endPos;

                // Destroy the electric line after a delay
                StartCoroutine(DestroyElectricLineAfterDelay(electricLineInstance, electricChainEffectDuration));
            }
        }
        private IEnumerator DestroyElectricLineAfterDelay(GameObject electricLineInstance, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(electricLineInstance);
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
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, electricBounceRadius);
            }
        }

#endif

    }
}