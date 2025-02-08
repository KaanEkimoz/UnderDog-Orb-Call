using com.game.enemysystem.statsystemextensions;
using System.Collections;
using UnityEngine;
public class RangedEnemy : Enemy
{
    public GameObject projectilePrebfab;

    [Header("Shoot")]
    public float shootingCooldown = 3f;
    public Transform firePoint;
    [Header("Projectile")]
    public float projectileSpeed = 5f;

    private bool canShoot = true;

    private ObjectPool _objectPool;
    protected override void Start()
    {
        base.Start();
        _objectPool = FindFirstObjectByType<ObjectPool>();
    }
    protected override void CustomUpdate()
    {
        if (CheckDistanceToPlayer() && canShoot)
            Shoot();
    }
    private void Shoot()
    {   
        canShoot = false;
       
        Vector3 shootingDirection = (target.transform.position - transform.position);
        shootingDirection.y = 0f;
        shootingDirection = shootingDirection.normalized; //projectile yonu

        //projectile'i instantiate et
        EnemyProjectile projectile = CreateProjectile();
        projectile.ApplyVelocity(shootingDirection * projectileSpeed); //projectile'a hiz ver
        StartCoroutine(EnableCooldown()); //saldiri cooldownunu baslat
    }
    private EnemyProjectile CreateProjectile()
    {
        EnemyProjectile projectile = _objectPool.GetPooledObject(1).GetComponent<EnemyProjectile>();
        projectile.transform.position = firePoint.transform.position;
        projectile.SetDamage(newEnemyStats.GetStat(EnemyStatType.Damage));

        return projectile;
    }
    private IEnumerator EnableCooldown()
    {
        yield return new WaitForSeconds(shootingCooldown); 
        canShoot = true;
    }
}

