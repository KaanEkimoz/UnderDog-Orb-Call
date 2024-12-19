using System.Collections.Generic;
using UnityEngine;
public class OrbController : MonoBehaviour
{
    [Header("Orb")]
    [SerializeField] private GameObject orbPrefab;
    [Space]
    [Header("Orb Spawn")]
    [Range(5, 15)] [SerializeField] private int maxOrbCount = 10;
    [SerializeField] private float ellipseXRadius = 0.5f;
    [SerializeField] private float ellipseYRadius = 0.75f;
    [Space]
    [Header("Orb Movement")]
    [SerializeField] private float orbMovementSpeed = 5f; // Speed at which orbs follow the player
    [SerializeField] private float orbFollowDelay = 0.3f;
    [SerializeField] private float rotationSpeed = 5f; // Delay for the orbs to follow the player
    [SerializeField] private Transform ellipseCenterTransform;
    [Space]
    [Header("Orb Sway")]
    [SerializeField] private float swayRange = 0.1f;
    [SerializeField] private float swaySpeed = 2f;
    [SerializeField] private float distanceThreshold = 0.05f;
    [Space]

    private List<GameObject> orbs = new();
    private List<Vector3> orbInitialPositions = new();
    private List<float> swayOffsets = new();
    private List<bool> hasReachedTarget = new();

    private void Update()
    {
        UpdateOrbPositions();
        UpdateOrbParentPosition();

    }
    private void LateUpdate()
    {
        
    }
    private void UpdateOrbParentPosition()
    {
        Vector3 targetPosition = ellipseCenterTransform.position;
        Quaternion targetRotation = ellipseCenterTransform.rotation;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * orbMovementSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    public void AddOrb()
    {
        GameObject newOrb = CreateOrb();
        newOrb.transform.SetParent(transform);
        orbs.Add(newOrb);
        swayOffsets.Add(Random.Range(0f, Mathf.PI * 2)); // Assign a random phase for sway
        hasReachedTarget.Add(false);
        UpdateOrbInitialPositions();
    }
    private GameObject CreateOrb()
    {
        return Instantiate(orbPrefab, ellipseCenterTransform.position, Quaternion.identity);
    }
    public void RemoveOrb()
    {
        if (orbs.Count > 0)
        {
            int lastIndex = orbs.Count - 1;
            GameObject orbToRemove = orbs[lastIndex];
            orbs.RemoveAt(lastIndex);
            swayOffsets.RemoveAt(lastIndex);
            hasReachedTarget.RemoveAt(lastIndex);
            Destroy(orbToRemove);
            UpdateOrbInitialPositions();
        }
    }
    private void UpdateOrbInitialPositions()
    {
        orbInitialPositions.Clear();

        float angleStep = 360f / Mathf.Max(1, orbs.Count); //Angle Between Orbs

        for (int i = 0; i < orbs.Count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 targetPosition = new Vector3(Mathf.Cos(angle) * ellipseXRadius, Mathf.Sin(angle) * ellipseYRadius, 0f);
            orbInitialPositions.Add(targetPosition);
        }
    }
    private void UpdateOrbPositions()
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            if (!hasReachedTarget[i])
            {
                // Move towards target position
                Vector3 targetPosition = orbInitialPositions[i];
                orbs[i].transform.localPosition = Vector3.Lerp(orbs[i].transform.localPosition, targetPosition, Time.deltaTime * orbMovementSpeed);

                // Check if the orb has reached the target
                if (Vector3.Distance(orbs[i].transform.localPosition, targetPosition) < distanceThreshold)
                {
                    hasReachedTarget[i] = true; // Mark as reached
                }
            }
            else
            {
                // Perform sway movement
                Vector3 basePosition = orbInitialPositions[i];
                float sway = Mathf.Sin(Time.time * swaySpeed + swayOffsets[i]) * swayRange;
                Vector3 swayPosition = new Vector3(basePosition.x, basePosition.y + sway, basePosition.z);

                orbs[i].transform.localPosition = Vector3.Lerp(orbs[i].transform.localPosition, swayPosition, Time.deltaTime * orbMovementSpeed);
            }
        }
    }
}