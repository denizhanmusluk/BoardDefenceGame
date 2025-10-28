using System.Collections;
using UnityEngine;
using EZ_Pooling;

public class Money : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetTR;
    public int banknotValue;
    public Transform spriteTR;
    public Transform spriteFollowTR;

    [Header("Settings")]
    [SerializeField] private float _speed = 6f;
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private float shakePeriod = 0.05f;
    [SerializeField] private Vector2 shakeBond = new(0.2f, 0.2f);

    private Camera _mainCam;
    private Coroutine _activeRoutine;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    public void Despawn() => EZ_PoolManager.Despawn(transform);

    public void MoneyStart(bool shakeActive, bool scaleActive, bool moneyEarnActive)
    {
        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);

        spriteTR.localScale = scaleActive ? Vector3.one * 0.1f : Vector3.one * 0.2f;
        _activeRoutine = StartCoroutine(MoneyRoutine(shakeActive, scaleActive, moneyEarnActive));
    }

    private IEnumerator MoneyRoutine(bool shakeActive, bool scaleActive, bool moneyEarnActive)
    {
        if (shakeActive)
        {
            yield return StartCoroutine(FirstMoneyCreate(scaleActive));
            yield return StartCoroutine(ShakeAnimation(moneyEarnActive));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(MoveUI(false, moneyEarnActive));
        }
    }

    private IEnumerator FirstMoneyCreate(bool scaleActive)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(
            Random.Range(scaleActive ? -0.15f : -0.35f, scaleActive ? 0.15f : 0.35f),
            Random.Range(scaleActive ? -0.15f : -0.35f, scaleActive ? 0.15f : 0.35f),
            0f
        );

        float t = 0f;
        const float speed = 6f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    private IEnumerator ShakeAnimation(bool moneyEarnActive)
    {
        float timeCounter = 0f;
        float shakeTimer = 0f;

        while (timeCounter < duration)
        {
            timeCounter += Time.deltaTime;
            shakeTimer += Time.deltaTime;

            if (shakeTimer > shakePeriod)
            {
                shakeTimer = 0f;
                spriteFollowTR.localPosition = new Vector2(
                    Random.Range(-shakeBond.x, shakeBond.x),
                    Random.Range(-shakeBond.y, shakeBond.y)
                );
            }

            spriteTR.position = Vector3.Lerp(
                spriteTR.position,
                spriteFollowTR.position,
                Time.deltaTime * _speed
            );

            yield return null;
        }

        yield return StartCoroutine(MoveUI(true, moneyEarnActive));
    }

    private IEnumerator MoveUI(bool scaleActive, bool moneyEarnActive)
    {
        if (_mainCam == null)
            _mainCam = Camera.main;

        Vector3 startPos = transform.position;
        Vector3 startScale = spriteTR.localScale;
        Vector3 targetScale = Vector3.one * 0.2f;

        float distance = CalculateZOffset(_mainCam);
        float t = 0f;
        float speed = 1f;
        float accel = Random.Range(2f, 8f);

        while (t < 1f)
        {
            t += Time.deltaTime * 0.75f * speed;
            speed = Mathf.Lerp(1f, accel, t);

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_mainCam, targetTR.position);
            Vector3 worldPos = _mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distance));
            Vector3 lerpedPos = Vector3.Lerp(startPos, worldPos, t);

            transform.position = Vector3.Lerp(startPos, new Vector3(lerpedPos.x, worldPos.y, lerpedPos.z), t);
            spriteTR.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        transform.position = targetTR.position;

        if (moneyEarnActive)
            UIManager.Instance.MoneyUpdate(banknotValue);

        Despawn();
    }

    private float CalculateZOffset(Camera cam)
    {
        const float baseDistance = 5f;
        float fovFactor = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return baseDistance / fovFactor;
    }
}
