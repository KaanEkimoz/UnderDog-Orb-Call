using com.game.enemysystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] spawnPoints;

    public float spawnDelay = 1f;
    public int maxEnemyCount = 30;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private int enemyCount = 0;

    private bool isCoroutineRunning;

    private Coroutine spawnCoroutine;

    private void Start()
    {
        //StartSpawning();
    }

    private void Update() {}

    IEnumerator SpawnEnemies()
    {
        isCoroutineRunning = true;

        while (enemyCount < maxEnemyCount)
        {
            int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randomSpawnPoint];

            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyPrefab = enemyPrefabs[randomEnemyIndex];

            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            
            enemyCount++;
            spawnedEnemies.Add(spawnedEnemy);

            EnemyCombatant enemyCombatant = spawnedEnemy.GetComponent<EnemyCombatant>();
            
            if (enemyCombatant != null)
            {
                enemyCombatant.OnDie += OnEnemyDeath;
                
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        isCoroutineRunning = false;
    }

    private void OnEnemyDeath()
    {
        enemyCount--;
        Debug.Log("dusman oldu");

        if (enemyCount < maxEnemyCount && !isCoroutineRunning)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    public void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;          
        }
    }

    public void ClearEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();
        enemyCount = 0;       
    }
}


