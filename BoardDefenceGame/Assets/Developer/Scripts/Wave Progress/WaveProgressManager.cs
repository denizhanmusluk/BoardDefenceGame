using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WaveProgressManager : Singleton<WaveProgressManager>
{
    [Header("UI References")]
    [SerializeField] public Slider waveProgress;
    [SerializeField] private int sliderHeightStep = 255;
    [SerializeField] private List<WaveLevel> waveLevelList = new();
    [SerializeField] private int startPosY_Offset = 0;

    private int _waveCount;
    private int _firstPosY;
    private int _lastPosY;
    private float _progressValue;
    private Coroutine _progressRoutine;
    private Coroutine _posRoutine;

    protected override void Awake()
    {
        base.Awake();
    }
    public void WaveProgressStart(int chapterCount)
    {
        _waveCount = chapterCount;

        if (waveLevelList == null || waveLevelList.Count == 0 || waveProgress == null)
        {
            return;
        }

        for (int i = 0; i < _waveCount && i < waveLevelList.Count; i++)
        {
            waveLevelList[i].gameObject.SetActive(true);
            waveLevelList[i].lvText.text = (i + 1).ToString();
            waveLevelList[i].NextNextLevel();
        }

        int sizeX = sliderHeightStep * _waveCount;
        _firstPosY = sizeX / 2 + startPosY_Offset;
        _lastPosY = -_firstPosY + 250;

        var rect = waveProgress.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(sizeX, 40);
        rect.anchoredPosition = new Vector3(0, _firstPosY, 0);

        waveProgress.value = 0f;
    }
    public void SetWaveLevelSize()
    {
        if (Globals.chapterLevel < 0 || Globals.chapterLevel >= waveLevelList.Count - 1)
            return;

        waveLevelList[Globals.chapterLevel].CurrentLevel();
        waveLevelList[Globals.chapterLevel + 1].CurrentNextLevel();
    }
    public void SetWaveProgress(float progressSpeed, int currentEnemyNo, int totalEnemyCount)
    {
        if (waveProgress == null || totalEnemyCount == 0) return;

        float firstRate = (float)Globals.chapterLevel / _waveCount;
        float lastRate = ((float)Globals.chapterLevel + 1f) / _waveCount;

        if (_progressRoutine != null) StopCoroutine(_progressRoutine);
        if (_posRoutine != null) StopCoroutine(_posRoutine);

        _progressRoutine = StartCoroutine(SmoothProgress(progressSpeed * 1.1f, firstRate, lastRate, currentEnemyNo, totalEnemyCount));
        _posRoutine = StartCoroutine(UpdateProgressY(progressSpeed));
    }

    private IEnumerator SmoothProgress(float speed, float firstRate, float lastRate, float currentEnemyNo, float totalEnemyCount)
    {
        float rateDiff = lastRate - firstRate;
        float startRate = currentEnemyNo / totalEnemyCount;
        float endRate = (currentEnemyNo + 1f) / totalEnemyCount;
        float counter = 0f;

        while (counter < 1f)
        {
            counter += speed * Time.deltaTime;
            counter = Mathf.Clamp01(counter);

            _progressValue = Mathf.Lerp(firstRate + rateDiff * startRate, firstRate + rateDiff * endRate, counter);
            waveProgress.value = _progressValue;
            yield return null;
        }
    }
    private IEnumerator UpdateProgressY(float speed)
    {
        var rect = waveProgress.GetComponent<RectTransform>();
        if (rect == null) yield break;

        float startValue = _progressValue;
        float counter = 0f;

        while (counter < 1f)
        {
            counter += speed * Time.deltaTime;
            counter = Mathf.Clamp01(counter);

            float posY = Mathf.Lerp(_firstPosY, _lastPosY, Mathf.Lerp(startValue, _progressValue, counter));
            rect.anchoredPosition = new Vector3(0, posY, 0);
            yield return null;
        }
    }
}
