using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawners : Singleton<EnemySpawners>
{
    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnTransforms = new List<Transform>();
    [SerializeField] private float waveDuration = 10f;
    [SerializeField] public float moveSpeedFactor = 5f;

    [Header("Enemy Data References")]
    [SerializeField] private EnemySetting[] enemySetList;
    [SerializeField] private AllEnemyList allEnemyList;

    private EnemySetting _currentEnemySet;
    private readonly List<EnemyData> _enemyDataList = new List<EnemyData>();
    private int _spawnCounter;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameEventSystem.RPG_Start += SpawnStart;
        InitializeEnemySet();
    }

    private void InitializeEnemySet()
    {
        if (enemySetList == null || enemySetList.Length == 0)
        {
            Debug.LogError("❌ Enemy Set List boş! Inspector'dan eklenmeli.");
            return;
        }

        _currentEnemySet = enemySetList[Globals.currentLevelIndex % enemySetList.Length];
        Globals.chapterCount = _currentEnemySet._chapter.Count;
        PrepareEnemyList();
    }

    private void PrepareEnemyList()
    {
        _enemyDataList.Clear();

        if (_currentEnemySet == null || Globals.chapterLevel >= _currentEnemySet._chapter.Count)
        {
            Debug.LogWarning("⚠️ Geçersiz chapter seviyesi!");
            return;
        }

        var currentChapter = _currentEnemySet._chapter[Globals.chapterLevel];
        foreach (var set in currentChapter._enemySetList)
        {
            int enemyId = (int)set._enemyType;
            if (enemyId < 0 || enemyId >= allEnemyList._enemyDatas.Count)
                continue;

            for (int i = 0; i < set.enemyCount; i++)
                _enemyDataList.Add(allEnemyList._enemyDatas[enemyId]);
        }

        Shuffle(_enemyDataList);
    }

    public void SpawnStart()
    {
        if (_currentEnemySet == null)
        {
            Debug.LogWarning(" Geçerli düşman seti bulunamadı!");
            return;
        }

        PrepareEnemyList();
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        WaveProgressManager.Instance.SetWaveLevelSize();

        for (int i = 0; i < _enemyDataList.Count; i++)
        {
            if (spawnTransforms.Count == 0)
            {
                Debug.LogError(" Spawn Transform listesi boş!");
                yield break;
            }

            int spawnPointId = _spawnCounter % spawnTransforms.Count;
            _spawnCounter++;

            Vector3 spawnPos = spawnTransforms[spawnPointId].position;
            spawnPos.z = 0f;

            Transform targetTR = PlayerController.Instance.followPositions[spawnPointId];

            SpawnManager.Instance.SpawnEnemy(_enemyDataList[i], spawnPos, Quaternion.identity, targetTR);

            float waitTime = waveDuration / Mathf.Max(1, _enemyDataList.Count);
            WaveProgressManager.Instance.SetWaveProgress((1 / waitTime), i, _enemyDataList.Count);
            yield return new WaitForSeconds(waitTime);
        }

        if (Globals.chapterLevel < _currentEnemySet._chapter.Count - 1)
        {
            LevelManager.Instance.OnChapterComplete();
        }
    }
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
