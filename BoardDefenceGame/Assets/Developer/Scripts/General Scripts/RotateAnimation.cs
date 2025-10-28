using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    public bool loop = false;
    [SerializeField] float firstPos, lastPos, duration, loopPeriod;
    Quaternion firstRot;
    void Start()
    {
        firstRot = transform.rotation;

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
                transform.rotation = Quaternion.Euler(firstRot.eulerAngles.x, firstRot.eulerAngles.y, firstRot.eulerAngles.z + value);
            }).OnComplete(delegate ()
            {

            });
        return tween;
    }
}
