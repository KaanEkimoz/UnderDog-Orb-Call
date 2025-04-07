using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.game
{
    public class IceOrb : SimpleOrb, IElemental
    {
        [Space]
        [Header("Ice")]
        [Range(0f, 100f)]
        [SerializeField] float iceSlowPercent = 100f;
        [SerializeField] float iceSlowDurationInSeconds = 1f;
        [SerializeField] float iceSlowRadius = 10f;
        [Header("Collision Instant Ice Effect")]
        [SerializeField] private GameObject instantIceEffect;
        [Header("Continuos Ice Effect")]
        [SerializeField] private GameObject continuosIceEffect; // Prefab for the electric line
        [SerializeField] private float iceEffectDurationInSeconds = 0.2f;

        private List<IDamageable> affectedEnemies = new List<IDamageable>();
        protected override void ApplyCombatEffects(IDamageable damageable, float damage)
        {
            base.ApplyCombatEffects(damageable, damage);
        }
        protected override void ApplyOrbThrowTriggerEffects(Collider collisionObject)
        {
            base.ApplyOrbThrowTriggerEffects(collisionObject);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, iceSlowRadius);

            GameRuntimeEvent evt = Game.Event;
            Game.Event = GameRuntimeEvent.Null;

            foreach (var hitCollider in hitColliders)
                if (hitCollider.gameObject.TryGetComponent(out Enemy hitEnemy))
                    hitEnemy.ApplySlowForSeconds(iceSlowPercent, iceSlowDurationInSeconds);

            affectedEnemies.Clear();
            GameObject instantEffect = Instantiate(instantIceEffect, transform.position, Quaternion.identity);
            StartCoroutine(DestroyIceEffectAfterDelay(instantEffect, 0.3f));

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.CompareTag("Player"))
                    continue;

                if (hitCollider.gameObject.TryGetComponent(out IDamageable hitDamageable))
                    affectedEnemies.Add(hitDamageable);
            }

            Game.Event = evt;

            if (continuosIceEffect == null)
            {
                Debug.LogWarning("IceEffect prefab is not assigned.");
                return;
            }

            CreateIceEffectOnEnemiesInRange();
        }
        private void CreateIceEffectOnEnemiesInRange()
        {

            for (int i = 0; i < affectedEnemies.Count; i++)
            {
                MonoBehaviour currentEnemy = affectedEnemies[i] as MonoBehaviour;

                Vector3 topPos = currentEnemy.GetComponentInChildren<MeshRenderer>().bounds.center; // Top Position

                CreateIceEffect(topPos);
            }
        }
        private IEnumerator CreateIceEffectWithIntervals()
        {
            CreateIceEffectOnEnemiesInRange();
            yield return new WaitForSeconds(iceEffectDurationInSeconds);
        }
        private void CreateIceEffect(Vector3 effectPos)
        {
            GameObject fireEffectInstance = Instantiate(continuosIceEffect, effectPos, Quaternion.identity);

            StartCoroutine(DestroyIceEffectAfterDelay(fireEffectInstance, iceEffectDurationInSeconds));
        }
        private IEnumerator DestroyIceEffectAfterDelay(GameObject fireEffectInstance, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(fireEffectInstance);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            // Draw the slow radius in the editor

            if (currentState != OrbState.OnEllipse)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, iceSlowRadius);
            }
        }

#endif

    }
}
