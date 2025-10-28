using EZ_Pooling;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] Transform pointText_TR;
    public void HitParticleSpawn(Transform hitParticleTR, Vector3 pos, Quaternion rot)
    {
        Transform explosionTR = EZ_PoolManager.Spawn(hitParticleTR, pos, rot);
        if (explosionTR.GetComponent<BulletExplosionParticle>() != null)
        {
            BulletExplosionParticle projectile = explosionTR.GetComponent<BulletExplosionParticle>();
            projectile.ParticlePlay();
        }
    }

    public void SpawnBossProjectile(GunSystem gunSystem, Transform projectileTR, Vector3 pos, Quaternion rot, Transform targetTr, float speed, float rotSpeed, float dmg)
    {
        Transform projectileTr = EZ_PoolManager.Spawn(projectileTR, pos, rot);

        Projectile projectile = projectileTr.GetComponent<Projectile>();
        projectile.targetTr = targetTr;
        projectile.speed = speed;
        projectile.rotationSpeed = rotSpeed;
        projectile.damage = dmg;
        projectile.gunSystem = gunSystem;

        if (targetTr.GetComponent<Enemy>() != null)
        {
            projectile.enemy = targetTr.GetComponent<Enemy>();
        }
        projectile.GoTargetProjectile();
    }
    public void EnergySpawn(Transform moneyPrefab, Vector3 pos, Quaternion rot, RectTransform targetTR, int banknoteValue, bool shakeActive, bool scaleActive)
    {
        Transform _money = EZ_PoolManager.Spawn(moneyPrefab, pos, rot);
        EnergyPoint banknote = _money.GetComponent<EnergyPoint>();
        banknote.banknotValue = banknoteValue;
        banknote.targetTR = targetTR;
        banknote.EnergyStart(shakeActive, scaleActive);
    }
    public void MoneySpawn(Transform moneyPrefab , Vector3 pos, Quaternion rot, RectTransform targetTR, int banknoteValue, bool shakeActive , bool scaleActive, bool moneyEarnActive)
    {
        Transform _money = EZ_PoolManager.Spawn(moneyPrefab, pos, rot);
        Money banknote = _money.GetComponent<Money>();
        banknote.banknotValue = banknoteValue;
        banknote.targetTR = targetTR;
        banknote.MoneyStart(shakeActive , scaleActive , moneyEarnActive);
    }

    public void DamagePointSpawn(Vector3 pos, int damageValue, bool isEnemy)
    {
        Quaternion spawnRot = Quaternion.Euler(-32.84f, 0, 0);

        Transform dmpPoint = EZ_PoolManager.Spawn(pointText_TR, pos, spawnRot);
        PointText pntTxt = dmpPoint.GetComponent<PointText>();
        pntTxt.pointValue = damageValue;
        pntTxt.PointInit(isEnemy);
    }
    public void HealthAddPointSpawn(Vector3 pos, int healthRegenAmount)
    {
        Quaternion spawnRot = Quaternion.Euler(-32.84f, 0, 0);

        Transform dmpPoint = EZ_PoolManager.Spawn(pointText_TR, pos, spawnRot);
        PointText pntTxt = dmpPoint.GetComponent<PointText>();
        pntTxt.pointValue = (int)(healthRegenAmount);
        pntTxt.PointInitHealth();
    }

    public void SpawnCrossBow(GunSystem gunSystem, Transform projectileTR, Vector3 pos, Quaternion rot, Transform targetTr, float speed, float rotSpeed, float dmg, float attackRange)
    {
        Transform projectileTr = EZ_PoolManager.Spawn(projectileTR, pos, rot);

        Projectile projectile = projectileTr.GetComponent<Projectile>();
        projectile.targetTr = targetTr;
        projectile.speed = speed;
        projectile.rotationSpeed = rotSpeed;
        projectile.damage = dmg;
        projectile.attackRange = attackRange;
        projectile.gunSystem = gunSystem;

        if (targetTr.GetComponent<Enemy>() != null)
        {
            projectile.enemy = targetTr.GetComponent<Enemy>();
        }
        projectile.GoTargetProjectile();
    }

    public void SpawnBoomerang(GunSystem gunSystem, Transform projectileTR, Vector3 pos, Quaternion rot, Transform targetTr, float speed, float rotSpeed, float dmg, float attackRange)
    {
        Transform projectileTr = EZ_PoolManager.Spawn(projectileTR, pos, rot);

        Projectile projectile = projectileTr.GetComponent<Projectile>();
        projectile.targetTr = targetTr;
        projectile.speed = speed;
        projectile.rotationSpeed = rotSpeed;
        projectile.damage = dmg;
        projectile.attackRange = attackRange;
        projectile.gunSystem = gunSystem;

        if (targetTr.GetComponent<Enemy>() != null)
        {
            projectile.enemy = targetTr.GetComponent<Enemy>();
        }
        projectile.GoTargetProjectile();
    }

    public void SpawnBomb(GunSystem gunSystem, Transform projectileTR, Vector3 pos, Quaternion rot, Transform targetTr, float speed, float rotSpeed, float dmg, float explosionRadius, float attackRange)
    {
        Transform projectileTr = EZ_PoolManager.Spawn(projectileTR, pos, rot);

        Projectile projectile = projectileTr.GetComponent<Projectile>();
        projectile.targetTr = targetTr;
        projectile.speed = speed;
        projectile.rotationSpeed = rotSpeed;
        projectile.damage = dmg;
        projectile.attackRange = attackRange;
        projectile.gunSystem = gunSystem;
        projectile.explosionRadius = explosionRadius;
        if (targetTr.GetComponent<Enemy>() != null)
        {
            projectile.enemy = targetTr.GetComponent<Enemy>();
        }
        projectile.GoTargetProjectile();
    }
    public void SpawnEnemy(EnemyData _enemyData, Vector3 pos, Quaternion rot, Transform targetTr)
    {
        Transform _enemyTransform = EZ_PoolManager.Spawn(_enemyData.enemyPrefab.transform, pos, rot);
        Enemy _enemy = _enemyTransform.GetComponent<Enemy>();
        _enemy.GetComponent<EnemyHealthController>().HealthInitialize(_enemyData.enemyHealth);
        _enemy.enemyDamage = _enemyData.enemyDamage;
        _enemy.attackCooldown = _enemyData._attackCooldown;
        _enemy.aiMoving.walkSpeed = _enemyData.moveSpeed * EnemySpawners.Instance.moveSpeedFactor;
        _enemy.followPosTR = targetTr;
        _enemy.SpawnStart();
    }
}
