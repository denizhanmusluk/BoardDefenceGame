using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Player References")]
    public GameObject playerGO;

    [Header("Enemy Tracking")]
    public List<Enemy> enemyList = new();

    protected override void Awake()
    {
        base.Awake();
    }

    #region Enemy Management
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        if (!enemyList.Contains(enemy))
            enemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        if (enemyList.Contains(enemy))
            enemyList.Remove(enemy);

        LevelManager.Instance?.CheckEnemyClear();
    }
    #endregion
}
