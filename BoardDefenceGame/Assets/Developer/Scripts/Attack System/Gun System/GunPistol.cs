using System.Collections.Generic;
using UnityEngine;

public class GunPistol : GunSystem
{
    [Header("Gun Settings")]
    public float attackRange;
    public float projectileSpeed;
    public float projectileRotationSpeed;
    public int bulletCount = 1;

    private readonly List<Transform> targetsInRange = new();

    public override void AttackSpesific(float cooldown)
    {
        isThereTarget = false;
        targetsInRange.Clear();

        foreach (Enemy enemy in PlayerManager.Instance.enemyList)
        {
            if (enemy == null || !enemy.isTargetable) continue;

            if (Vector3.Distance(enemy.transform.position, transform.position) <= attackRange)
            {
                isThereTarget = true;
                targetsInRange.Add(enemy.transform);
            }
        }

        if (!isThereTarget) return;

        for (int i = 0; i < bulletCount; i++)
        {
            Transform nearest = GetNearestTarget();
            if (nearest == null) continue;
            Fire(nearest);
        }
    }

    private Transform GetNearestTarget()
    {
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Transform t in targetsInRange)
        {
            float dist = Vector3.Distance(t.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }

        return closest;
    }

    private void Fire(Transform target)
    {
        if (target == null) return;

        currentItem.ItemIconTurnTarget(target.position);
        float damage = prjectileDamage;
        Quaternion spawnRot = Quaternion.Euler(-32.84f, 0, 0);

        SpawnManager.Instance.SpawnCrossBow(this, projectileTR, projectileSpawnPosTR.position, spawnRot, target, projectileSpeed, projectileRotationSpeed, damage, attackRange);

        if (damage > target.GetComponent<EnemyHealthController>().CurrentHealth)
            target.GetComponent<Enemy>().isTargetable = false;
    }
}
