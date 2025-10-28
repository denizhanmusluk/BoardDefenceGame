using System.Collections;
using UnityEngine;

public class PlayerHealthController : HealthControllerBase
{
    [Header("Regeneration Settings")]
    [SerializeField] private float regenAmount = 2f;
    [SerializeField] private float regenDelay = 3f;
    [SerializeField] private float regenInterval = 0.5f;

    private Coroutine regenRoutine;

    public override void TakeDamage(float damage, Transform attacker, int impulse = 0, int weaponID = -1)
    {
        base.TakeDamage(damage, attacker, impulse, weaponID);

        if (regenRoutine != null)
            StopCoroutine(regenRoutine);

        if (isAlive)
            regenRoutine = StartCoroutine(RegenAfterDelay());
    }

    private IEnumerator RegenAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay);

        while (currentHealth < maxHealth && isAlive)
        {
            Heal(regenAmount);
            yield return new WaitForSeconds(regenInterval);
        }
    }

    public override void Die(Transform killer)
    {
        base.Die(killer);
        PlayerController.Instance.Die();
    }
}
