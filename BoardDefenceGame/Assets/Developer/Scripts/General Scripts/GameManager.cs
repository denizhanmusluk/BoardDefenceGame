using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        GameEventSystem.GameWin += Win;
        GameEventSystem.GameFail += Fail;
    }
    public void Win()
    {
        SaveGlobalData();
        StartCoroutine(HandleWinProgress());
    }

    public void Fail()
    {
        UIManager.Instance.FailPanelOpen();
        StartCoroutine(HandleFailProgress());
    }

    public void RestartLevel()
    {
        ResetGlobalData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ResetGlobalData()
    {
        Globals.chapterLevel = 0;
        Globals.currentEnergy = 0f;
        Globals.earnedMoney_inLevel = 0;
    }

    private void SaveGlobalData()
    {
        PlayerPrefs.SetInt("money", Globals.moneyAmount);
    }

    private IEnumerator HandleFailProgress()
    {
        float counter = 0f;
        while (counter < 1f)
        {
            counter += Time.deltaTime;
            UIManager.Instance.failWaveProgress.value = Mathf.Lerp(0f, ((float)Globals.chapterLevel + 1f) / Globals.chapterCount, counter);
            yield return null;
        }
    }

    private IEnumerator HandleWinProgress()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Instance.WinPanelOpen();

        float counter = 0f;
        while (counter < 1f)
        {
            counter += Time.deltaTime;
            UIManager.Instance.completeWaveProgress.value = Mathf.Lerp(0f, 1f, counter);
            yield return null;
        }
    }
}
