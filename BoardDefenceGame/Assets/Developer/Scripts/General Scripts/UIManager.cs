using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    public Button completeButton;
    public Button failButton;

    public GameObject completePanelGO;
    public GameObject failPanelGO;

    public Transform moneyImg_TR;
    public TextMeshProUGUI moneyText;

    public Slider failWaveProgress;

    public Slider completeWaveProgress;

    public GameObject waveCompleteTextGO;

    Vector3 firstSizeMoney;
    Vector3 firstSizeStar;
    protected override void Awake()
    {
        base.Awake();
        Globals.moneyAmount = PlayerPrefs.GetInt("money");
    }
    private void Start()
    {
        firstSizeMoney = moneyImg_TR.localScale;

        moneyText.text = CoefficientTransformation.Converter(Globals.moneyAmount);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            MoneyUpdate(1000);
        }
    }
    public void MoneyUpdate(int miktar)
    {
        float moneyOld = (float)Globals.moneyAmount;
        Globals.moneyAmount = Globals.moneyAmount + miktar;
        StartCoroutine(SetMoney(miktar, moneyOld));

        PlayerPrefs.SetInt("money", Globals.moneyAmount);
    }

    IEnumerator SetMoney(int amount, float oldAmount)
    {
        int decimalCounter = 0;
        float _speed = 4f;
        float counter = 0f;
        while (counter < 1f)
        {
            counter += _speed * Time.deltaTime;
            float money = Mathf.Lerp(oldAmount, (float)Globals.moneyAmount, counter);
            moneyText.text = CoefficientTransformation.Converter((int)money);

            if (decimalCounter % 25 == 0 && oldAmount > 0)
            {
                MoneyScale(0.8f, 1f, 0.8f, Ease.OutElastic);
            }
            decimalCounter++;
            yield return null;
        }
        moneyText.text = CoefficientTransformation.Converter(Globals.moneyAmount);

    }

    public Tween MoneyScale(float value, float lastValue, float duration, DG.Tweening.Ease type)
    {
        Tween tween = DOTween.To
            (() => value, x => value = x, lastValue, duration).SetEase(type).OnUpdate(delegate ()
            {
                moneyImg_TR.transform.localScale = firstSizeMoney * value;
            }).OnComplete(delegate ()
            {

            });
        return tween;
    }

    public void WinPanelOpen()
    {
        completePanelGO.SetActive(true);
    }
    public void FailPanelOpen()
    {
        failPanelGO.SetActive(true);
    }

    public void RestartLevelButton()
    {
        StartCoroutine(Restart_Delay());
    }
    IEnumerator Restart_Delay()
    {
        failButton.interactable = false;
        yield return new WaitForSeconds(2f);
        GameManager.Instance.RestartLevel();
    }
    public void NextLevelButton()
    {

        StartCoroutine(NextLevelClickDelay());
    }
    IEnumerator NextLevelClickDelay()
    {
        completeButton.interactable = false;
        yield return new WaitForSeconds(3f);

        GameManager.Instance.RestartLevel();
    }
}
