using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace com.game
{
    public class Parry : MonoBehaviour
    {
        [Header("Shield")]
        [SerializeField] private GameObject shieldOnPlayer;
        [SerializeField] private float shieldEffectsTimer = 0.75f;

        [Header("Reflection Settings")]
        [SerializeField] private float reflectAngleOffset = 0f; // Adjustable reflection angle
        [SerializeField] private float reflectSpeedMultiplier = 1.5f; // Speed multiplier after reflection

        [Space]
        [Header("Events")]
        public UnityEvent OnShieldEnable;
        public UnityEvent OnShieldDisable;

        private SphereCollider shieldCollider;
        private Animator shieldAnimator;
        private bool isShieldActive = false;
        private Camera mainCamera;

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (shieldOnPlayer == null)
                Debug.LogError("Shield GameObject is not assigned in the inspector!");

            //shieldCollider = shieldOnPlayer.GetComponent<SphereCollider>();
            //shieldAnimator = shieldOnPlayer.GetComponent<Animator>();

            DisableShield();
        }
        void Update()
        {
            if (PlayerInputHandler.Instance.ParryButtonPressed && !isShieldActive)
                EnableShield();

            if (isShieldActive)
            {
                //TO DO:
            }

            if (PlayerInputHandler.Instance.ParryButtonReleased && isShieldActive)
                StartCoroutine(nameof(ShieldDisableEffectsCoroutine));
        }
        private void EnableShield()
        {
            shieldOnPlayer.SetActive(true);
            isShieldActive = true;
            StartCoroutine(nameof(ShieldEnableEffectsCoroutine));
            OnShieldEnable?.Invoke();
        }
        private IEnumerator ShieldEnableEffectsCoroutine()
        {
            //shieldCollider.enabled = false;
            //shieldAnimator.SetTrigger("Enable");
            yield return new WaitForSeconds(shieldEffectsTimer);
            //shieldCollider.enabled = true;
        }
        private IEnumerator ShieldDisableEffectsCoroutine()
        {
            
            //shieldAnimator.SetTrigger("Disable");
            yield return new WaitForSeconds(shieldEffectsTimer);
            DisableShield();
        }
        private void DisableShield()
        {
            shieldOnPlayer.SetActive(false);
            isShieldActive = false;
            OnShieldDisable?.Invoke();
        }
    }
}
