using UnityEngine;

public class EnemyHealthController : HealthControllerBase
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(float damage, Transform attacker, int impulse = 0, int weaponID = -1)
    {
        base.TakeDamage(damage, attacker, impulse, weaponID);

        if (isAlive)
        {
            enemy?.EnemyGetDamage();
        }
    }

    public override void Die(Transform killer)
    {
        base.Die(killer);

        if (enemy == null) return;

        PlayerManager.Instance.UnregisterEnemy(enemy);
        enemy.OnDeath();
    }
}
