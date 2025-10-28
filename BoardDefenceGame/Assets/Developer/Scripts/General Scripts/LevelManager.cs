using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class LevelData
{
    public List<int> levels;
}

public class LevelManager : Singleton<LevelManager>
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Start Settings")]
    [SerializeField] private int startLevelIndex = 0;
    [SerializeField] private int startChapterIndex = 0;

    private bool _levelEndCheck = true;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        // Level index güncellemesi
        if (startLevelIndex > 0)
        {
            Globals.currentLevelIndex = startLevelIndex;
            PlayerPrefs.SetInt("level", Globals.currentLevelIndex);
        }

        StartCoroutine(DelayedChapterInit());
        LoadLevelUI();
    }

    private IEnumerator DelayedChapterInit()
    {
        yield return new WaitForSeconds(1.5f);
        if (startChapterIndex > 0)
            Globals.chapterLevel = startChapterIndex;
    }

    private void LoadLevelUI()
    {
        Globals.currentLevelIndex = PlayerPrefs.GetInt("level", Globals.currentLevelIndex);
        if (levelText != null)
            levelText.text = $"Level {Globals.currentLevelIndex + 1}";
    }

    /// <summary>
    /// Düþman listesini kontrol eder ve tamamlandýysa bitiþi tetikler.
    /// </summary>
    public void CheckEnemyClear()
    {
        if (PlayerManager.Instance.enemyList.Count == 0)
            HandleFinish();
    }

    private void HandleFinish()
    {
        if (!_levelEndCheck) return;

        bool lastChapter = Globals.chapterLevel >= Globals.chapterCount - 1;
        if (lastChapter)
        {
            _levelEndCheck = false;
            EndLevel();
        }
        else
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        // Bir sonraki dalgayý baþlat
        EnemySpawners.Instance.SpawnStart();
    }

    public void OnChapterComplete()
    {
        Globals.chapterLevel++;
        StartCoroutine(ChapterCompleteRoutine());
    }

    private IEnumerator ChapterCompleteRoutine()
    {
        MoneyManager.Instance.MoneyCreateUIPosToWorld(12, 1, true, false);
        Globals.earnedMoney_inLevel += 12;

        if (UIManager.Instance?.waveCompleteTextGO != null)
        {
            UIManager.Instance.waveCompleteTextGO.SetActive(true);
            yield return new WaitForSeconds(2f);
            UIManager.Instance.waveCompleteTextGO.SetActive(false);
        }

        EnemySpawners.Instance.SpawnStart();
    }

    private void EndLevel()
    {
        KillAllEnemies();

        Globals.currentLevelIndex++;
        PlayerPrefs.SetInt("level", Globals.currentLevelIndex);

        GameEventSystem.GunStopEvent();
        GameEventSystem.GameWinEvent();
    }

    private void KillAllEnemies()
    {
        var enemies = PlayerManager.Instance.enemyList;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
                enemies[i].OnDeath();
        }

        enemies.Clear();
    }
}
