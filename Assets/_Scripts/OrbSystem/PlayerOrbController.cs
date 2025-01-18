using System.Collections.Generic;
using UnityEngine;
public class OrbController : MonoBehaviour
{
    [Header("Orb Count")]
    [Range(5, 15)][SerializeField] private int maximumOrbCount = 10;
    [Range(0, 10)][SerializeField] private int orbCountAtStart;
    [Space]
    [Header("Orb Throw")]
    [SerializeField] private Transform firePointTransform;
    [Space]
    [Header("Ellipse Creation")]
    [SerializeField] private Transform ellipseCenterTransform;
    [SerializeField] private float ellipseXRadius = 0.5f;
    [SerializeField] private float ellipseYRadius = 0.75f;
    [Space]
    [Header("Ellipse Movement")]
    [SerializeField] private float ellipseMovementSpeed = 5f;
    [SerializeField] private float ellipseRotationSpeed = 5f;
    [Space]
    [Header("Components")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private ObjectPool objectPool;


    private List<Orb> orbsOnEllipse = new();

    //Orb Throw
    private Orb orbToThrow;
    private List<Orb> orbsThrowed = new();

    //Flags
    private bool isAiming = false;
    private bool isReturning = false;

    private void Start()
    {
        if(objectPool == null)
            objectPool = GameObject.FindAnyObjectByType<ObjectPool>();

        if(input == null)
            input = GameObject.FindAnyObjectByType<PlayerInputHandler>();

        CreateOrbsAtStart();
    }
    private void Update()
    {
        if(input.AttackButtonPressed)
            Aim();
        else if(input.AttackButtonReleased)
            Throw();

        if (input.BlockButtonPressed)
            CallOrbs();

        if (orbToThrow != null)
            orbToThrow.SetNewDestination(firePointTransform.position);

        UpdateEllipsePos();
        UpdateOrbEllipsePositions();
    }
    private void CallOrbs()
    {
        for(int i = 0; i < orbsThrowed.Count; i++)
            CallOrb(orbsThrowed[i]);

        orbsThrowed.Clear();
    }
    private void CallOrb(Orb orb)
    {
        orb.Return();
        AddOrbToList(orb);
    }
    private void CreateOrbsAtStart()
    {
        if (orbCountAtStart <= 0)
            return;

        for (int i = 0; i < orbCountAtStart; i++)
            AddOrb();
    }

    private void Aim()
    {
        if (orbToThrow != null || orbsOnEllipse.Count == 0)
            return;

        isAiming = true;
        orbToThrow = orbsOnEllipse[orbsOnEllipse.Count - 1];
        RemoveOrbFromList(orbToThrow);
    }
    private void Throw()
    {
        if (orbToThrow == null)
            return;

        isAiming = false;
        orbToThrow.Throw(firePointTransform.forward * 20f);
        //orbToThrow.SetNewDestination(firePointTransform.position + (firePointTransform.forward * 20f));
        orbsThrowed.Add(orbToThrow);
        orbToThrow = null;
    }
    
    private void UpdateEllipsePos()
    {
        Vector3 targetPosition = ellipseCenterTransform.position;
        Quaternion targetRotation = ellipseCenterTransform.rotation;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * ellipseMovementSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * ellipseRotationSpeed);
    }
    public void AddOrb()
    {
        Orb newOrb = objectPool.GetPooledObject(0).GetComponent<Orb>();
        newOrb.transform.position = ellipseCenterTransform.position;

        orbsOnEllipse.Add(newOrb);
        UpdateOrbEllipsePositions();
    }
    public void AddOrbToList(Orb orb)
    {
        orbsOnEllipse.Add(orb);
    }
    public void RemoveOrbFromList(Orb orb)
    {
        orbsOnEllipse.Remove(orb);
        UpdateOrbEllipsePositions();
    }
    private void UpdateOrbEllipsePositions()
    {
        for (int i = 0; i < orbsOnEllipse.Count; i++)
        {
            if (orbsOnEllipse[i] == orbToThrow)
                continue;

            float angle = i * CalculateAngleBetweenOrbs() * Mathf.Deg2Rad;

            float localX = Mathf.Cos(angle) * ellipseXRadius;
            float localY = Mathf.Sin(angle) * ellipseYRadius;

            Vector3 localPosition = new Vector3(localX, localY, 0f);

            Vector3 rotatedPosition = ellipseCenterTransform.rotation * localPosition;
            Vector3 targetPosition = ellipseCenterTransform.position + rotatedPosition;
            
            orbsOnEllipse[i].SetNewDestination(targetPosition);
        }
    }
    private float CalculateAngleBetweenOrbs()
    {
        float angleStep = 360f / Mathf.Max(1, orbsOnEllipse.Count);
        return angleStep;
    }
}