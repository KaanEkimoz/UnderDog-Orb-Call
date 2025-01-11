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
            int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randomSpawnPoint];

            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyPrefab = enemyPrefabs[randomEnemyIndex];

            Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            enemyCount++;

            yield return new WaitForSeconds(spawnDelay);
        }
    }



}


