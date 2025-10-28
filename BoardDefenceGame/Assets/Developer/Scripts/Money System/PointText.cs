using System.Collections;
using UnityEngine;
using TMPro;
using EZ_Pooling;

public class PointText : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color defaultColor = Color.white;
    [Range(0, 1)] [SerializeField] private float alpha = 0f;
    [Range(0, 20)] [SerializeField] private float upwardSpeed = 10f;
    [SerializeField] private Vector2 randomSpeed = new Vector2(2f, 5f);

    [Header("References")]
    public TextMeshProUGUI pointText;

    [HideInInspector] public int pointValue;
    [HideInInspector] public string pointString;

    private float simulationSpeed;
    private float lifeTimer;

    private Coroutine _activeRoutine;

    private void OnEnable()
    {
        lifeTimer = 0f;
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > 1.25f)
        {
            lifeTimer = 0f;
            StopAllCoroutines();
            Despawn();
        }
    }

    public void Despawn() => EZ_PoolManager.Despawn(transform);

    public void PointInit(bool isEnemy)
    {
        StartAnimation(isEnemy ? Color.red : defaultColor, pointValue.ToString());
    }

    public void PointInitString()
    {
        StartAnimation(defaultColor, pointString);
    }

    public void PointInitHealth()
    {
        StartAnimation(Color.green, $"+{pointValue}");
    }

    private void StartAnimation(Color color, string text)
    {
        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);

        pointText.color = color;
        pointText.text = text;

        simulationSpeed = Random.Range(randomSpeed.x, randomSpeed.y);
        _activeRoutine = StartCoroutine(PointUp());
    }

    private IEnumerator PointUp()
    {
        float counter = 0f;

        while (counter < Mathf.PI / 2)
        {
            counter += simulationSpeed * Time.deltaTime;
            float spd = Mathf.Cos(counter) * upwardSpeed;
            transform.position = Vector3.MoveTowards(
                transform.position,
                transform.position + Vector3.up,
                Time.deltaTime * spd
            );
            yield return null;
        }

        StartCoroutine(FadeOut(alpha));
    }

    private IEnumerator FadeOut(float targetAlpha)
    {
        float counter = 0f;
        Color initialColor = pointText.color;

        while (counter < Mathf.PI / 2)
        {
            counter += 4f * simulationSpeed * Time.deltaTime;
            float t = counter / (Mathf.PI / 2);
            float currentAlpha = Mathf.Abs(targetAlpha - t);

            pointText.color = new Color(
                initialColor.r,
                initialColor.g,
                initialColor.b,
                currentAlpha
            );

            yield return null;
        }

        Despawn();
    }
}
