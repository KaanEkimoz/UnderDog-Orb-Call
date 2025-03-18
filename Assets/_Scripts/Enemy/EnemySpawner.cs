using com.game.enemysystem;
using com.game.player;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] spawnPoints;

    public float spawnDelay = 1f;
    public int maxEnemyCount = 30;

    private int enemyCount = 0;

    private bool isCoroutineRunning;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private void Update() {

        int EnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount < maxEnemyCount && !isCoroutineRunning)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        isCoroutineRunning = true;

        while (enemyCount < maxEnemyCount)
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            GameObject enemyPrefab = GetRandomEnemyPrefab();

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemy.GetComponentInChildren<EnemyCombatant>().ProvidePlayerCombatant(Player.Instance.Hub.Combatant);
            enemyCount++;

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
}


