using com.game.enemysystem;
using com.game.player;
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
            Transform spawnPoint = GetRandomSpawnPoint();
            GameObject enemyPrefab = GetRandomEnemyPrefab();

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            EnemyCombatant enemyCombatant = enemy.GetComponent<EnemyCombatant>();
            enemyCombatant.ProvidePlayerCombatant(Player.Instance.Hub.Combatant);
            enemyCount++;
            spawnedEnemies.Add(enemy);
            if (enemyCombatant != null)
            {
                enemyCombatant.OnDie += OnEnemyDeath;
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        isCoroutineRunning = false;
    }

    public GameObject GetRandomEnemyPrefab()
    {
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomEnemyIndex];

        return enemyPrefab;
    }

    public Transform GetRandomSpawnPoint()
    {
        int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
        GameObject spawnPoint = spawnPoints[randomSpawnPoint];

        return spawnPoint.transform;
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


