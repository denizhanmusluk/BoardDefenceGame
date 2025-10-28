using System.Collections.Generic;
using UnityEngine;

public abstract class HealthControllerBase : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected bool isAlive = true;

    //public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public delegate void DeathEvent(Transform killer);
    public event DeathEvent OnDeath;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        healthBar?.Initialize(maxHealth, currentHealth);
    }
    public void HealthInitialize(float healthValue)
    {
        isAlive = true;
        healthBar?.gameObject.SetActive(true);
        currentHealth = maxHealth = healthValue;
        healthBar?.Initialize(maxHealth, currentHealth);
    }
    public virtual void TakeDamage(float damage, Transform attacker, int impulse = 0, int weaponID = -1)
    {
        if (!isAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        healthBar?.UpdateHealth(maxHealth, currentHealth, damage);

        if (currentHealth <= 0f)
        {
            Die(attacker);
        }
    }

    public virtual void Heal(float amount)
    {
        if (!isAlive) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        healthBar?.UpdateHealth(maxHealth, currentHealth, -amount);
    }

    public virtual void Die(Transform killer)
    {
        isAlive = false;
        healthBar?.gameObject.SetActive(false);
        OnDeath?.Invoke(killer);
    }

    public void ResetHealth()
    {
        isAlive = true;
        currentHealth = maxHealth;
        healthBar?.Initialize(maxHealth, currentHealth);
    }
}
