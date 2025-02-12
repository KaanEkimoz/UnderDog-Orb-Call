using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace com.game
{
    public class Parry : MonoBehaviour
    {
        [Header("Settings")]
        public bool activateShieldTimer = false;
        public bool activateCooldown = false;
        [Header("Shield")]
        [SerializeField] private GameObject shieldOnPlayer;
        [SerializeField] private float parryShieldDurationInSeconds = 1f;
        [SerializeField] private float parryShieldCooldownInSeconds = 2.0f;

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

        private float _shieldCooldownTimer = 0;

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
            HandleCooldownTimer();
            if (PlayerInputHandler.Instance.ParryButtonPressed && !isShieldActive)
                EnableShield();

            if (isShieldActive)
            {
                //TO DO:
            }

            if (PlayerInputHandler.Instance.ParryButtonReleased && isShieldActive)
                StartCoroutine(nameof(ShieldDisableEffectsCoroutine));
        }
        private void HandleCooldownTimer()
        {
            _shieldCooldownTimer = Mathf.Max(0, _shieldCooldownTimer - Time.deltaTime);
        }
        private void EnableShield()
        {
            if(_shieldCooldownTimer > 0)
                return;

            _shieldCooldownTimer = parryShieldCooldownInSeconds;
            shieldOnPlayer.SetActive(true);
            isShieldActive = true;
            StartCoroutine(nameof(ShieldEnableEffectsCoroutine));
            OnShieldEnable?.Invoke();
        }
        private IEnumerator ShieldDisableCoroutine()
        {
            yield return new WaitForSeconds(parryShieldDurationInSeconds);
            DisableShield();
        }
        private IEnumerator ShieldEnableEffectsCoroutine()
        {
            //shieldCollider.enabled = false;
            //shieldAnimator.SetTrigger("Enable");
            yield return new WaitForSeconds(parryShieldDurationInSeconds);
            DisableShield();
        }
        private IEnumerator ShieldDisableEffectsCoroutine()
        {
            
            //shieldAnimator.SetTrigger("Disable");
            yield return new WaitForSeconds(parryShieldDurationInSeconds);
            DisableShield();
        }
        private void DisableShield()
        {
            isShieldActive = false;
            shieldOnPlayer.SetActive(false);
            OnShieldDisable?.Invoke();
        }
    }
}
