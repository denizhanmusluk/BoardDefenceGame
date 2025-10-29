using System.Collections.Generic;
using UnityEngine;

public class GunBomb : GunSystem
{
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

        for (int i = 0; i < list.Count; i++)
        {
            Enemy e = list[i];
            if (e == null || !e.isTargetable) continue;

            if (Mathf.Abs(e.transform.position.y - selfPos.y) <= verticalRange && Mathf.Abs(e.transform.position.x - selfPos.x) <= horizontalRange)
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
            explosionRadius,
            verticalRange
        );
    }
}
