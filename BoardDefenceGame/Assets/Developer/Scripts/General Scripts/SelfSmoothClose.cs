using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SelfSmoothClose : MonoBehaviour
{
    [SerializeField] float waitingTime;
    [SerializeField] Color opaqueColor;
    [SerializeField] Color transparentColor;

    private List<TextMeshProUGUI> allChildText = new List<TextMeshProUGUI>();
    private void Awake()
    {
        foreach (var txt in GetComponentsInChildren<TextMeshProUGUI>())
        {
            allChildText.Add(txt);
        }
    }
    private void OnEnable()
    {
        StartCoroutine(ColorSet());
    }
    IEnumerator ColorSet()
    {
        float counter = 0f;
        while(counter < 1f)
        {
            counter += 4 * Time.deltaTime;
            Mathf.Clamp01(counter);
            foreach (var txt in allChildText)
            {
                txt.color = Color.Lerp(transparentColor, opaqueColor, counter);
            }

            yield return null;
        }
        yield return new WaitForSeconds(waitingTime);
        counter = 0f;
        while (counter < 1f)
        {
            counter += 4 * Time.deltaTime;
            Mathf.Clamp01(counter);
            foreach (var txt in allChildText)
            {
                txt.color = Color.Lerp(opaqueColor, transparentColor, counter);
            }

            yield return null;
        }
        gameObject.SetActive(false);
    }
}
