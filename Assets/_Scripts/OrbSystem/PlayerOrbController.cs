using com.game;
using com.game.player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbController : MonoBehaviour
{
    [Header("Orb Count")]
    [Range(5, 15)][SerializeField] private int maximumOrbCount = 10;
    [Range(0, 10)][SerializeField] private int orbCountAtStart;

    [Header("Orb Throw")]
    [SerializeField] private float cooldownBetweenThrowsInSeconds = 2f;
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private LayerMask cursorDetectMask;

    [Header("Ellipse Creation")]
    [SerializeField] private Transform ellipseCenterTransform;
    [SerializeField] private float ellipseXRadius = 0.5f;
    [SerializeField] private float ellipseYRadius = 0.75f;

    [Header("Ellipse Movement")]
    [SerializeField] private float ellipseMovementSpeed = 1.5f;
    [SerializeField] private float ellipseRotationSpeed = 5f;

    [Header("Components")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private ObjectPool objectPool;

    [Header("Orb Materials")]
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material ghostMaterial;

    //Events
    public event Action OnOrbThrowed;
    public event Action OnOrbCalled;
    public event Action<int> OnOrbCountChanged;
    public event Action OnAllOrbsCalled;
    public event Action OnNextOrbSelected;
    public event Action OnPreviousOrbSelected;
    public event Action OnSelectedOrbChanged;
    public event Action OnOrbAdded;

    //Orbs
    private List<SimpleOrb> orbsOnEllipse = new();
    private List<SimpleOrb> orbsThrowed = new();
    private SimpleOrb orbToThrow;

    //Cooldown
    private float throwCooldownTimer;

    public List<SimpleOrb> OrbsOnEllipse => orbsOnEllipse;

    //Flags
    private bool isAiming = false;

    public bool IsAiming => isAiming;

    private void Start()
    {
        orbCountAtStart = Player.Instance.CharacterProfile.OrbCount;

        if (objectPool == null)
            objectPool = FindAnyObjectByType<ObjectPool>();

        if (input == null)
            input = FindAnyObjectByType<PlayerInputHandler>();

        CreateOrbsAtStart();
    }

    private void Update()
    {
        if (Game.Paused) return;

        HandleInput();
        HandleCooldowns();
        UpdateEllipsePosition();
        UpdateOrbEllipsePositions();
    }

    private void HandleInput()
    {
        if (input.AttackButtonPressed)
            Aim();
        else if (input.AttackButtonReleased)
            Throw();

        if (input.RecallButtonPressed)
            CallOrbs();

        if (PlayerInputHandler.Instance.NextChooseButtonPressed)
            ChooseNextOrb();

        if (PlayerInputHandler.Instance.PreviousChooseButtonPressed)
            ChoosePreviousOrb();

        if (orbToThrow != null)
        {
            orbToThrow.IncreaseSpeedForSeconds(15f, 0.1f);
            orbToThrow.SetNewDestination(firePointTransform.position);
        }
    }

    private void CallOrbs()
    {
        if (orbsThrowed.Count == 0) return;

        foreach (var orb in orbsThrowed)
            CallOrb(orb);

        orbsThrowed.Clear();
        OnAllOrbsCalled?.Invoke();
    }

    private void CallOrb(SimpleOrb orb)
    {
        orb.Return();
        Player.Instance.Hub.OrbHandler.AddOrb();
        AddOrbToList(orb);
        OnOrbCalled?.Invoke();
    }

    private void ChooseNextOrb()
    {
        if (orbsOnEllipse.Count <= 1) return;

        var lastOrb = orbsOnEllipse[orbsOnEllipse.Count - 1];
        orbsOnEllipse.RemoveAt(orbsOnEllipse.Count - 1);
        orbsOnEllipse.Insert(0, lastOrb);

        UpdateOrbMaterials();
        OnNextOrbSelected?.Invoke();
    }

    private void ChoosePreviousOrb()
    {
        if (orbsOnEllipse.Count <= 1) return;

        var firstOrb = orbsOnEllipse[0];
        orbsOnEllipse.RemoveAt(0);
        orbsOnEllipse.Add(firstOrb);

        UpdateOrbMaterials();
        OnPreviousOrbSelected?.Invoke();
    }

    private void UpdateOrbMaterials()
    {
        for (int i = 0; i < orbsOnEllipse.Count; i++)
        {
            if (i == 0)
                orbsOnEllipse[i].SetMaterial(highlightMaterial);
            else
                orbsOnEllipse[i].ResetMaterial();
        }
    }

    private void CreateOrbsAtStart()
    {
        if (orbCountAtStart <= 0) return;

        for (int i = 0; i < orbCountAtStart; i++)
            AddOrb(false);

        OnOrbCountChanged?.Invoke(orbCountAtStart);
    }

    private void Aim()
    {
        if (orbToThrow != null || orbsOnEllipse.Count == 0 || throwCooldownTimer > 0) return;

        isAiming = true;
        orbToThrow = orbsOnEllipse[0];
        RemoveOrbFromList(orbToThrow);
    }

    private void Throw()
    {
        if (orbToThrow == null || !isAiming) return;

        throwCooldownTimer = cooldownBetweenThrowsInSeconds;
        isAiming = false;

        Vector3 throwDirection = GetMouseWorldPosition() - firePointTransform.position;
        throwDirection.y = 0;
        orbToThrow.Throw(throwDirection.normalized * 20f);

        orbToThrow.ResetMaterial();
        orbsThrowed.Add(orbToThrow);
        orbToThrow = null;

        Player.Instance.Hub.OrbHandler.RemoveOrb();
        OnOrbThrowed?.Invoke();
    }

    private void HandleCooldowns()
    {
        if (throwCooldownTimer > 0)
            throwCooldownTimer -= Time.deltaTime;
    }

    private void UpdateEllipsePosition()
    {
        transform.position = Vector3.Lerp(transform.position, ellipseCenterTransform.position, Time.deltaTime * ellipseMovementSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, ellipseCenterTransform.rotation, Time.deltaTime * ellipseRotationSpeed);
    }

    public void AddOrb(bool withCallbacks = true)
    {
        if (orbsThrowed.Count + orbsOnEllipse.Count >= Constants.MAX_ORBS_CAN_BE_HELD)
            return;

        var newOrb = objectPool.GetPooledObject(0).GetComponent<SimpleOrb>();
        newOrb.transform.position = ellipseCenterTransform.position;

        orbsOnEllipse.Add(newOrb);
        Player.Instance.Hub.OrbHandler.AddOrb();
        UpdateOrbEllipsePositions();
        OnOrbAdded?.Invoke();

        if (withCallbacks) OnOrbCountChanged?.Invoke(orbsThrowed.Count + orbsOnEllipse.Count + 1);
    }
    public void RemoveOrb()
    {
        var orbToRemove = orbsOnEllipse[orbsOnEllipse.Count - 1];
        RemoveOrbFromList(orbToRemove);

        OnOrbCountChanged?.Invoke(orbsThrowed.Count + orbsOnEllipse.Count - 1);
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
        if (orbsOnEllipse.Count == 0) return;

        float angleStep = 360f / orbsOnEllipse.Count;
        float angleOffset = 90f; // Start from the top

        for (int i = 0; i < orbsOnEllipse.Count; i++)
        {
            if (orbsOnEllipse[i] == orbToThrow) continue;

            float angle = angleOffset + i * angleStep;
            float angleInRadians = angle * Mathf.Deg2Rad;

            float localX = Mathf.Cos(angleInRadians) * ellipseXRadius;
            float localY = Mathf.Sin(angleInRadians) * ellipseYRadius;

            Vector3 localPosition = new Vector3(localX, localY, 0f);
            Vector3 targetPosition = ellipseCenterTransform.position + ellipseCenterTransform.rotation * localPosition;

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
}