using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveLevel : MonoBehaviour
{
    public Transform parentTR;
    public GameObject current_BG;
    public GameObject nextLV_BG;
    public TextMeshProUGUI lvText;

    Vector3 firstSize;

    private void Awake()
    {
        firstSize = parentTR.localScale;
    }

    public void SizeUp()
    {
        Scale(1f, 1.5f, 0.8f, Ease.OutElastic);
    }
    public void SizeDown()
    {
        Scale(1.5f, 1f, 0.8f, Ease.OutElastic);
    }
    public void NextNextLevel()
    {
        nextLV_BG.SetActive(true);
        current_BG.SetActive(false);
    }
    public void CurrentNextLevel()
    {
        SizeUp();
        nextLV_BG.SetActive(true);
        current_BG.SetActive(false);
    }
    public void CurrentLevel()
    {
        SizeDown();
        nextLV_BG.SetActive(false);
        current_BG.SetActive(true);
    }

    public Tween Scale(float value, float lastValue, float duration, DG.Tweening.Ease type)
    {
        Tween tween = DOTween.To
            (() => value, x => value = x, lastValue, duration).SetEase(type).OnUpdate(delegate ()
            {
                parentTR.transform.localScale = firstSize * value;
            }).OnComplete(delegate ()
            {

            });
        return tween;
    }
}
