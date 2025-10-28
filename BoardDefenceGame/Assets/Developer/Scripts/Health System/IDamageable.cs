using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, Transform attacker, int impulse = 0, int weaponID = -1);
    void Heal(float healAmount);
}
