using UnityEngine;
public class Orb : MonoBehaviour
{
    [Header("Orb Movement")]
    [SerializeField] private float movementSpeed = 5f; // Speed at which orbs follow the player
    [Header("Orb Positions")]
    [HideInInspector] public Vector3 posOnEllipse;
    [Space]
    [Header("Orb Sway")]
    [SerializeField] private float swayRange = 0.1f;
    [SerializeField] private float swayOffset;
    [SerializeField] private float swaySpeed = 2f;
    [SerializeField] private float distanceThreshold = 0.05f;
    [Space]

    /*
    [Header("Orb Throw")]
    [SerializeField] Transform throwPoint;
    [SerializeField] private float throwSpeed = 0.1f;
    [SerializeField] private float maxDistance = 2f;
    */
    //Flags
    [HideInInspector] public bool hasReachedEllipsePos = false;
    private bool isSwaying = false;
    private bool isThrowing = false;

    private void OnEnable()
    {
        swayOffset = Random.Range(0f, Mathf.PI * 2);
        hasReachedEllipsePos = false;
    }
    private void Update()
    {
        if (hasReachedEllipsePos)
            Sway();
        else
            MoveEllipsePos();
    }
    private void OnDisable()
    {
        
    }
    private void Throw()
    {

    }
    private void MoveEllipsePos()
    {
        // Move towards target position
        Vector3 targetPosition = posOnEllipse;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * movementSpeed);

        // Check if the orb has reached the target
        if (Vector3.Distance(transform.localPosition, targetPosition) < distanceThreshold)
            hasReachedEllipsePos = true;
    }
    private void Sway()
    {
        Vector3 basePosition = posOnEllipse;
        float sway = Mathf.Sin(Time.time * swaySpeed + swayOffset) * swayRange;
        Vector3 swayPosition = new Vector3(basePosition.x, basePosition.y + sway, basePosition.z);

        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPosition, Time.deltaTime * movementSpeed);
    }
}
