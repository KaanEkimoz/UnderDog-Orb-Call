using com.game.player;
using com.game.player.statsystemextensions;
using UnityEngine;
using Zenject;
[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Walk")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float walkSpeed = 2.0f;
    [Header("Slow Walk")]
    [Tooltip("Slow walk speed of the character in m/s")]
    [SerializeField] private float slowWalkSpeed = 0.6f;
    [Header("Sprint")]
    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float sprintSpeed = 5.335f;
    [SerializeField] private bool alwaysSprint = false;
    [Header("Dash")]
    [Tooltip("Dash speed of the character in m/s")]
    [SerializeField] private float dashSpeed = 30.0f;
    [Tooltip("Dash duration of the character in seconds")]
    [SerializeField] private float dashDurationInSeconds = 0.21f;
    [Tooltip("Dash cooldown of the character in seconds")]
    [SerializeField] private float dashCooldownInSeconds = 1.5f;
    [Tooltip("Maximum dash count")]
    [SerializeField] private int maxDashCount = 2;
    [Header("Rotation")]
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [Header("Acceleration")]
    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float speedChangeRate = 10.0f;
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] footstepAudioClips;
    [Range(0, 1)][SerializeField] private float footstepAudioVolume = 0.5f;

    //player
    private float _currentHorizontalSpeed;
    private float _rotationVelocity;

    //dash
    private int _dashCount = 0;
    private float _dashCooldownTimer = 0;
    private float _dashDurationTimer = 0;

    public float DashCooldown => dashCooldownInSeconds;
    public float DashCooldownTimer => _dashCooldownTimer;

    public float DashDuration => dashDurationInSeconds;
    public float DashDurationTimer => _dashDurationTimer;

    public int MaxDashCount => maxDashCount;
    public int DashCount => _dashCount;

    //animation
    private bool _hasAnimator;
    private float _horizontalSpeedAnimationBlend;
    private int _animIDSpeed;
    private int _animIDMotionSpeed;

    //components
    private Animator _animator;
    private CharacterController _controller;
    private PlayerInputHandler _input;
    private GameObject _mainCamera;

    //Extras (Events, SFX, VFX, Achievements)
    private PlayerStats _playerStats;
    private SoundFXManager _soundFXManager;

    [Inject]
    private void ZenjectSetup(PlayerStats playerStats,SoundFXManager soundFXManager)
    {
        _playerStats = playerStats;
        _soundFXManager = soundFXManager;

        if (_playerStats == null)
            Debug.LogError("ThirdPersonController Zenject setup failed!! Player Stats is null");
        if (_soundFXManager == null)
            Debug.LogError("ThirdPersonController Zenject setup failed!! Player Stats is null");
    }
    private void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();

        walkSpeed *= _playerStats.GetStat(PlayerStatType.WalkSpeed);
        _dashCount = maxDashCount;

        AssignAnimationIDs();
    }
    private void Update()
    {
        HandleDash();
        HandleHorizontalMovement();
    }

    #region Horizontal Movement

    private void HandleHorizontalMovement()
    {
        float targetSpeed = CalculateMaximumSpeed();
        _currentHorizontalSpeed = CalculateCurrentSpeed(targetSpeed);

        _horizontalSpeedAnimationBlend = CalculateAnimationBlend(targetSpeed);

        Vector3 inputDirection = GetInputDirection();
        float targetRotation = CalculateTargetRotation(inputDirection);
        ApplyRotation(targetRotation);

        Vector3 targetDirection = CalculateTargetDirection(targetRotation);
        MovePlayer(targetDirection);

        UpdateAnimator();
    }
    private float CalculateMaximumSpeed()
    {
        if (_input.MovementInput == Vector2.zero)
            return 0.0f;

        if (alwaysSprint)
            return sprintSpeed;

        if (_input.SprintButtonHeld)
            return sprintSpeed;

        if(PlayerInputHandler.Instance.AttackButtonHeld)
            return slowWalkSpeed;

        return walkSpeed;
    }

    private float CalculateCurrentSpeed(float targetSpeedThisFrame)
    {
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.MovementInput.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeedThisFrame - speedOffset ||
            currentHorizontalSpeed > targetSpeedThisFrame + speedOffset)
        {
            float newSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeedThisFrame * inputMagnitude, Time.deltaTime * speedChangeRate);
            return Mathf.Round(newSpeed * 1000f) / 1000f;
        }

        if (_dashDurationTimer > 0)
            return dashSpeed;

        return targetSpeedThisFrame;
    }

    private float CalculateAnimationBlend(float targetSpeed)
    {
        float blend = Mathf.Lerp(_horizontalSpeedAnimationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        return blend < 0.01f ? 0f : blend;
    }

    private Vector3 GetInputDirection()
    {
        return new Vector3(_input.MovementInput.x, 0.0f, _input.MovementInput.y).normalized;
    }

    private float CalculateTargetRotation(Vector3 inputDirection)
    {
        if (_input.MovementInput == Vector2.zero )
            return transform.eulerAngles.y;

        return Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
    }

    private void ApplyRotation(float targetRotation)
    {
        if (PlayerInputHandler.Instance.AttackButtonHeld)
            return;

        targetRotation = Mathf.Clamp(targetRotation, -360f, 360f);

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity, rotationSmoothTime);
        Quaternion rotationQuat = Quaternion.Euler(0.0f, rotation, 0.0f);

        rotationQuat = rotationQuat.normalized;

        // Apply the validated rotation
        transform.rotation = rotationQuat;
    }

    private Vector3 CalculateTargetDirection(float targetRotation)
    {
        return Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
    }
    private void MovePlayer(Vector3 targetDirection)
    {
        _controller.Move(targetDirection.normalized * (_currentHorizontalSpeed * Time.deltaTime));
    }
    #endregion

    #region Dash
    private void HandleDash()
    {
        HandleDashTimers();

        if (!PlayerInputHandler.Instance.DashButtonPressed) return;

        if (CanDash())
            StartDash();
    }
    private bool CanDash()
    {
        return _dashCount > 0 && _dashDurationTimer <= 0;
    }
    private void StartDash()
    {
        _dashCount--;
        _dashDurationTimer = dashDurationInSeconds;
        _dashCooldownTimer = dashCooldownInSeconds + dashDurationInSeconds;
    }
    private void HandleDashTimers()
    {
        _dashCooldownTimer = Mathf.Max(0, _dashCooldownTimer - Time.deltaTime);
        _dashDurationTimer = Mathf.Max(0, _dashDurationTimer - Time.deltaTime);

        if (_dashCooldownTimer <= 0)
            _dashCount = maxDashCount;
    }
    #endregion

    #region Animation
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
    private void UpdateAnimator()
    {
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _horizontalSpeedAnimationBlend);
            _animator.SetFloat(_animIDMotionSpeed, _input.analogMovement ? _input.MovementInput.magnitude : 1f);
        }
    }
    #endregion

    #region Animation Events
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, footstepAudioClips.Length);
                _soundFXManager.PlayRandomSoundFXAtPosition(footstepAudioClips, transform, footstepAudioVolume);
            }
        }
    }
    #endregion
}