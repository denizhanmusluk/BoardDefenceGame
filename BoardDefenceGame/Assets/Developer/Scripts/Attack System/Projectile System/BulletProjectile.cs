using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : Projectile
{
    public Transform hitParticleTR;

    public override void GoToTargetSpesific()
    {
        StartCoroutine(GoToTarget());
    }

    public override void TurnTowardsToTarget(Vector3 targetPos, float deltaTime)
    {
        // Basit görsel dönüþ (ileriye doðru)
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

        while (t < 1f && _isUpdating)
        {
            t += speedFactor * speed * Time.deltaTime;
            t = Mathf.Clamp01(t);

            if (targetTr == null || enemy == null || !enemy.enemyAlive)
            {
                Vector3 p = Vector3.Lerp(startPos, targetPos, t);
                transform.position = p;
            }
            else
            {
                Vector3 dynTarget = enemy.projectileTargetTR != null ? enemy.projectileTargetTR.position : targetTr.position;
                Vector3 p = Vector3.Lerp(startPos, dynTarget, t);
                transform.position = p;
                targetPos = dynTarget;
            }

            TurnTowardsToTarget(targetPos, Time.deltaTime);
            yield return null;
        }

        if (targetTr != null)
        {
            if (hitParticleTR != null)
                SpawnManager.Instance.HitParticleSpawn(hitParticleTR, targetTr.position, Quaternion.identity);

            ProjectileHitBehaviour(targetTr);
        }

        ResetThis();
        Despawn();
    }
}
