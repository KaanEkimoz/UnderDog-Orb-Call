using com.game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerInputHandler : MonoBehaviour
{
    #region Singleton
    public static PlayerInputHandler Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        Game.OnResume += OnGameResume;
    }
    #endregion
    public bool DashButtonPressed => _dashButtonPressedThisFrame;
    public bool DashButtonHeld => _dashButtonHeld;
    public bool AttackButtonPressed => _attackButtonPressedThisFrame;
    public bool AttackButtonReleased => _attackButtonReleasedThisFrame;
    public bool AttackButtonHeld => _attackButtonHeld;
    public bool ParryButtonPressed => _parryButtonPressedThisFrame;
    public bool ParryButtonReleased => _parryButtonReleasedThisFrame;
    public bool RecallButtonPressed => _recallButtonPressedThisFrame;
    public bool RecallButtonReleased => _recallButtonReleasedThisFrame;
    public bool RecallButtonPerformed => _recallButtonPerformedThisFrame;
    public bool NextChooseButtonPressed => _nextChooseButtonPressedThisFrame;
    public bool PreviousChooseButtonPressed => _previousChooseButtonPressedThisFrame;
    public bool SprintButtonHeld => _sprintButtonHeld;
    public bool ClosestRecallButtonPressed => _closestRecallButtonPressedThisFrame;

    public Vector2 MovementInput => _moveInput;
    public Vector2 MouseInput => _mouseInput;

    // Movement - WASD Keyboard Buttons, Mouse Cursor
    private Vector2 _moveInput;
    private Vector2 _mouseInput;

    // Dash - Space Keyboard Button
    private bool _dashButtonHeld;
    private bool _dashButtonPressedThisFrame;

    // Sprint - Left Shift
    private bool _sprintButtonHeld;

    // Attack - Left Mouse Button
    private bool _attackButtonPressedThisFrame;
    private bool _attackButtonReleasedThisFrame;
    private bool _attackButtonHeld;

    //Parry - Right Mouse Button
    private bool _parryButtonPressedThisFrame;
    private bool _parryButtonReleasedThisFrame;

    // Recall - R Keyboard Button
    private bool _recallButtonPressedThisFrame;
    private bool _recallButtonReleasedThisFrame;
    private bool _recallButtonPerformedThisFrame;

    // Closest Recall - F Button
    private bool _closestRecallButtonPressedThisFrame;

    // Choose - Q and E Keyboard Buttons
    private bool _nextChooseButtonPressedThisFrame;
    private bool _previousChooseButtonPressedThisFrame;

    [Header("Movement Settings")]
    public bool analogMovement;

    private void OnGameResume()
    {
        ResetInputFlags();
    }
    private void LateUpdate()
    {
        ResetInputFlags();
    }
    private void ResetInputFlags()
    {
        _attackButtonPressedThisFrame = false;
        _attackButtonReleasedThisFrame = false;
        _dashButtonPressedThisFrame = false;
        _recallButtonPressedThisFrame = false;
        _recallButtonReleasedThisFrame = false;
        _nextChooseButtonPressedThisFrame = false;
        _previousChooseButtonPressedThisFrame = false;
        _parryButtonPressedThisFrame = false;
        _parryButtonReleasedThisFrame = false;
        _recallButtonPerformedThisFrame = false;
        _closestRecallButtonPressedThisFrame = false;
    }

    #region Mouse Cursor

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }
    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
    public Vector3 GetMouseWorldPosition(LayerMask cursorDetectMask)
    {
        if (Camera.main == null)
            return Vector3.zero;

        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, cursorDetectMask))
            return hitInfo.point;

        return Vector3.zero;
    }

    #endregion

    #region Input Functions

    public void OnMovement(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _dashButtonPressedThisFrame = true;
            _dashButtonHeld = true;
        }
        else if (context.canceled)
            _dashButtonHeld = false;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            _sprintButtonHeld = true;
        else if (context.canceled)
            _sprintButtonHeld = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _attackButtonHeld = true;
            _attackButtonPressedThisFrame = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _attackButtonHeld = false;
            _attackButtonReleasedThisFrame = true;
        }
    }

    public void OnRecall(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _recallButtonPressedThisFrame = true;
        }
        else if (context.performed)
        {
            _recallButtonPerformedThisFrame = true;
        }
        else if (context.canceled)
        {
            _recallButtonReleasedThisFrame = true;
        }

    }
    public void OnClosestRecall(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            _closestRecallButtonPressedThisFrame = true;
    }
    public void OnNextChoose(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            _nextChooseButtonPressedThisFrame = true;
    }

    public void OnPreviousChoose(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            _previousChooseButtonPressedThisFrame = true;
    }
    public void OnParry(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            _parryButtonPressedThisFrame = true;
        else if (context.phase == InputActionPhase.Canceled)
            _parryButtonReleasedThisFrame = true;
    }

    #endregion
}
