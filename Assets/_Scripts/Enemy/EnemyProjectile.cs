using com.game.enemysystem.statsystemextensions;
using com.game;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float Damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("projectile playera carpti");

            if (other.gameObject.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(Damage);

            //HASAR VER
            Destroy(gameObject);
        }
    }
}
