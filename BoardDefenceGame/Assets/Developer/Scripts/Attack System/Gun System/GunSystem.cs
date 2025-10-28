using System;
using UnityEngine;

public abstract class GunSystem : MonoBehaviour
{
    public Action OnUpdate;

    [Header("References")]
    public Transform gunBaseTR;
    public Transform projectileTR;
    [SerializeField] protected Transform projectileSpawnPosTR;

    [Header("Stats")]
    public float attackCooldown;
    public float prjectileDamage;
    public float explosionRadius;
    protected float cooldownTimer = 0f;

    [Header("State")]
    public bool isThereTarget;
    public Item currentItem;

    private void Update() => OnUpdate?.Invoke();

    public abstract void AttackSpesific(float cooldown);

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
