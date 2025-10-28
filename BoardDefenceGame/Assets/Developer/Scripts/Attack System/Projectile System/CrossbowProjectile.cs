using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowProjectile : Projectile
{
    public Transform hitParticleTR;

    public override void GoToTargetSpesific()
    {
        StartCoroutine(GoToTarget());
    }

    public override void TurnTowardsToTarget(Vector3 targetPos, float deltaTime)
    {
        // Hafif spin efekti
        Vector3 spinAxis = new Vector3(0f, 0f, 1f);
        transform.Rotate(spinAxis * rotationSpeed * deltaTime);
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
        float sinusOffsetAmp = 0.1f * distance; // hafif dalgalý yol

        while (t < 1f && _isUpdating)
        {
            t += speedFactor * speed * Time.deltaTime;
            t = Mathf.Clamp01(t);

            if (targetTr == null || enemy == null || !enemy.enemyAlive)
            {
                Vector3 p = Vector3.Lerp(startPos, targetPos, t);
                p.x += Mathf.Sin(t * Mathf.PI * 2f) * sinusOffsetAmp * 0.1f;
                transform.position = p;
            }
            else
            {
                Vector3 dynTarget = enemy.projectileTargetTR != null ? enemy.projectileTargetTR.position : targetTr.position;
                Vector3 p = Vector3.Lerp(startPos, dynTarget, t);
                p.x += Mathf.Sin(t * Mathf.PI * 2f) * sinusOffsetAmp * 0.1f;
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
