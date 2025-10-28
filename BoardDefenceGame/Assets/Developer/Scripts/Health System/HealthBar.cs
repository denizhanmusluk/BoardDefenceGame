using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color emptyColor = Color.red;

    public void Initialize(float max, float current)
    {
        slider.maxValue = max;
        slider.value = current;
        UpdateColor();
        UpdateText();
    }

    public void UpdateHealth(float max, float current, float delta)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateHealthChange(max, current, delta));
    }

    private IEnumerator AnimateHealthChange(float max, float current, float delta)
    {
        float duration = 0.25f;
        float startValue = slider.value;
        float endValue = current;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, endValue, elapsed / duration);
            UpdateColor();
            UpdateText();
            yield return null;
        }

        slider.value = endValue;
        UpdateColor();
        UpdateText();
    }

    private void UpdateColor() =>
        fillImage.color = Color.Lerp(emptyColor, fullColor, slider.normalizedValue);

    private void UpdateText()
    {
        if (valueText)
            valueText.text = Mathf.CeilToInt(slider.value).ToString();
    }
}
