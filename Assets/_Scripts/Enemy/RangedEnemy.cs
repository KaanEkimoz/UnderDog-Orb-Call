using com.game.enemysystem.statsystemextensions;
using System.Collections;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectilePrebfab;
    //firepoint ekle

    [Header("Shooting Settings")]
    public float projectileSpeed = 5f;
    public float shootingCooldown = 3f;
    
    private bool canShoot = true;

    protected override void CustomUpdate()
    {
        if (CheckDistanceToPlayer() && canShoot == true)
        {
            Shoot();
        }
    }     

    private void Shoot()
    {   
        canShoot = false;
       
        Vector3 targetCenter = target.transform.position + new Vector3(0, target.GetComponent<Collider>().bounds.extents.y, 0); //playerin boyunun yarisi
        Vector3 shootingDirection = (targetCenter - transform.position).normalized; //projectile yonu
        GameObject projectile = Instantiate(projectilePrebfab, transform.position, Quaternion.identity); //projectile'i instantiate et
        projectile.GetComponent<EnemyProjectile>().Damage = newEnemyStats.GetStat(EnemyStatType.Damage); //projectile'a hasar ver
        projectile.GetComponent<Rigidbody>().linearVelocity = shootingDirection*projectileSpeed; //projectile'a hiz ver

        StartCoroutine(EnableCooldown()); //saldiri cooldownunu baslat      

    }

    private IEnumerator EnableCooldown()
    {
        yield return new WaitForSeconds(shootingCooldown); 
        canShoot = true;
    }
}

