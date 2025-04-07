using com.game;
using com.game.enemysystem;
using com.game.player;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public event Action OnEnemiesCleared;

    public bool IsSpawning => spawnCoroutine != null;

    private void Start()
    {
        StartSpawning();
    }

    private void Update() {}

    IEnumerator SpawnEnemies()
    {
        isCoroutineRunning = true;

        while (enemyCount < maxEnemyCount)
        {
            yield return new WaitForSeconds(spawnDelay);

            Transform spawnPoint = GetRandomSpawnPoint();
            GameObject enemyPrefab = GetRandomEnemyPrefab();

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            EnemyCombatant enemyCombatant = enemy.GetComponentInChildren<EnemyCombatant>();
            enemyCombatant.ProvidePlayerCombatant(Player.Instance.Hub.Combatant);
            enemyCount++;
            spawnedEnemies.Add(enemy);
            if (enemyCombatant != null)
            {
                enemyCombatant.OnDie += OnEnemyDeath;
            }
        }

        isCoroutineRunning = false;
    }

    public GameObject GetRandomEnemyPrefab()
    {
        int randomEnemyIndex = UnityEngine.Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomEnemyIndex];

        return enemyPrefab;
    }

    public Transform GetRandomSpawnPoint()
    {
        int randomSpawnPoint = UnityEngine.Random.Range(0, spawnPoints.Length);
        GameObject spawnPoint = spawnPoints[randomSpawnPoint];

        return spawnPoint.transform;
    }
    private void OnEnemyDeath(DeathCause cause)
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

        OnEnemiesCleared?.Invoke();
    }
}


