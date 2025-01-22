using System.Collections;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public float meleeAttackDelay = 2f;
    public float attackingCooldown = 2f;

    private bool canAttack = true;

    protected override void CustomUpdate()
    {
        if (CheckDistanceToPlayer() && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        canAttack = false;

        //HASAR VER
        Debug.Log("hasar verildi");

        StartCoroutine(EnableCooldown());

    }

    private IEnumerator EnableCooldown()
    {
        yield return new WaitForSeconds(attackingCooldown);
        canAttack = true;
    }
}
