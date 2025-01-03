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

    private ObjectPool objectPool;
    private List<Orb> orbs = new();


    [SerializeField] private StarterAssetsInputs input;

    private void Start()
    {
        objectPool = GetComponent<ObjectPool>();
    }
    private void Update()
    {
        if(input.attack)
        {
            FireOrb();
        }
            
        UpdateEllipsePos();
    }

    private void FireOrb()
    {
        if (orbToFire != null)
            return;

        int lastIndex = orbs.Count - 1;
        orbToFire = orbs[lastIndex];
        orbToFire.isIdleOnEllipse = false;
        orbToFire.SetNewDestination(firePointTransform.position);
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
    public void RemoveOrb()
    {
        if (orbs.Count > 0)
        {
            int lastIndex = orbs.Count - 1;
            Orb orbToRemove = orbs[lastIndex];
            //orbToRemove.DisableOrb();
            //orbToRemove.SetActive(false);
            orbs.RemoveAt(lastIndex);
            UpdateOrbInitialPositions();
        }
    }
    private void UpdateOrbInitialPositions()
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            float angle = i * CalculateAngleBetweenOrbs() * Mathf.Deg2Rad;
            Vector3 targetPosition = new Vector3(Mathf.Cos(angle) * ellipseXRadius, Mathf.Sin(angle) * ellipseYRadius, 0f);
            orbs[i].currentTargetPos = targetPosition;
        }
    }
    private float CalculateAngleBetweenOrbs()
    {
        float angleStep = 360f / Mathf.Max(1, orbs.Count);
        return angleStep;
    }
}