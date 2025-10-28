using System.Collections.Generic;
using UnityEngine;

public class GunCannonBall : GunSystem
{
    [Header("Gun Settings")]
    public float attackRange;
    public float projectileSpeed;
    public float projectileRotationSpeed;
    public int projectileCount = 1;

    public List<Transform> inRangeTargetList; // Inspector'da baðlý olabilir
    public Transform targetTr;

    public override void AttackSpesific(float cooldown)
    {
        EnsureList();
        ScanTargetsInRange();
        if (!isThereTarget) return;

        for (int i = 0; i < projectileCount; i++)
        {
            targetTr = GetNearestTarget();
            if (targetTr == null) continue;
            Attack(targetTr);
        }
    }

    private void EnsureList()
    {
        if (inRangeTargetList == null)
            inRangeTargetList = new List<Transform>();
        else
            inRangeTargetList.Clear();
    }

    private void ScanTargetsInRange()
    {
        isThereTarget = false;

        var list = PlayerManager.Instance != null ? PlayerManager.Instance.enemyList : null;
        if (list == null || list.Count == 0) return;

        Vector3 selfPos = transform.position;
        float range = attackRange;

        for (int i = 0; i < list.Count; i++)
        {
            Enemy e = list[i];
            if (e == null || !e.isTargetable) continue;

            if (Vector3.Distance(e.transform.position, selfPos) <= range)
            {
                isThereTarget = true;
                inRangeTargetList.Add(e.transform);
            }
        }
    }

    private Transform GetNearestTarget()
    {
        if (inRangeTargetList.Count == 0) return null;

        Transform closest = inRangeTargetList[0];
        Vector3 selfPos = transform.position;
        float minDist = Vector3.Distance(closest.position, selfPos);

        for (int i = 1; i < inRangeTargetList.Count; i++)
        {
            Transform t = inRangeTargetList[i];
            if (t == null) continue;

            float d = Vector3.Distance(t.position, selfPos);
            if (d < minDist)
            {
                minDist = d;
                closest = t;
            }
        }
        return closest;
    }

    private void Attack(Transform target)
    {
        if (target == null) return;

        currentItem?.ItemIconTurnTarget(target.position);

        float dmg = prjectileDamage;
        Quaternion spawnRot = Quaternion.identity;

        SpawnManager.Instance.SpawnBomb(
           this,
           projectileTR,
           projectileSpawnPosTR.position,
           spawnRot,
           target,
           projectileSpeed,
           projectileRotationSpeed,
           dmg,
           attackRange,
           explosionRadius
       );
    }
}
