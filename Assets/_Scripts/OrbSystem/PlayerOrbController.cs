using com.game;
using com.game.player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class OrbController : MonoBehaviour
{
    [Header("Orb Count")]
    [Range(5, 15)][SerializeField] private int maximumOrbCount = 10;
    [Range(0, 10)][SerializeField] private int orbCountAtStart;
    [Space]
    [Header("Orb Throw")]
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private LayerMask cursorDetectMask;
    [Space]
    [Header("Ellipse Creation")]
    [SerializeField] private Transform ellipseCenterTransform;
    [SerializeField] private float ellipseXRadius = 0.5f;
    [SerializeField] private float ellipseYRadius = 0.75f;
    [Space]
    [Header("Ellipse Movement")]
    [SerializeField] private float ellipseMovementSpeed = 1.5f;
    [SerializeField] private float ellipseRotationSpeed = 5f;
    [Space]
    [Header("Components")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private ObjectPool objectPool;
    [Header("Orb Selection")]
    [SerializeField] private Material highlightMaterial;

    private List<SimpleOrb> orbsOnEllipse = new();

    //Orb Throw
    private SimpleOrb orbToThrow;
    private List<SimpleOrb> orbsThrowed = new();

    private Material startMaterial;

    //Flags
    private bool isAiming = false;
    private bool isReturning = false;
    private void Start()
    {
        orbCountAtStart = Player.Instance.CharacterProfile.OrbCount;

        if (objectPool == null)
            objectPool = GameObject.FindAnyObjectByType<ObjectPool>();

        if(input == null)
            input = GameObject.FindAnyObjectByType<PlayerInputHandler>();

        CreateOrbsAtStart();
    }
    private void Update()
    {
        if (Game.Paused) return;

        if(orbsOnEllipse.Count > 0)
            orbsOnEllipse[0].SetMaterial(highlightMaterial);

        if (input.AttackButtonPressed)
            Aim();
        else if(input.AttackButtonReleased)
            Throw();

        if (input.RecallButtonPressed)
            CallOrbs();

        if(PlayerInputHandler.Instance.NextChooseButtonPressed)
            ChooseNextOrb();

        if (PlayerInputHandler.Instance.PreviousChooseButtonPressed)
            ChoosePreviousOrb();

        if (orbToThrow != null)
        {
            orbToThrow.IncreaseSpeedForSeconds(15f,0.1f);
            orbToThrow.SetNewDestination(firePointTransform.position);
        }

       

        UpdateEllipsePos();
        UpdateOrbEllipsePositions();
    }
    private void CallOrbs()
    {
        if(orbsThrowed.Count == 0)
            return;

        for (int i = 0; i < orbsThrowed.Count; i++)
            CallOrb(orbsThrowed[i]);

        orbsOnEllipse[0].SetMaterial(highlightMaterial);
        orbsThrowed.Clear();
    }
    private void CallOrb(SimpleOrb orb)
    {
        orb.Return();
        Player.Instance.Hub.OrbHandler.AddOrb();
        AddOrbToList(orb);
    }
    private void ChooseNextOrb()
    {
        if (orbsOnEllipse.Count <= 1)
            return;

        SimpleOrb lastOrb = orbsOnEllipse[orbsOnEllipse.Count - 1];

        // Shifts all orbs to the right
        for (int i = orbsOnEllipse.Count - 1; i > 0; i--)
            orbsOnEllipse[i] = orbsOnEllipse[i - 1];

        orbsOnEllipse[0] = lastOrb;

        CheckMaterials();
    }
    private void ChoosePreviousOrb()
    {
        if (orbsOnEllipse.Count <= 1)
            return;

        SimpleOrb firstOrb = orbsOnEllipse[0];

        // Shifts all orbs to the left
        for (int i = 0; i < orbsOnEllipse.Count - 1; i++)
            orbsOnEllipse[i] = orbsOnEllipse[i + 1];

        orbsOnEllipse[orbsOnEllipse.Count - 1] = firstOrb;

        CheckMaterials();
    }
    private void CheckMaterials()
    {
        foreach (SimpleOrb orb in orbsOnEllipse)
        {
            if (orb == orbsOnEllipse[0])
                orb.SetMaterial(highlightMaterial);
            else
                orb.ResetMaterial();
        }
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

        orbToThrow = orbsOnEllipse[0];
        
        RemoveOrbFromList(orbToThrow);
    }
    private void Throw()
    {
        if (orbToThrow == null || !isAiming)
            return;

        isAiming = false;

        Vector3 throwDirection = GetMouseWorldPosition() - firePointTransform.position;
        orbToThrow.Throw(throwDirection.normalized * 20f);

        orbToThrow.ResetMaterial();

        if(orbsOnEllipse.Count > 0)
            orbsOnEllipse[0].SetMaterial(highlightMaterial);

        orbsThrowed.Add(orbToThrow);
        orbToThrow = null;

        Player.Instance.Hub.OrbHandler.RemoveOrb();
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
        SimpleOrb newOrb = objectPool.GetPooledObject(0).GetComponent<SimpleOrb>();
        newOrb.transform.position = ellipseCenterTransform.position;

        orbsOnEllipse.Add(newOrb);
        Player.Instance.Hub.OrbHandler.AddOrb();
        UpdateOrbEllipsePositions();
    }
    public void AddOrbToList(SimpleOrb orb)
    {
        orbsOnEllipse.Add(orb);
    }
    public void RemoveOrbFromList(SimpleOrb orb)
    {
        orbsOnEllipse.Remove(orb);
        UpdateOrbEllipsePositions();
    }
    private void UpdateOrbEllipsePositions()
    {
        if (orbsOnEllipse.Count == 0)
            return;

        for (int i = 0; i < orbsOnEllipse.Count; i++)
        {
            if (orbsOnEllipse[i] == orbToThrow)
                continue;

            // İlk topun her zaman en üstte olmasını sağla
            float angleOffset = 90f; // 0 derece üstte, 90 dereceyi sola kaydırır
            float angle = angleOffset + i * CalculateAngleBetweenOrbs();

            float angleInRadians = angle * Mathf.Deg2Rad;

            float localX = Mathf.Cos(angleInRadians) * ellipseXRadius;
            float localY = Mathf.Sin(angleInRadians) * ellipseYRadius;

            Vector3 localPosition = new Vector3(localX, localY, 0f);

            Vector3 rotatedPosition = ellipseCenterTransform.rotation * localPosition;
            Vector3 targetPosition = ellipseCenterTransform.position + rotatedPosition;

            orbsOnEllipse[i].SetNewDestination(targetPosition);
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, cursorDetectMask))
            return hitInfo.point;

        return Vector3.zero;
    }
    private float CalculateAngleBetweenOrbs()
    {
        float angleStep = 360f / Mathf.Max(1, orbsOnEllipse.Count);
        return angleStep;
    }
}