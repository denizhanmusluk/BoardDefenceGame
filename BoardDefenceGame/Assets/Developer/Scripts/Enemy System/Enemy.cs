using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public AIMoving aiMoving;

    public Transform projectileTargetTR;

    [HideInInspector] public Transform followPosTR;
    [HideInInspector] public float enemyDamage;
    [HideInInspector] public float attackCooldown;
    [HideInInspector] public bool isTargetable = true;
    [HideInInspector] public bool enemyAlive = true;

    public List<Enemy> enemyListForBoss = new List<Enemy>();

    public void Despawn() => EZ_PoolManager.Despawn(transform);

    public void ResetThis()
    {
        enemyAlive = true;
        isTargetable = true;
    }

    public void SpawnStart()
    {
        PlayerManager.Instance.RegisterEnemy(this);
        aiMoving.GoTargetPos(followPosTR.position);
        aiMoving.BehaviourInit(EnemyAttackStart);
    }

    public void EnemyGetDamage()
    {
        if (!enemyAlive) return;
        isTargetable = true;
    }

    public void OnDeath()
    {
        if (!enemyAlive) return;

        aiMoving.StopAllActions();
        enemyAlive = false;
        animator?.SetTrigger("die");

        MoneyManager.Instance.MoneyCreateWorld(transform.position, 10, 1, false, false);

        Invoke(nameof(ResetThis), 2f);
        Invoke(nameof(Despawn), 2f);
    }

    public void EnemyAttackStart()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (PlayerController.Instance != null &&
               PlayerController.Instance.currentState != PlayerStates.Dead)
        {
            if (enemyAlive && isTargetable)
            {
                var playerHealth = PlayerController.Instance.playerHealth;
                if (playerHealth != null && playerHealth.TryGetComponent(out IDamageable iDamageable))
                {
                    animator?.SetTrigger("attack");
                    iDamageable.TakeDamage(enemyDamage, transform, 0, 0);
                }
            }

            yield return new WaitForSeconds(attackCooldown);
        }
    }
}
