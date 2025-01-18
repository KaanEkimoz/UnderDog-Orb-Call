using UnityEngine;
public class Orb : MonoBehaviour
{
    [Header("Orb Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [Header("Orb Positions")]
    [HideInInspector] private Vector3 currentTargetPos;
    [SerializeField] private float distanceThreshold = 0.05f;
    [Space]
    [Header("Orb Sway")]
    [SerializeField] private float swayRange = 0.1f;
    [SerializeField] private float swayOffset;
    [SerializeField] private float swaySpeed = 2f;
    [Space]
    /*
    [Header("Orb Throw")]
    [SerializeField] Transform throwPoint;
    [SerializeField] private float throwSpeed = 0.1f;
    [SerializeField] private float maxDistance = 2f;
    */
    //Flags
    [HideInInspector] public bool hasReachedTargetPos = false;

    [SerializeField] private bool isSwaying = false;
    [SerializeField] private bool isSticked = false;
    [SerializeField] private bool isThrowing = false;
    [SerializeField] private bool isReturning = false;

     private Transform startParent;
    private Rigidbody rb;
    [SerializeField] private SphereCollider sphereCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        swayOffset = Random.Range(0f, Mathf.PI * 2);
        startParent = transform.parent;
    }
    private void Update()
    {
        if(!isSticked && !isThrowing)
            MoveTargetPos();
        if(isReturning && hasReachedTargetPos)
        {
            isReturning = false;
            sphereCollider.isTrigger = false;
        }
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    public void Return()
    {
        sphereCollider.isTrigger = true;
        rb.isKinematic = true;
        isSticked = false;
        isReturning = true;
        
        isThrowing = false;
        
        ResetParent();
    }
    public void ResetParent()
    {
        transform.SetParent(startParent);
    }
    public void Throw(Vector3 force)
    {
        rb.isKinematic = false;
        isThrowing = true;
        rb.AddForce(force,ForceMode.Impulse);
    }
    public void SetNewDestination(Vector3 newPos)
    {
        currentTargetPos = newPos;
    }
    private void MoveTargetPos()
    {
        // Move towards target position
        Vector3 posToMove = currentTargetPos;
        transform.position = Vector3.Lerp(transform.position, posToMove, Time.deltaTime * movementSpeed);

        // Check if the orb has reached the target
        if (Vector3.Distance(transform.position, posToMove) < distanceThreshold)
            hasReachedTargetPos = true;
        else
            hasReachedTargetPos = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(isReturning)
            return;

        isSticked = true;
        rb.isKinematic = true;
        transform.position = collision.contacts[0].point;
        transform.SetParent(collision.transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            Debug.Log("Enemy Hit");
    }
    private void Sway()
    {
        Vector3 basePosition = currentTargetPos;
        float sway = Mathf.Sin(Time.time * swaySpeed + swayOffset) * swayRange;
        Vector3 swayPosition = new Vector3(basePosition.x, basePosition.y + sway, basePosition.z);

        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPosition, Time.deltaTime * movementSpeed);
    }
}
