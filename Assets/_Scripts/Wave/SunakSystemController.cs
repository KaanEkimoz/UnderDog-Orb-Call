using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SunakSystemController : MonoBehaviour
{
    [Header("Sunak System Settings")]
    public float safeZoneTimer = 120f;
    public int maxSafeZoneEntries = 4;
    public float difficultyIncreaseTime = 30f;
    public int safeZoneEntryCount = 0;
    public int maxEnemyIncrease = 10;

    [Header("Refeences")]
    public GameObject safeZoneBoundaries;
    public GameObject safeZoneArea;
    public EnemySpawner enemySpawner;

    [Header("Bools")]
    [SerializeField]
    private bool isPlayerInSafeZone = true;
    private bool canEnterSafeZone = true;
    private bool isTimerActive = false;
    private float timer;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private void Start()
    {
        safeZoneBoundaries.SetActive(false);
        ResetTimer();

        if (isPlayerInSafeZone)
        {
            safeZoneEntryCount++;
        }
    }

    private void Update()
    {
        if (!isTimerActive) return;

        timer += Time.deltaTime;
        UpdateTimerUI();

        if (timer < safeZoneTimer) return;

        canEnterSafeZone = true;
        safeZoneBoundaries.SetActive(false);

        int difficultyLevel = (int)((timer - safeZoneTimer) / difficultyIncreaseTime);

        SetDifficulty(difficultyLevel);

    }

    public void SetDifficulty(int difficulty)
    {
        enemySpawner.maxEnemyCount += maxEnemyIncrease * difficulty;
        
        switch (difficulty) 
        {
            case 0:
                
            case 1:
                //todo : x dusmani arttir 
                break;

            case 2:                
                //todo : dusmanlari hizlandir
                break;

            case 3:               
                //todo : dusmanlari kuvvetlendir
                break;

            default:
                //idk
                break;
        }
    }
    public void PlayerEnteredSafeZone()
    {
        if (isPlayerInSafeZone) return;

        isPlayerInSafeZone = true;

        if (!canEnterSafeZone) return;

        enemySpawner.ClearEnemies();
        safeZoneEntryCount++;

        if (safeZoneEntryCount >= maxSafeZoneEntries) 
        {
            Debug.Log("Boss geliyor!");
        }

    }

    public void PlayerExitedSafeZone()
    {
        if (!isTimerActive)
        {
            isPlayerInSafeZone = false;
            StartTimer();         
            canEnterSafeZone = false;
            safeZoneBoundaries.SetActive(true);
            enemySpawner.StartSpawning();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("girdi");

        if (other.CompareTag("Player") && other.gameObject == safeZoneArea)
        {          
            PlayerEnteredSafeZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("cikti");

        if (other.CompareTag("Player") && other.gameObject == safeZoneArea)
        {         
            PlayerExitedSafeZone();  
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = minutes + " : " + seconds;
    }

    private void ResetTimer()
    {
        timer = 0f;
        isTimerActive = false;
    }

    private void StartTimer()
    {
        isTimerActive = true;
    }
}
