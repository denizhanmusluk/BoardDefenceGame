using System.Collections.Generic;
using UnityEngine;

public class GunPistol : GunSystem
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
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Transform t in inRangeTargetList)
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

    private void Attack(Transform target)
    {
        if (target == null) return;

        currentItem.ItemIconTurnTarget(target.position);
        float damage = prjectileDamage;
        Quaternion spawnRot = Quaternion.Euler(-32.84f, 0, 0);

        SpawnManager.Instance.SpawnCrossBow(
            this,
            projectileTR,
            projectileSpawnPosTR.position,
            spawnRot,
            target,
            projectileSpeed,
            projectileRotationSpeed, 
            damage,
            verticalRange
         );

        if (damage > target.GetComponent<EnemyHealthController>().CurrentHealth)
            target.GetComponent<Enemy>().isTargetable = false;
    }
}
