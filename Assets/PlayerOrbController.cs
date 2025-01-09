using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
public class OrbController : MonoBehaviour
{
    
    [Header("Maximum Orb")]
    [Range(5, 15)][SerializeField] private int maxOrbCount = 10;
    [Range(0, 10)][SerializeField] private int orbCountAtStart;
    
    [Space]
    [Header("Fire Orb")]
    [SerializeField] private Transform firePointTransform;
    private Orb orbToFire;
    [Space]
    [Header("Ellipse Creation")]
    [SerializeField] private Transform ellipseCenterTransform;
    [SerializeField] private float ellipseXRadius = 0.5f;
    [SerializeField] private float ellipseYRadius = 0.75f;
    [Space]
    [Header("Ellipse Movement")]
    [SerializeField] private float ellipseMovementSpeed = 5f;
    [SerializeField] private float ellipseRotationSpeed = 5f;
    private bool isAiming = false;

    private ObjectPool objectPool;
    private List<Orb> orbs = new();
    private List<Vector3> orbPositions = new();

    [SerializeField] private StarterAssetsInputs input;

    private void Start()
    {
        objectPool = GameObject.FindAnyObjectByType<ObjectPool>();

        if (orbCountAtStart > 0)
        {
            for (int i = 0; i < orbCountAtStart; i++)
                AddOrb();
        }
    }
    private void Update()
    {
        if(input.AttackButtonPressed)
            ReadyOrb();
        else if(input.AttackButtonReleased)
            FireOrb();

        if (orbToFire != null)
            orbToFire.SetNewDestination(firePointTransform.position);

        UpdateEllipsePos();
        UpdateOrbInitialPositions();
    }

    private void ReadyOrb()
    {
        if (orbToFire != null)
            return;
        isAiming = true;

        int lastIndex = orbs.Count - 1;
        orbToFire = orbs[lastIndex];
        orbToFire.isIdleOnEllipse = false;
        RemoveOrbFromList();

    }
    private void FireOrb()
    {
        isAiming = false;
        orbToFire.SetNewDestination(firePointTransform.position + (firePointTransform.forward * 20f));
        //orbToFire.DisableOrb();
        orbToFire = null;
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

        orbs.Add(newOrb);
        newOrb.isIdleOnEllipse = true;
        UpdateOrbInitialPositions();
    }
    public void RemoveOrbFromList()
    {
        if (orbs.Count > 0)
        {
            int lastIndex = orbs.Count - 1;
            Orb orbToRemove = orbs[lastIndex];
            //orbToRemove.DisableOrb();
            orbs.RemoveAt(lastIndex);
            UpdateOrbInitialPositions();
        }
    }
    private void UpdateOrbInitialPositions()
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            if (orbs[i] == orbToFire)
                continue;

            float angle = i * CalculateAngleBetweenOrbs() * Mathf.Deg2Rad;

            // Elips üzerindeki yerel pozisyonu hesapla
            float localX = Mathf.Cos(angle) * ellipseXRadius;
            float localY = Mathf.Sin(angle) * ellipseYRadius;

            // Yerel pozisyonu dünya uzayýna hedef transform'un rotasyonuyla döndür
            Vector3 localPosition = new Vector3(localX, localY, 0f);
            Vector3 rotatedPosition = ellipseCenterTransform.rotation * localPosition;

            // Elipsin dünya uzayýndaki pozisyonunu hesapla
            Vector3 targetPosition = ellipseCenterTransform.position + rotatedPosition;
            
            orbs[i].currentTargetPos = targetPosition;
        }
    }
   /* private void UpdateOrbInitialPositions()
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            float angle = i * CalculateAngleBetweenOrbs() * Mathf.Deg2Rad;
            Vector3 targetPosition = transform.position + new Vector3(Mathf.Cos(angle) * ellipseXRadius, Mathf.Sin(angle) * ellipseYRadius, 0f);
            orbs[i].currentTargetPos = targetPosition;
        }
    }*/
    private float CalculateAngleBetweenOrbs()
    {
        float angleStep = 360f / Mathf.Max(1, orbs.Count);
        return angleStep;
    }
}