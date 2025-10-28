using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float totalHealth;
    public float maxHealth;
    public float currentHealth;

    [Header("Cannon Settings")]
    public int gunId;
    public bool targetAlive = true;
    public Transform gunPosTR;
    public Transform gunBaseTR;
    public Animator cannonAnimator;
    public ParticleSystem attackParticle;

    [Header("Gun Management")]
    public List<GunSystem> currentGunList = new();
    public SlotPanel _slotPanel;

    private void Start() => InitializeHealth();

    #region Health
    public void InitializeHealth()
    {
        totalHealth = maxHealth;
        currentHealth = totalHealth;
    }

    void IDamageable.TakeDamage(float damage, Transform fromWhat, int impulse, int weaponID)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (currentHealth <= 0) HandleDeath(fromWhat);
    }

    public void Heal(float regenAmount)
    {
        StartCoroutine(HealthRegenDelay(regenAmount));
    }

    private IEnumerator HealthRegenDelay(float regenAmount)
    {
        yield return new WaitForSeconds(1f);

        SpawnManager.Instance.HealthAddPointSpawn(transform.position + Vector3.up * 1.5f, (int)regenAmount);

        float regenValue = currentHealth + ((maxHealth / 100f) * regenAmount);
        currentHealth = Mathf.Min(regenValue, maxHealth);
    }
    #endregion

    #region Gun Management
    public void CreateGun(List<GunSystem> gunPrefabs, List<Item> items)
    {
        RemoveAllGuns();

        for (int i = 0; i < gunPrefabs.Count; i++)
        {
            GunSystem gun = Instantiate(gunPrefabs[i], gunPosTR.position, gunPosTR.rotation, gunPosTR);
            currentGunList.Add(gun);

            gun.RegisterGun();
            gun.GunStart();
            if (i < items.Count)
                gun.currentItem = items[i];
        }
    }

    private void RemoveAllGuns()
    {
        foreach (var gun in currentGunList)
        {
            if (gun != null)
            {
                gun.RemoveGun();
                Destroy(gun.gameObject);
            }
        }
        currentGunList.Clear();
    }
    #endregion

    #region Death
    private void HandleDeath(Transform fromWhat)
    {
        cannonAnimator?.SetTrigger("death");
        targetAlive = false;

        RemoveAllGuns();
        PlayerController.Instance.RemoveCannon(this);

        StartCoroutine(CloseDelay());
    }

    private IEnumerator CloseDelay()
    {
        yield return new WaitForSeconds(2f);
        _slotPanel.SlotPanelReset();
        _slotPanel.gameObject.SetActive(false);
    }
    #endregion

    #region Rotation & Animation
    public void TurnToTargetRotation(Transform targetTr)
    {
        if (targetTr == null) return;
        Vector3 direction = (targetTr.position + Vector3.up * 5f) - gunBaseTR.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg * 0.5f;
        StartCoroutine(SmoothRot(angle));
    }

    private IEnumerator SmoothRot(float angle)
    {
        Quaternion startRot = gunBaseTR.rotation;
        float t = 0f;
        const float speed = 8f;

        while (t < 1f)
        {
            t += speed * Time.deltaTime;
            gunBaseTR.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(gunBaseTR.eulerAngles.x, angle, gunBaseTR.eulerAngles.z), t);
            yield return null;
        }
    }

    public void AttackAnimation()
    {
        cannonAnimator?.SetTrigger("attack");
        attackParticle?.Play();
    }
    #endregion
}
