using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
public class IsometricAiming : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundMask;
    private Camera mainCamera;
    public Vector3 HitPoint { get; private set; }

    private void Start()
    {
        mainCamera = Camera.main;
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
        var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            position = hitInfo.point;
            HitPoint = hitInfo.point;
            return true;
        }

        position = Vector3.zero;
        return false;
    }
    private void AimAtTarget(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        direction.y = 0; // Yükseklik farkýný yok say
        transform.forward = direction;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(HitPoint, 0.5f);
    }

#endif
}