using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class UpDownAnimation : MonoBehaviour
{
    public bool loop = false;
    [SerializeField] float firstPos, lastPos, duration, loopPeriod;
    Vector3 firstPosition;
    void Awake()
    {
        firstPosition = transform.localPosition;

    }

    // Update is called once per frame
    void OnEnable()
    {
        StartCoroutine(LoopAnim());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator LoopAnim()
    {
        OpenScale(firstPos, lastPos, duration, Ease.OutElastic);

        while (true && loop)
        {
            yield return new WaitForSeconds(loopPeriod);
            OpenScale(firstPos, lastPos, duration, Ease.OutElastic);
        }
    }
    public Tween OpenScale(float value, float lastValue, float duration, DG.Tweening.Ease type)
    {

        Tween tween = DOTween.To
            (() => value, x => value = x, lastValue, duration).SetEase(type).OnUpdate(delegate ()
            {
            transform.localPosition = firstPosition + new Vector3(0,value,0);
            }).OnComplete(delegate ()
            {

            });
        return tween;
    }
}
