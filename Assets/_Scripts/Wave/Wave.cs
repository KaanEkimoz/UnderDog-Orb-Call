using UnityEngine;

[System.Serializable]
 public class Wave
 {
    public string waveName = "Wave";

    [Header("Wave Settings")]
    public float waveDuration = 60f;

    [Header("Spawn Settings")]

    [Range(1, 50)]
    public int maxEnemyCount = 15;
    [Range(1, 10)]
    public float spawnDelay = 1.0f;
 }
    

