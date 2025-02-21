using com.game;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float _damage;
    private Rigidbody _rigidBody;

    private void OnEnable()
    {
        if(_rigidBody == null)
            _rigidBody = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("projectile playera carpti");

            if (other.gameObject.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(_damage);

            Destroy(gameObject);
        }
    }
    public void ApplyVelocity(Vector3 velocity)
    {
        _rigidBody.linearVelocity = velocity;
    }
    public Vector3 GetVelocity()
    {
        return _rigidBody.linearVelocity;
    }
    public Rigidbody GetRigidbody()
    {
        return _rigidBody;
    }
    public void SetDamage(float damage)
    {
        _damage = damage;
    }
}
