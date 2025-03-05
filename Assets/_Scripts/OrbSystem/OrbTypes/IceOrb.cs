using UnityEngine;
namespace com.game
{
    public class IceOrb : SimpleOrb, IElemental
    {
        [Space]
        [Header("Slow")]
        [Range(0f, 100f)]
        [SerializeField] float slowPercent = 100f;
        [SerializeField] float slowDurationInSeconds = 1f;
        [SerializeField] float slowRadius = 10f;

        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            base.ApplyCombatEffects(damageable, damage);
        }
        protected override void ApplyCollisionEffects(Collision collisionObject)
        {
            base.ApplyCollisionEffects(collisionObject);

            //if(collisionObject.gameObject.TryGetComponent(out Enemy hitEnemy))
              //  hitEnemy.ApplySlowForSeconds(slowPercent, slowDurationOnSeconds);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, slowRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.TryGetComponent(out Enemy hitEnemy))
                    hitEnemy.ApplySlowForSeconds(slowPercent, slowDurationInSeconds);
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            // Draw the slow radius in the editor

            if (currentState != OrbState.OnEllipse)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, slowRadius);
            }
        }

#endif

    }
}
