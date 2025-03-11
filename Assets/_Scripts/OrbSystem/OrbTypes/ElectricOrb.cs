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
        [Header("Line Renderer")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float lineDuration = 0.2f;

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
                if (hitCollider.gameObject.TryGetComponent(out IDamageable hitDamageable) && count <= maxElectricBounceCount)
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
            if (lineRenderer == null)
            {
                Debug.LogWarning("LineRenderer is not assigned.");
                return;
            }
            lineRenderer.positionCount = affectedEnemies.Count;
            for (int i = 0; i < affectedEnemies.Count; i++)
            {
                if (affectedEnemies[i] is Enemy enemy)
                {
                    lineRenderer.SetPosition(i, enemy.transform.position);
                }
            }

            StartCoroutine(ClearLineAfterDelay());
        }

        private System.Collections.IEnumerator ClearLineAfterDelay()
        {
            yield return new WaitForSeconds(lineDuration);
            lineRenderer.positionCount = 0;
        }
        protected override void ApplyCollisionEffects(Collision collisionObject)
        {
            base.ApplyCollisionEffects(collisionObject);
        }
    }
}
