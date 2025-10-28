using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallProjectile : Projectile
{
    public Transform hitParticleTR;

    public override void GoToTargetSpesific()
    {
        StartCoroutine(GoToTarget());
    }

    public override void TurnTowardsToTarget(Vector3 targetPos, float deltaTime)
    {
        Vector3 axis = new Vector3(1f, 0f, 0f);
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

        // Basit parabolik hareket (z ekseninde “yukarý” gibi davranalým)
        float t = 0f;
        float arcAmplitude = Mathf.Clamp(distance * 0.25f, 1f, 6f);

        while (t < 1f && _isUpdating)
        {
            t += speedFactor * speed * Time.deltaTime;
            t = Mathf.Clamp01(t);

            Vector3 lerpTarget;
            if (targetTr == null || enemy == null || !enemy.enemyAlive)
            {
                lerpTarget = targetPos;
            }
            else
            {
                lerpTarget = enemy.projectileTargetTR != null ? enemy.projectileTargetTR.position : targetTr.position;
                targetPos = lerpTarget;
            }

            Vector3 p = Vector3.Lerp(startPos, targetPos, t);
            float arc = Mathf.Sin(t * Mathf.PI) * arcAmplitude;
            p.z -= arc; // sahne eksenine göre ayarlanmýþ "yukarý"
            transform.position = p;

            TurnTowardsToTarget(targetPos, Time.deltaTime);
            yield return null;
        }

        if (targetTr != null)
        {
            // splash damage
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
