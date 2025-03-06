using com.game.orbsystem.statsystemextensions;
using com.game;
using System.Collections;
using UnityEngine;
using com.game.enemysystem.statsystemextensions;

public class MeleeEnemy : Enemy
{
    public float meleeAttackDelay = 2f;
    public float attackingCooldown = 2f;
    private bool canAttack = true;


    protected override void CustomUpdate()
    {
        navMeshAgent.SetDestination(target.transform.position);

        if (GetDistanceToPlayer() && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        canAttack = false;

        //HASAR VER
        if (target.gameObject.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(enemyStats.GetStat(EnemyStatType.Damage));

        Debug.Log("hasar verildi");

        StartCoroutine(EnableCooldown());

    }

    private IEnumerator EnableCooldown()
    {
        yield return new WaitForSeconds(attackingCooldown);
        canAttack = true;
    }
}
