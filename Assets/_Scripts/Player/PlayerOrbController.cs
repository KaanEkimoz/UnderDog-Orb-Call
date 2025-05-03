using com.game;
using com.game.player;
using com.game.player.statsystemextensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
public class PlayerOrbController : MonoBehaviour
{
    private static readonly Func<SimpleOrb, bool> s_bestOrbPredicateThrow = (orb) =>
    {
        return orb.currentState == OrbState.OnEllipse;
    };

    private static readonly Func<SimpleOrb, bool> s_bestOrbPredicateRecall = (orb) =>
    {
        return orb.currentState == OrbState.Sticked;
    };

    public static readonly Dictionary<Type, int> OrbTypePoolIndexDict = new Dictionary<Type, int>()
    {
        { typeof(SimpleOrb), SIMPLE_ORB_INDEX },
        { typeof(FireOrb), FIRE_ORB_INDEX },
        { typeof(IceOrb), ICE_ORB_INDEX },
        { typeof(ElectricOrb), ELECTRIC_ORB_INDEX },
    };

    [Header("Orb Count")]
    [Range(5, 15)][SerializeField] private int maximumOrbCount = 10;
    [Range(0, 10)][SerializeField] private int orbCountAtStart = 5;
    [SerializeField] private bool smartOrbSelection = true;
    [SerializeField] private bool autoSelectNextOrbOnShoot = false;
    [Header("Orb Throw")]
    [SerializeField] private float cooldownBetweenThrowsInSeconds = 0f;
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private LayerMask aimCursorDetectMask;
    [Header("Orb Recall")]
    [SerializeField] private float recallButtonHoldTime = 0.2f;
    [SerializeField] private Transform returnPointTransform;
    [SerializeField] private float callAllOrbsSpeedCoefficient;
    [SerializeField] private bool allOrbsCoefficientAffectsPerOrb;
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
    [Space]
    [Header("Extensions")]
    [SerializeField] private List<PlayerOrbControllerExtensionBase> m_extensions = new();
    [SerializeField] private Animator _playerAnimator;

    //Orb Types
    public const int SIMPLE_ORB_INDEX = 7;
    public const int FIRE_ORB_INDEX = 9;
    public const int ICE_ORB_INDEX = 10;
    public const int ELECTRIC_ORB_INDEX = 11;

    // Events
    public event Action OnOrbThrowed;
    public event Action OnOrbCalled;
    public event Action<float> OnDamageGiven;
    public event Action<int> OnOrbCountChanged;
    public event Action OnAllOrbsCalled;
    public event Action OnNextOrbSelected;
    public event Action OnPreviousOrbSelected;
    public event Action OnSelectedOrbChanged;
    public event Action<SimpleOrb> OnOrbAdded;

    public List<SimpleOrb> orbsOnEllipse = new();
    private List<GhostOrb> ghostOrbs = new();
    private SimpleOrb orbToThrow;
    private float throwCooldownTimer;
    private int activeOrbCount = 0;
    private int selectedOrbIndex = 0;
    private float angleStep; // The angle between orbs
    private PlayerStats _playerStats;
    private SoundFXManager _soundFXManager;
    public bool IsAiming { get; private set; } = false;
    public int SelectedOrbIndex => selectedOrbIndex;

    [Inject]
    private void ZenjectSetup(PlayerStats playerStats, SoundFXManager soundFXManager)
    {
        _playerStats = playerStats;
        _soundFXManager = soundFXManager;
    }
    private void Start()
    {
        orbCountAtStart = Player.Instance.CharacterProfile.OrbCount;

        if (objectPool == null)
            objectPool = FindAnyObjectByType<ObjectPool>();

        InitializeGhostOrbs();
        InitializeOrbs();
        CalculateAngleStep();

        StartAiming();
    }
    private void Update()
    {
        if (Game.Paused) return;

        HandleInput();
        HandleCooldowns();
        UpdateOrbEllipsePositions();
        UpdateAnimator ();
    }
    private void HandleInput()
    {
        if (PlayerInputHandler.Instance.AttackButtonPressed)
            ThrowOrb();

        if (PlayerInputHandler.Instance.RecallButtonPerformed)
            CallAllOrbs();
        else if (PlayerInputHandler.Instance.RecallButtonReleased)
            CallOrb(orbsOnEllipse[selectedOrbIndex]);

        if(PlayerInputHandler.Instance.ClosestRecallButtonPressed)
            CallOrb(FindClosestOrb(orbsOnEllipse));

        if (PlayerInputHandler.Instance.NextChooseButtonPressed)
            SelectNextOrb();
        else if (PlayerInputHandler.Instance.PreviousChooseButtonPressed)
            SelectPreviousOrb();  
    }
    private void InitializeOrbs()
    {
        if (orbCountAtStart <= 0) return;

        for (int i = 0; i < orbCountAtStart; i++)
            AddOrb();

        OnOrbCountChanged?.Invoke(orbCountAtStart);
    }
    private void InitializeGhostOrbs()
    {
        for (int i = 0; i < maximumOrbCount; i++)
        {
            var ghostOrb = Instantiate(ghostOrbPrefab, transform);
            ghostOrb.SetActive(false);
            ghostOrbs.Add(ghostOrb.GetComponent<GhostOrb>());
        }
    }
    private void StartAiming()
    {
        selectedOrbIndex = -1;
        SelectNextOrb();
        IsAiming = true;
    }
    private void ThrowOrb()
    {
        if (throwCooldownTimer > 0)
            return;

        if (!IsAiming)
            return;

        foreach (SimpleOrb orb in orbsOnEllipse)
        {
            if (orb.currentState == OrbState.Returning)
                return;
        }

        if (orbToThrow == null || orbToThrow.currentState != OrbState.OnEllipse)
        {
            if (smartOrbSelection) SelectBestOrbLHS(s_bestOrbPredicateThrow);
            else return;
        }

        throwCooldownTimer = cooldownBetweenThrowsInSeconds;
        _playerAnimator.SetTrigger("Throw");

        Vector3 throwDirection = PlayerInputHandler.Instance.GetMouseWorldPosition(aimCursorDetectMask) - firePointTransform.position;
        throwDirection.y = 0;

        foreach (PlayerOrbControllerExtensionBase extension in m_extensions)
        {
            throwDirection = extension.ConvertAimDirection(throwDirection);
        }

        orbToThrow.Throw(throwDirection.normalized);

        if (autoSelectNextOrbOnShoot)
            SelectPreviousOrb();

        Player.Instance.Hub.OrbHandler.RemoveOrb();
        
        OnOrbThrowed?.Invoke();
    }
    private void CallOrb(SimpleOrb orb, float coefficient = 1f)
    {
        if (orb.currentState != OrbState.Sticked)
        {
            if (smartOrbSelection) SelectBestOrbLHS(s_bestOrbPredicateRecall);
            else return;
        }

        orb.ReturnToPosition(returnPointTransform.position, coefficient);
        OnOrbCalled?.Invoke();
    }
    private void CallAllOrbs()
    {
        IEnumerable<SimpleOrb> stickedOrbs = 
            orbsOnEllipse.Where(orb => orb.currentState == OrbState.Sticked);

        int lastCalledOrbCount = stickedOrbs.Count();

        if (lastCalledOrbCount == 1)
        {
            CallOrb(stickedOrbs.FirstOrDefault());
            return;
        }

        float speedCoefficient = 1f;

        if (allOrbsCoefficientAffectsPerOrb)
            speedCoefficient = Mathf.Pow(callAllOrbsSpeedCoefficient, lastCalledOrbCount);
        else
            speedCoefficient = callAllOrbsSpeedCoefficient;

        foreach (var orb in stickedOrbs)
        {
            CallOrb(orb, speedCoefficient);
        }

        OnAllOrbsCalled?.Invoke();
    }
    private void SelectNextOrb()
    {
        if (orbsOnEllipse.Count <= 1) return;

        selectedOrbIndex = (selectedOrbIndex + 1) % orbsOnEllipse.Count;
        orbToThrow = orbsOnEllipse[selectedOrbIndex];
        ShiftOrbs();
        UpdateOrbEllipsePositions();
        OnNextOrbSelected?.Invoke();
    }
    private void SelectPreviousOrb()
    {
        if (orbsOnEllipse.Count <= 1) return;

        selectedOrbIndex = (selectedOrbIndex - 1 + orbsOnEllipse.Count) % orbsOnEllipse.Count;
        orbToThrow = orbsOnEllipse[selectedOrbIndex];
        ShiftOrbs();
        UpdateOrbEllipsePositions();
        OnPreviousOrbSelected?.Invoke();
    }
    private void ShiftOrbs()
    {
        List<SimpleOrb> shiftedOrbs = new();

        for (int i = 0; i < orbsOnEllipse.Count; i++)
        {
            int newIndex = (i + selectedOrbIndex) % orbsOnEllipse.Count;
            shiftedOrbs.Add(orbsOnEllipse[newIndex]);
        }

        orbsOnEllipse = shiftedOrbs;
        selectedOrbIndex = 0; 
    }
    private void UpdateOrbEllipsePositions()
    {
        if (orbsOnEllipse.Count == 0) return;

        float angleOffset = 90f;

        for (int i = 0; i < orbsOnEllipse.Count; i++)
        {
            float angle = angleOffset + i * -angleStep;
            float angleInRadians = angle * Mathf.Deg2Rad;

            float localX = Mathf.Cos(angleInRadians) * ellipseXRadius;
            float localY = Mathf.Sin(angleInRadians) * ellipseYRadius;

            Vector3 localPosition = new Vector3(localX, localY, 0f);
            Vector3 targetPosition = ellipseCenterTransform.position + (ellipseCenterTransform.rotation * localPosition);

            if (orbsOnEllipse[i] == orbToThrow)
            {
                if (orbsOnEllipse[i].currentState == OrbState.OnEllipse)
                {
                    orbToThrow.IncreaseSpeedForSeconds(15f, 0.1f);
                    orbToThrow.SetNewDestination(firePointTransform.position);
                }
            }
            else if (ghostOrbs[i] != null)
            {
                if (orbsOnEllipse[i].currentState != OrbState.OnEllipse)
                {
                    ghostOrbs[i].gameObject.SetActive(true);
                    ghostOrbs[i].SetNewDestination(targetPosition);
                }
                else
                {
                    ghostOrbs[i].gameObject.SetActive(false);
                    orbsOnEllipse[i].SetNewDestination(targetPosition);
                }
            }

            if (orbsOnEllipse[i].currentState == OrbState.Returning)
                orbsOnEllipse[i].SetNewDestination(returnPointTransform.position);

        }
        UpdateSelectedOrbMaterial();
    }
    private void UpdateSelectedOrbMaterial()
    {
        for (int i = 0; i < orbsOnEllipse.Count; i++)
        {
            if (i == selectedOrbIndex)
                orbsOnEllipse[i].SetSelected(true);
            else
                orbsOnEllipse[i].SetSelected(false);
        }
    }
    private void HandleCooldowns()
    {
        if (throwCooldownTimer > 0)
            throwCooldownTimer -= Time.deltaTime * ((_playerStats.GetStat(PlayerStatType.AttackSpeed) / 10) + 1);
    }

    public void AddOrb(ElementalType elementalType = ElementalType.None)
    {
        int spawnIndex = elementalType switch
        {
            ElementalType.Fire => FIRE_ORB_INDEX,
            ElementalType.Ice => ICE_ORB_INDEX,
            ElementalType.Electric => ELECTRIC_ORB_INDEX,
            _ => SIMPLE_ORB_INDEX,
        };

        SimpleOrb newOrb = objectPool.GetPooledObject(spawnIndex).GetComponent<SimpleOrb>();
        newOrb.transform.position = ellipseCenterTransform.position;
        newOrb.AssignPlayerStats(_playerStats);
        newOrb.AssingSoundFXManager(_soundFXManager);

        InitializeOrb(newOrb);
        activeOrbCount++;
        orbsOnEllipse.Add(newOrb);
        CalculateAngleStep();

        UpdateOrbEllipsePositions();

        Player.Instance.Hub.OrbHandler.AddOrb();
        OnOrbAdded?.Invoke(newOrb);
    }

    public bool SwapOrb(SimpleOrb target, SimpleOrb prefab, out SimpleOrb newOrb)
    {
        newOrb = null;
        if (!orbsOnEllipse.Contains(target))
            return false;

        if (OrbTypePoolIndexDict.TryGetValue(prefab.GetType(), out int poolIndex))
            newOrb = objectPool.GetPooledObject(poolIndex).GetComponent<SimpleOrb>();
        else
            newOrb = Instantiate(prefab);

        int targetIndex = orbsOnEllipse.IndexOf(target);

        InitializeOrb(newOrb);
        newOrb.transform.position = target.transform.position;
        orbsOnEllipse[targetIndex] = newOrb;

        if (selectedOrbIndex == targetIndex)
        {
            selectedOrbIndex--;
            SelectNextOrb();
        }

        return true;
    }
    void InitializeOrb(SimpleOrb orb)
    {
        orb.transform.position = ellipseCenterTransform.position;
        orb.AssignPlayerStats(_playerStats);
        orb.OnReachedToEllipse += () =>
        {
            Player.Instance.Hub.OrbHandler.AddOrb();
        };
        orb.OnCallDemanded += (orb) =>
        {
            CallOrb(orb);
        };
    }

    bool SelectBestOrbLHS(Func<SimpleOrb, bool> condition)
    {
        bool success = FindBestOrbLHS(condition, out int differenceLHS);

        if (!success)
            return false;

        for (int i = 0; i < differenceLHS; i++) 
        {
            SelectPreviousOrb();
        }

        return true;
    }

    bool FindBestOrbLHS(Func<SimpleOrb, bool> condition, out int differenceLHS)
    {
        differenceLHS = -1;
        bool success = false;
        for (int i = selectedOrbIndex; i > selectedOrbIndex - orbsOnEllipse.Count; i--)
        {
            differenceLHS++;

            int index = i;
            if (index < 0) index += orbsOnEllipse.Count;

            SimpleOrb iteratedOrb = orbsOnEllipse[index];

            bool result = condition.Invoke(iteratedOrb);

            if (result)
            {
                success = true;
                break;
            }
        }

        return success;
    }

    public void RemoveOrbFromEllipse(SimpleOrb orb)
    {
        orbsOnEllipse.Remove(orb);
        activeOrbCount--;
        UpdateOrbEllipsePositions();
    }
    public void RemoveOrb()
    {
        int indexToRemove = orbsOnEllipse.Count - 1;
        orbsOnEllipse.RemoveAt(indexToRemove);
        activeOrbCount--;
        CalculateAngleStep();
        UpdateOrbEllipsePositions();
    }
    private void CalculateAngleStep()
    {
        angleStep = 360f / activeOrbCount;
    }
    public SimpleOrb FindClosestOrb(List<SimpleOrb> orbList)
    {
        if (orbList == null || orbList.Count == 0 || returnPointTransform == null)
            return null;

        SimpleOrb closestOrb = null;
        float minDistance = Mathf.Infinity;

        foreach(SimpleOrb orb in orbList)
        {
            if (orb.currentState != OrbState.Sticked) continue;

            float distance = Vector3.Distance(returnPointTransform.position, orb.gameObject.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestOrb = orb;
            }
        }

        
        return closestOrb;
    }
    public void UpdateAnimator()
    {
        //_playerAnimator.SetBool("IsAiming", IsAiming);
    }

    //private void OnGUI()
    //{
    //    for (int i = 0; i < orbsOnEllipse.Count; i++) 
    //    {
    //        Color prevColor = GUI.color;
    //        if (i == selectedOrbIndex) GUI.color = Color.yellow;

    //        GUILayout.Label(orbsOnEllipse[i].penetrationExcessDamage.ToString());

    //        GUI.color = prevColor;
    //    }
    //}
}