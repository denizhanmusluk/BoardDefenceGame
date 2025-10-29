using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class GunSystem : MonoBehaviour
{
    public Action OnUpdate;

    [Header("References")]
    public GunSettings gunSettings;
    public Transform gunBaseTR;
    public Transform projectileTR;
    [SerializeField] protected Transform projectileSpawnPosTR;

    [Header("Stats")]
    public float attackCooldown;
    public float prjectileDamage;
    public float explosionRadius;
    protected float cooldownTimer = 0f;
    public float verticalRange;
    public float horizontalRange;
    public float projectileSpeed;
    public float projectileRotationSpeed;
    public int projectileCount = 1;

    [Header("State")]
    public bool isThereTarget;
    public Item currentItem;
    public List<Transform> inRangeTargetList;
    public Transform targetTr;

    private void Update() => OnUpdate?.Invoke();

    public abstract void AttackSpesific(float cooldown);
    private void Start()
    {
        prjectileDamage = gunSettings.attackDamage;
        attackCooldown = gunSettings.attackCooldown;
        verticalRange = gunSettings.verticalRange * Globals.attackRangeFactor;
        horizontalRange = gunSettings.horizontalRange;
    }

    #region Gun Lifecycle
    public void RegisterGun()
    {
        GameEventSystem.GunStart += GunStart;
        GameEventSystem.GunStop += GunStop;
    }

    public void RemoveGun()
    {
        GameEventSystem.GunStart -= GunStart;
        GameEventSystem.GunStop -= GunStop;
    }

    public void GunStart()
    {
        cooldownTimer = 0f;
        OnUpdate += Attacking;
    }

    public void GunStop() => OnUpdate = null;
    #endregion

    #region Attack Logic
    private void Attacking()
    {
        cooldownTimer += Time.deltaTime;

        if (isThereTarget)
            currentItem.SpriteCooldown(cooldownTimer, attackCooldown);

        if (cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0f;
            AttackSpesific(attackCooldown);
        }
    }
    #endregion
}
