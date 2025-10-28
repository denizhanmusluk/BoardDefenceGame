using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public struct EnemyDataSet
{
    public EnemyType _enemyType;
    public int enemyCount;
}
[System.Serializable]
public struct Chapter
{
    public List<EnemyDataSet> _enemySetList;
}
[CreateAssetMenu(menuName = "ScriptableObjects/Enemy/EnemySettings")]
public class EnemySetting : ScriptableObject
{
    public List<Chapter> _chapter;
}
