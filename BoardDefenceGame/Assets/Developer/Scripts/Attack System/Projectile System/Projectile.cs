using System.Collections;
using UnityEngine;
using EZ_Pooling;

/// <summary>
/// Tüm mermilerin temel sýnýfý.
/// </summary>
[System.Serializable]
public abstract class Projectile : MonoBehaviour
{
    [Header("Base Settings")]
    public bool bulletCollisionDetect = false;

    [HideInInspector] public float speed;
    [HideInInspector] public float rotationSpeed;
    [HideInInspector] public float damage;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float explosionRadius;

    [HideInInspector] public Transform targetTr;
    [HideInInspector] public Enemy enemy;
    [HideInInspector] public GunSystem gunSystem;

    protected bool _isUpdating;

    // --- Abstract Methods ---
    public abstract void GoToTargetSpesific();
    public abstract void TurnTowardsToTarget(Vector3 targetPos, float deltaTime);

    // --- Common Behaviours ---
    public void GoTargetProjectile() => GoToTargetSpesific();

    public void Despawn()
    {
        if (transform != null)
            EZ_PoolManager.Despawn(transform);
    }

    public void ResetThis()
    {
        _isUpdating = false;
        transform.rotation = Quaternion.identity;
    }

    public void ProjectileHitBehaviour(Transform targetTR)
    {
        if (targetTR == null) return;

        if (targetTR.TryGetComponent(out IDamageable iDamageable))
        {
            iDamageable.TakeDamage(damage, transform, 0, 0);
            SpawnManager.Instance?.DamagePointSpawn(transform.position, (int)damage, false);
        }
    }

    public void TurnToTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, transform.eulerAngles.y, -angle);
        transform.rotation = targetRot;
    }
}
