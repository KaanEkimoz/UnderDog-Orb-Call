using System.Collections.Generic;
using UnityEngine;
public class OrbController : MonoBehaviour
{
    [Header("Orb")]
    public GameObject orbPrefab; // Orb prefab
    [Space]
    [Header("Orb Spawn")]
    public Transform ellipseCenterTransform; // Center of the ellipse
    //public int maxOrbCount = 10; // Maximum number of orbs
    public float verticalRadius = 0.75f; // Vertical radius of the ellipse
    public float horizontalRadius = 0.5f; // Horizontal radius of the ellipse
    [Space]
    [Header("Orb Follow")]
    public float orbMovementSpeed = 5f; // Speed at which orbs follow the player
    public float orbFollowDelay = 0.3f; // Delay for the orbs to follow the player
    public Transform playerTransform; // Player reference
    [Space]

    private List<GameObject> orbs = new List<GameObject>();

    void Update()
    {
        UpdateOrbPositions();
    }

    // Orb ekleme
    public void AddOrb()
    {
        GameObject newOrb = Instantiate(orbPrefab, ellipseCenterTransform.position, Quaternion.identity);
        newOrb.transform.SetParent(ellipseCenterTransform); // Orb'larý player ile birlikte hareket ettir
        orbs.Add(newOrb);
        UpdateOrbPositions();
    }

    // Orb eksiltme
    public void RemoveOrb()
    {
        if (orbs.Count > 0)
        {
            GameObject orbToRemove = orbs[orbs.Count - 1];
            orbs.Remove(orbToRemove);
            Destroy(orbToRemove);
            UpdateOrbPositions();
        }
    }

    // Oval üzerindeki pozisyonlarý güncelle
    private void UpdateOrbPositions()
    {
        float angleStep = 360f / Mathf.Max(1, orbs.Count); // Her orb için açý aralýðý
        for (int i = 0; i < orbs.Count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad; // Açýyý radyana çevir
            Vector3 newPosition = new Vector3(
                Mathf.Cos(angle) * horizontalRadius, // X ekseni için elips yarýçapý
                Mathf.Sin(angle) * verticalRadius, // Y ekseni için elips yarýçapý
                0f); // Z ekseni sabit
            orbs[i].transform.localPosition = newPosition;
        }
    }
}