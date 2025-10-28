using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : Projectile
{
    public Transform hitParticleTR;
    private float rotateCounter = 0f;

    public override void GoToTargetSpesific()
    {
        StartCoroutine(GoToTarget());
    }

    public override void TurnTowardsToTarget(Vector3 targetPos, float deltaTime)
    {
        Vector3 axis = new Vector3(0f, 0f, 1f);
        transform.Rotate(axis * rotationSpeed * deltaTime);
    }

    private IEnumerator GoToTarget()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = (enemy != null && enemy.projectileTargetTR != null)
            ? enemy.projectileTargetTR.position
            : (targetTr != null ? targetTr.position : startPos);

        float distance = Mathf.Max(0.001f, Vector3.Distance(startPos, targetPos));
        float speedFactor = attackRange / distance;

        TurnToTarget(targetPos);

        _isUpdating = false;
        yield return null;
        _isUpdating = true;

        float t = 0f;
        float posY = 0f;
        float posY_Factor = Random.Range(2f, 3f);

        while (t < 1f && _isUpdating)
        {
            t += speedFactor * speed * Time.deltaTime;
            t = Mathf.Clamp01(t);

            float angle = t * Mathf.PI;
            posY = posY_Factor * Mathf.Sin(angle);

            if (targetTr == null || enemy == null || !enemy.enemyAlive)
            {
                Vector3 p = Vector3.Lerp(startPos, targetPos, t);
                p.z -= posY;
                transform.position = p;
            }
            else
            {
                Vector3 dynTarget = enemy.projectileTargetTR != null ? enemy.projectileTargetTR.position : targetTr.position;
                Vector3 p = Vector3.Lerp(startPos, dynTarget, t);
                p.z -= posY;
                transform.position = p;
                targetPos = dynTarget;
            }

            TurnTowardsToTarget(targetPos, Time.deltaTime);
            yield return null;
        }

        if (targetTr != null)
        {
            ApplyExplosionDamage();

            if (hitParticleTR != null)
                SpawnManager.Instance.HitParticleSpawn(hitParticleTR, targetTr.position, Quaternion.identity);
        }

        ResetThis();
        Despawn();
    }

    private void ApplyExplosionDamage()
    {
        var list = PlayerManager.Instance != null ? PlayerManager.Instance.enemyList : null;
        if (list == null || list.Count == 0) return;

        Vector3 pos = transform.position;

        for (int i = 0; i < list.Count; i++)
        {
            Enemy e = list[i];
            if (e == null) continue;

            if (explosionRadius > Vector3.Distance(e.transform.position, pos))
                ProjectileHitBehaviour(e.transform);
        }
    }
}
