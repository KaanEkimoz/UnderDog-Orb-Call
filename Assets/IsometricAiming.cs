using UnityEngine;
using UnityEngine.InputSystem;
public class IsometricAiming : MonoBehaviour
{
    [Header("Cursor Detection")]
    [SerializeField] private LayerMask cursorDetectMask;
    private Camera _mainCamera;
    private Vector3 _hitPoint;

    private void Start()
    {
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        HandleAiming();
    }
    private void HandleAiming()
    {
        if (!PlayerInputHandler.Instance.AttackButtonHeld) return;

        if (TryGetMouseWorldPosition(out var targetPosition))
            AimAtTarget(targetPosition);
    }
    private bool TryGetMouseWorldPosition(out Vector3 position)
    {
        var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, cursorDetectMask))
        {
            position = hitInfo.point;
            _hitPoint = hitInfo.point;
            return true;
        }

        position = Vector3.zero;
        return false;
    }
    private void AimAtTarget(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        direction.y = 0; // Keep the direction on the XZ plane
        transform.forward = direction;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_hitPoint, 0.5f);
    }
#endif
}