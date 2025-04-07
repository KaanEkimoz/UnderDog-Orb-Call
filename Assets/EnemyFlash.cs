using UnityEngine;

namespace com.game
{
    public class EnemyFlash : MonoBehaviour
    {
        [Header("Emission Settings")]
        [SerializeField] private Renderer enemyRenderer;
        [SerializeField] private string emissionProperty = "_Emission";
        [SerializeField] private float maxEmissionValue = 2f;
        [SerializeField] private float flashDuration = 0.5f;

        [Header("Timer Settings")]
        [SerializeField] private float minTime = 2f;
        [SerializeField] private float maxTime = 5f;

        private Material material;
        private float timer;
        private bool isFlashing;
        private float flashTimer;
        private int flashPhase; // 0 = not flashing, 1 = increasing, 2 = decreasing

        private void Start()
        {
            material = enemyRenderer.material;
            timer = GetRandomTime();
        }

        private void Update()
        {
            if (isFlashing)
            {
                HandleFlash();
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    StartFlash();
                }
            }
        }

        private void StartFlash()
        {
            isFlashing = true;
            flashTimer = 0f;
            flashPhase = 1; // Start increasing
        }

        private void HandleFlash()
        {
            flashTimer += Time.deltaTime;
            float t = flashTimer / (flashDuration / 2f);

            if (flashPhase == 1) // Increasing
            {
                material.SetFloat(emissionProperty, Mathf.Lerp(0, maxEmissionValue, t));
                if (flashTimer >= flashDuration / 2f)
                {
                    flashPhase = 2;
                    flashTimer = 0f;
                }
            }
            else if (flashPhase == 2) // Decreasing
            {
                material.SetFloat(emissionProperty, Mathf.Lerp(maxEmissionValue, 0, t));
                if (flashTimer >= flashDuration / 2f)
                {
                    isFlashing = false;
                    timer = GetRandomTime();
                }
            }
        }

        private float GetRandomTime()
        {
            return Random.Range(minTime, maxTime);
        }
    }
}
