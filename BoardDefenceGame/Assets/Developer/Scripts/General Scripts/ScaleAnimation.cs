using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    [SerializeField] bool loop = false;
    [SerializeField(), Range(1f, 5f)] public float scaleFactor;
    [SerializeField(), Range(0f, 10f)] public float scaleSpeed;
    Vector3 firstScale;
    private void Awake()
    {
        firstScale = transform.localScale;
    }
    void OnEnable()
    {
        StartCoroutine(SwipeMove());
    }
    IEnumerator SwipeMove()
    {
        yield return new WaitForSeconds(0.2f);
        float duration = 1f;
        float counter = 0f;
        float value = 0;
        while (counter < duration)
        {
            if (loop)
            {
                duration += scaleSpeed * Time.deltaTime;
            }
            counter += scaleSpeed * Time.deltaTime;
            value = Mathf.Abs(Mathf.Sin(counter * Mathf.PI));
            value *= (scaleFactor - 1);
            transform.localScale = firstScale * (1 + value);

            yield return null;
        }
    }
    private void OnDisable()
    {
        transform.localScale = firstScale;
        StopAllCoroutines();
    }
}
