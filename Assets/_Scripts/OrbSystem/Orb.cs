using UnityEngine;
public class Orb : MonoBehaviour
{
    [Header("Orb Movement")]
    [SerializeField] private float movementSpeed = 5f; // Speed at which orbs follow the player
    [Header("Orb Positions")]
    [HideInInspector] public Vector3 currentTargetPos;
    [HideInInspector] public Transform currentFollowTransform;
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
    [HideInInspector] public bool hasReachedTargetPos = false;
    [HideInInspector] public bool isIdleOnEllipse = false;
    private bool isSwaying = false;
    private bool isThrowing = false;

    private void OnEnable()
    {
        swayOffset = Random.Range(0f, Mathf.PI * 2);
        hasReachedTargetPos = false;
    }
    private void Update()
    {
        MoveTargetPos();
    }
    public void DisableOrb()
    {
        gameObject.SetActive(false);
    }
    private void Throw()
    {

    }
    public void SetNewDestination(Vector3 newPos)
    {
        currentFollowTransform = null;
        currentTargetPos = newPos;
        hasReachedTargetPos = false;
    }
    public void SetNewFollowTransform(Transform trans)
    {
        currentFollowTransform = trans;
    }
    private void MoveTargetPos()
    {
        // Move towards target position
        Vector3 posToMove = currentTargetPos;
        transform.position = Vector3.Lerp(transform.position, posToMove, Time.deltaTime * movementSpeed);

        // Check if the orb has reached the target
        //if (Vector3.Distance(transform.position, posToMove) < distanceThreshold)
          //  hasReachedTargetPos = true;
    }
    private void Sway()
    {
        Vector3 basePosition = currentTargetPos;
        float sway = Mathf.Sin(Time.time * swaySpeed + swayOffset) * swayRange;
        Vector3 swayPosition = new Vector3(basePosition.x, basePosition.y + sway, basePosition.z);

        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPosition, Time.deltaTime * movementSpeed);
    }
}
