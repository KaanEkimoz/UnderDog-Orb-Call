using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class PlayerInputHandler : MonoBehaviour
	{
        #region Singleton
        public static PlayerInputHandler Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        #endregion 
        public bool DashButtonPressed { get { return IsDashButtonPressedThisFrame(); } }
        public bool DashButtonHeld { get { return _dashButtonHeld; } }
        public bool AttackButtonPressed { get { return IsAttackButtonPressedThisFrame(); } }
        public bool AttackButtonReleased { get { return IsAttackButtonReleasedThisFrame(); } }
        public bool AttackButtonHeld { get { return _attackButtonHeld; } }
        public bool BlockButtonPressed { get { return IsBlockButtonPressedThisFrame(); } }
        public bool SprintButtonHeld { get { return _sprintButtonHeld; } }
        public Vector2 MovementInput { get { return _moveInput; } }
        public Vector2 MouseInput { get { return _mouseInput; } }

        //Movement
        private Vector2 _moveInput;
        private Vector2 _mouseInput;

        //Jump
        private bool _dashButtonHeld;
        private bool _dashButtonPressedThisFrame;

        //Sprint
        private bool _sprintButtonHeld;

        //Attack
        private bool _attackButtonPressedThisFrame;
		private bool _attackButtonReleasedThisFrame;
        private bool _attackButtonHeld;

        //Block
        private bool _blockButtonPressedThisFrame;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
        
        private bool IsAttackButtonPressedThisFrame()
        {
            if (_attackButtonPressedThisFrame)
            {
                _attackButtonPressedThisFrame = false;
                return true;
            }
            return false;
        }
        private bool IsAttackButtonReleasedThisFrame()
        {
            if (_attackButtonReleasedThisFrame)
            {
                _attackButtonReleasedThisFrame = false;
                return true;
            }
            return false;
        }
        private bool IsBlockButtonPressedThisFrame()
        {
            if (_blockButtonPressedThisFrame)
            {
                _blockButtonPressedThisFrame = false;
                return true;
            }
            return false;
        }
		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
        private bool IsDashButtonPressedThisFrame()
        {
            if (_dashButtonPressedThisFrame)
            {
                _dashButtonPressedThisFrame = false;
                return true;
            }
            return false;
        }
        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

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
        public void OnBlock(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                _blockButtonPressedThisFrame = true;

        }

    }
    #endregion

}