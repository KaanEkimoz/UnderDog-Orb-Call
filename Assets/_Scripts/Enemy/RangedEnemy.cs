using com.game.enemysystem.statsystemextensions;
using System.Collections;
using UnityEngine;
public class RangedEnemy : Enemy
{
    public GameObject projectilePrebfab;

    [Header("Shooting")]
    public float shootingCooldown = 3f;
    public Transform firePoint;
    [Header("Projectile")]
    public float projectileSpeed = 5f;
    [Header("Raycast")]
    public float rayDistance = 20f;
    public GameObject raySource;
    [SerializeField] LayerMask layerMask;
    [SerializeField] bool canSeePlayer;

    private bool canShoot = true;

    private ObjectPool _objectPool;
    protected override void Start()
    {
        base.Start();
        _objectPool = FindFirstObjectByType<ObjectPool>();
    }
    protected override void CustomUpdate()
    {
        navMeshAgent.SetDestination(target.transform.position);

        canSeePlayer = CanSeePlayer();

        if (canShoot && GetDistanceToPlayer() && CanSeePlayer())
            Shoot();

    }
    
    private bool CanSeePlayer()
    {
        Vector3 rayDirection = transform.TransformDirection(Vector3.forward); 
        Debug.DrawRay(raySource.transform.position, rayDirection * rayDistance, Color.blue); //ray'i default olarak mavi ciz

        if (Physics.Raycast(raySource.transform.position, rayDirection, out RaycastHit hitInfo, rayDistance, layerMask)) //ray bir seye carpti mi?
        {   
            bool isPlayer = hitInfo.collider.CompareTag("Player"); //carpilan nesne player mi?
            
            Debug.DrawRay(raySource.transform.position, rayDirection * rayDistance, isPlayer ? Color.green : Color.red ); //ray'in carptigi nesne player'sa yesil, degilse kirmizi ciz
           
            return isPlayer;
        }

        return false;   
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
        EnemyProjectile projectile = _objectPool.GetPooledObject(6).GetComponent<EnemyProjectile>();
        projectile.transform.position = firePoint.transform.position;
        projectile.SetDamage(enemyStats.GetStat(EnemyStatType.Damage));

        return projectile;
    }
    private IEnumerator EnableCooldown()
    {
        yield return new WaitForSeconds(shootingCooldown); 
        canShoot = true;
    }
}

