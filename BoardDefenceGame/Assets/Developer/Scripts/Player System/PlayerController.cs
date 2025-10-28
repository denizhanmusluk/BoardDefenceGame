using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    Attack,
    FollowingPlayer,
    Idle,
    Dead,
    Hand
}

public class PlayerController : Singleton<PlayerController>
{
    [Header("Player State")]
    public PlayerStates currentState = PlayerStates.Idle;

    [Header("Weapons & Cannons")]
    public List<Transform> weaponPositions = new();
    public List<Cannon> activeCannons = new();
    public List<Cannon> aliveCannons = new();
    public List<GameObject> weaponLockObjects = new();

    [Header("Player Components")]
    public GameObject healthBar;
    public List<Transform> followPositions = new();
    public Transform bossFollowPosition;
    public PlayerHealthController playerHealth;

    [HideInInspector] public bool isAlive = true;

    protected override void Awake()
    {
        base.Awake();
    }

    #region Player Lifecycle
    public void StartBattle()
    {
        healthBar?.SetActive(true);
    }

    public void InitializeHealth()
    {
    }

    public void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        currentState = PlayerStates.Dead;

        GameEventSystem.GameFailEvent();
    }
    #endregion

    #region Cannon Management
    public void CreateCannon(Cannon cannonPrefab, int cannonIndex)
    {
        if (cannonPrefab == null)
        {
            return;
        }

        if (cannonIndex < 0 || cannonIndex >= weaponPositions.Count)
        {
            return;
        }

        Transform parent = weaponPositions[cannonIndex];
        Cannon newCannon = Instantiate(cannonPrefab, parent.position, Quaternion.identity, parent);
        newCannon.transform.localRotation = Quaternion.identity;

        newCannon.gunId = cannonIndex;
        newCannon._slotPanel = SlotBuyManager.Instance.slotPanelLisForCannon[cannonIndex];

        AddCannon(newCannon);
        weaponLockObjects[cannonIndex]?.SetActive(false);
    }

    public void AddCannon(Cannon cannon)
    {
        if (!activeCannons.Contains(cannon))
            activeCannons.Add(cannon);

        if (!aliveCannons.Contains(cannon))
            aliveCannons.Add(cannon);
    }

    public void RemoveCannon(Cannon cannon)
    {
        if (cannon == null) return;
        aliveCannons.Remove(cannon);

        if (aliveCannons.Count == 0)
            GameEventSystem.GameFailEvent();
    }
    #endregion
}
