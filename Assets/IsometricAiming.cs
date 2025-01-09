using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
public class IsometricAiming : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    private Camera mainCamera;
    public Vector3 hitPoint;

    private bool isAiming = false;

    public StarterAssetsInputs input;

    private void Start()
    {
        mainCamera = Camera.main;
        input = GetComponent<StarterAssetsInputs>();
    }
    private void Update()
    {
        if (input.AttackButtonHeld)
            Aim();
    }
    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            // Calculate the direction
            var direction = position - transform.position;

            // You might want to delete this line.
            // Ignore the height difference.
            direction.y = 0;

            // Make the transform look in the direction.
            transform.forward = direction;
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            hitPoint = hitInfo.point;
            return (success: true, position: hitInfo.point);
        }        
        else
            return (success: false, position: Vector3.zero);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hitPoint, 1f);
    }
}