using com.game.enemysystem;
using System.Collections;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] spawnPoints;

    public float spawnDelay = 1f;
    public int maxEnemyCount = 30;

    private int enemyCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (enemyCount < maxEnemyCount)
        {
            Transform spawnPoint = GetRandomSpawnPoint();
            GameObject enemyPrefab = GetRandomEnemyPrefab();

            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemyCount++;

            yield return new WaitForSeconds(spawnDelay);
        }
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


