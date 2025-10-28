using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum EnemyType
{
    Enemy_Type_1 = 0,
    Enemy_Type_2 = 1,
    Enemy_Type_3 = 2,
}

[CreateAssetMenu(menuName = "ScriptableObjects/Enemy/AllEnemyList")]
public class AllEnemyList : ScriptableObject
{
    public List<EnemyData> _enemyDatas;
}
