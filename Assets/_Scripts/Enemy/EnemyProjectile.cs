using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("projectile playera carpti");
            //HASAR VER
            Destroy(gameObject);
        }
    }
}
