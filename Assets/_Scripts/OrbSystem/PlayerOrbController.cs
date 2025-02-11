using com.game;
using com.game.player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

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
    [SerializeField] private ObjectPool objectPool;

    [Header("Orb Materials")]
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private GameObject ghostOrbPrefab;

    // Events
    public event Action OnOrbThrowed;
    public event Action OnOrbCalled;
    public event Action<int> OnOrbCountChanged;
    public event Action OnAllOrbsCalled;
    public event Action OnNextOrbSelected;
    public event Action OnPreviousOrbSelected;
    public event Action OnSelectedOrbChanged;
    public event Action OnOrbAdded;

    public List<SimpleOrb> OrbsOnEllipse = new();
    private List<GhostOrb> ghostOrbs = new();
    private SimpleOrb orbToThrow;
    private float throwCooldownTimer;
    public bool isAiming = false;

    private int activeOrbCount = 0;
    private int selectedOrbIndex = 0;
    private float angleStep; // The angle between orbs

    private PlayerStats _playerStats;

    [Inject]
    private void ZenjectSetup(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }
    private void Start()
    {
        orbCountAtStart = Player.Instance.CharacterProfile.OrbCount;

        if (objectPool == null)
            objectPool = FindAnyObjectByType<ObjectPool>();

        CreateGhostOrbsAtStart();
        CreateOrbsAtStart();
        CalculateAngleStep();
    }
    private void Update()
    {
        if (Game.Paused) return;

        HandleInput();
        HandleCooldowns();
        //UpdateEllipsePosition();
        UpdateOrbEllipsePositions();
    }

    private void HandleInput()
    {
        if (PlayerInputHandler.Instance.AttackButtonPressed)
            Aim();
        else if (PlayerInputHandler.Instance.AttackButtonReleased)
            Throw();

        if (PlayerInputHandler.Instance.RecallButtonPressed)
            CallOrbs();

        if (PlayerInputHandler.Instance.NextChooseButtonPressed)
            ChooseNextOrb();

        if (PlayerInputHandler.Instance.PreviousChooseButtonPressed)
            ChoosePreviousOrb();  
    }
    private void CreateGhostOrbsAtStart()
    {
        for (int i = 0; i < maximumOrbCount; i++)
        {
            var ghostOrb = Instantiate(ghostOrbPrefab, transform);
            ghostOrb.SetActive(false);
            ghostOrbs.Add(ghostOrb.GetComponent<GhostOrb>());
        }
    }
    private void CallOrbs()
    {
        foreach (var orb in OrbsOnEllipse)
        {
            if (orb.currentState == OrbState.Sticked)
                CallOrb(orb);
        }
        OnAllOrbsCalled?.Invoke();
    }
    private void CallOrb(SimpleOrb orb)
    {
        orb.Return(firePointTransform.position);
        Player.Instance.Hub.OrbHandler.AddOrb();
        OnOrbCalled?.Invoke();
    }
    private void ChooseNextOrb()
    {
        if (OrbsOnEllipse.Count <= 1) return;

        selectedOrbIndex = (selectedOrbIndex + 1) % OrbsOnEllipse.Count;
        ShiftOrbsInList();
        UpdateOrbEllipsePositions();
        OnNextOrbSelected?.Invoke();
    }
    private void ChoosePreviousOrb()
    {
        if (OrbsOnEllipse.Count <= 1) return;

        selectedOrbIndex = (selectedOrbIndex - 1 + OrbsOnEllipse.Count) % OrbsOnEllipse.Count;
        ShiftOrbsInList();
        UpdateOrbEllipsePositions();
        OnPreviousOrbSelected?.Invoke();
    }
    private void ShiftOrbsInList()
    {
        List<SimpleOrb> newList = new();

        for (int i = 0; i < OrbsOnEllipse.Count; i++)
        {
            int newIndex = (i + selectedOrbIndex) % OrbsOnEllipse.Count;
            newList.Add(OrbsOnEllipse[newIndex]);
        }

        OrbsOnEllipse = newList;
        selectedOrbIndex = 0; // En üstteki top her zaman sıfırıncı indexte olacak
    }

    private void UpdateSelectedOrbMaterial()
    {
        for (int i = 0; i < OrbsOnEllipse.Count; i++)
        {
            if (i == selectedOrbIndex)
                OrbsOnEllipse[i].SetMaterial(highlightMaterial);
            else
                OrbsOnEllipse[i].ResetMaterial();
        }
    }

    private void CreateOrbsAtStart()
    {
        if (orbCountAtStart <= 0) return;

        for (int i = 0; i < orbCountAtStart; i++)
            AddOrb();

        OnOrbCountChanged?.Invoke(orbCountAtStart);
    }

    private void Aim()
    {
        if (orbToThrow != null || OrbsOnEllipse.Count == 0 || throwCooldownTimer > 0 || OrbsOnEllipse[selectedOrbIndex].currentState != OrbState.OnEllipse) return;

        isAiming = true;
        orbToThrow = OrbsOnEllipse[selectedOrbIndex];
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

    public void AddOrb()
    {
        var newOrb = objectPool.GetPooledObject(0).GetComponent<SimpleOrb>();
        newOrb.transform.position = ellipseCenterTransform.position;

        activeOrbCount++;
        OrbsOnEllipse.Add(newOrb);
        CalculateAngleStep();

        UpdateOrbEllipsePositions();

        Player.Instance.Hub.OrbHandler.AddOrb();
        OnOrbAdded?.Invoke();
    }

    public void AddOrbToList(SimpleOrb orb)
    {
        OrbsOnEllipse.Add(orb);
    }

    public void RemoveOrbFromEllipse(SimpleOrb orb)
    {
        OrbsOnEllipse.Remove(orb);
        activeOrbCount--;
        UpdateOrbEllipsePositions();
    }
    public void RemoveOrb()
    {
        int indexToRemove = OrbsOnEllipse.Count - 1;
        OrbsOnEllipse.RemoveAt(indexToRemove);
        activeOrbCount--;
        CalculateAngleStep();
        UpdateOrbEllipsePositions();
    }

    private void UpdateOrbEllipsePositions()
    {
        if (OrbsOnEllipse.Count == 0) return;

        float angleOffset = 90f;

        for (int i = 0; i < OrbsOnEllipse.Count; i++)
        {
            float angle = angleOffset + i * angleStep;
            float angleInRadians = angle * Mathf.Deg2Rad;

            float localX = Mathf.Cos(angleInRadians) * ellipseXRadius;
            float localY = Mathf.Sin(angleInRadians) * ellipseYRadius;

            Vector3 localPosition = new Vector3(localX, localY, 0f);
            Vector3 targetPosition = ellipseCenterTransform.position + (ellipseCenterTransform.rotation * localPosition);

            if(OrbsOnEllipse[i] == orbToThrow)
            {
                orbToThrow.IncreaseSpeedForSeconds(15f, 0.1f);
                orbToThrow.SetNewDestination(firePointTransform.position);
            }
            else if (ghostOrbs[i] != null)
            {
                if (OrbsOnEllipse[i].currentState != OrbState.OnEllipse)
                {
                    if (ghostOrbs[i].gameObject.activeSelf == false)
                        ghostOrbs[i].gameObject.SetActive(true);

                    ghostOrbs[i].SetNewDestination(targetPosition);
                }
                else
                {
                    if (ghostOrbs[i].gameObject.activeSelf == true)
                        ghostOrbs[i].gameObject.SetActive(false);

                    OrbsOnEllipse[i]?.SetNewDestination(targetPosition);
                }
            }

            if (OrbsOnEllipse[i].currentState == OrbState.Returning)
                OrbsOnEllipse[i].SetNewDestination(firePointTransform.position);
        }
        UpdateSelectedOrbMaterial();
    }
    private void CalculateAngleStep()
    {
        angleStep = 360f / activeOrbCount;
    }
    private Vector3 GetMouseWorldPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, cursorDetectMask))
            return hitInfo.point;

        return Vector3.zero;
    }
}