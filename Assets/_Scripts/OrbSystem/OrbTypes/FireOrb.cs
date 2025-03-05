using UnityEngine;

namespace com.game
{
    public class FireOrb : SimpleOrb, IElemental
    {
        [Space]
        [Header("Fire")]
        [SerializeField] float fireDurationInSeconds = 1f;
        [SerializeField] float fireDamageIntervalInSeconds = 100f;
        [SerializeField] float fireDamageRadius = 10f;

        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            //base.ApplyCombatEffects(damageable);
            damageable.TakeDamageInSeconds(damage, fireDurationInSeconds, fireDamageIntervalInSeconds);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, fireDamageRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.TryGetComponent(out IDamageable hitDamageable))
                    hitDamageable.TakeDamage(damage);
            }
        }
        protected override void ApplyCollisionEffects(Collision collisionObject)
        {
            base.ApplyCollisionEffects(collisionObject);
        }
    }
}
