using System.Collections.Generic;
using UnityEngine;
namespace com.game
{
    public class ElectricOrb : SimpleOrb, IElemental
    {
        [Space]
        [Header("Electric")]
        [SerializeField] private float electricEffectDurationInSeconds;
        [SerializeField] private float electrictEffectIntervalInSeconds;
        [SerializeField] private int maxElectricBounceCount = 3;
        [SerializeField] private float electricBounceRadius = 15f;

        private List<IDamageable> affectedEnemies = new List<IDamageable>();
        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            damageable.TakeDamageInSeconds(damage, electricEffectDurationInSeconds, electrictEffectIntervalInSeconds);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, electricBounceRadius);

            int count = 0;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.TryGetComponent(out IDamageable hitDamageable) || count <= maxElectricBounceCount)
                {
                    hitDamageable.TakeDamageInSeconds(damage, electricEffectDurationInSeconds, electrictEffectIntervalInSeconds);
                    count++;
                }
            }
        }
        protected override void ApplyCollisionEffects(Collision collisionObject)
        {
            base.ApplyCollisionEffects(collisionObject);
        }
    }
}
