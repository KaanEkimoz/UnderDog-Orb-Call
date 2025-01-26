using UnityEngine;
using com.game.testing;
using System.Linq;

namespace com.game
{
    public class EnemyCombatant : MonoBehaviour, IDamageable
    {
        private float _health;
        private readonly int _maxHealth;
        public bool IsAlive => _health > 0;

        private void Start()
        {
            _health = 20;
        }
        private void Update()
        {
            Debug.Log("Enemy Health: " + _health);
        }
        public void TakeDamage(float damage)
        {
            _health -= damage;

            if (_health <= 0)
            {
                _health = 0;
                Die();
            }
        }
        public void Die()
        {
            SimpleOrb[] orbsOnEnemy = GetComponentsInChildren<SimpleOrb>();

            foreach (SimpleOrb orb in orbsOnEnemy)
            {
                orb.SetNewDestination(new Vector3(orb.transform.position.x, 0, orb.transform.position.z));
                orb.ResetParent();
            }

            TestEventChannel.ReceiveEnemyKill();
            Destroy(gameObject);
        }
    }
}
