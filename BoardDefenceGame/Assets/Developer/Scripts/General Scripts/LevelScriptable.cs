using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ChapterCount
{
    public int[] _enemyCount;
}
[CreateAssetMenu(menuName = "ScriptableObjects/LevelSet")]

public class LevelScriptable : ScriptableObject
{
    [Header("CHAPTERS")]
    public ChapterCount[] chapters;
}
