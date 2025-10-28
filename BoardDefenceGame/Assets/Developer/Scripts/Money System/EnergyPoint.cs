using EZ_Pooling;
using System.Collections;
using UnityEngine;

public class EnergyPoint : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetTR;
    public int banknotValue;
    public Transform spriteTR;
    public Transform spriteFollowTR;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float shakePeriod = 0.05f;
    [SerializeField] private Vector2 shakeBounds = new(0.15f, 0.15f);

    private Camera _mainCam;
    private Coroutine _activeRoutine;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    public void Despawn() => EZ_PoolManager.Despawn(transform);

    public void EnergyStart(bool shakeActive, bool scaleActive)
    {
        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);

        spriteTR.localScale = scaleActive ? Vector3.one * 0.5f : Vector3.one;
        _activeRoutine = StartCoroutine(MoneySequence(shakeActive, scaleActive));
    }

    private IEnumerator MoneySequence(bool shakeActive, bool scaleActive)
    {
        if (shakeActive)
        {
            yield return StartCoroutine(SpawnShake(scaleActive));
            yield return StartCoroutine(PerformShake());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return StartCoroutine(MoveToUI(scaleActive));
    }

    private IEnumerator SpawnShake(bool scaleActive)
    {
        Vector3 startPos = transform.position;
        Vector3 randomOffset = scaleActive
            ? new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f))
            : new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f));

        Vector3 targetPos = startPos + randomOffset;
        float counter = 0f;
        const float speed = 4f;

        while (counter < 1f)
        {
            counter += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, targetPos, counter);
            yield return null;
        }
    }

    private IEnumerator PerformShake()
    {
        float timer = 0f;
        float shakeTimer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            shakeTimer += Time.deltaTime;

            if (shakeTimer > shakePeriod)
            {
                shakeTimer = 0f;
                spriteFollowTR.localPosition = new Vector2(
                    Random.Range(-shakeBounds.x, shakeBounds.x),
                    Random.Range(-shakeBounds.y, shakeBounds.y)
                );
            }

            spriteTR.position = Vector3.Lerp(spriteTR.position, spriteFollowTR.position, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    private IEnumerator MoveToUI(bool scaleActive)
    {
        if (_mainCam == null)
            _mainCam = Camera.main;

        Vector3 startPos = transform.position;
        Vector3 startScale = spriteTR.localScale;
        Vector3 endScale = Vector3.one * 0.5f;

        float counter = 0f;
        float baseSpeed = 1f;
        float accel = Random.Range(2f, 8f);
        float distance = CalculateZOffset(_mainCam);

        while (counter < 1f)
        {
            counter += Time.deltaTime * baseSpeed * 0.75f;
            baseSpeed = Mathf.Lerp(1f, accel, counter);

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_mainCam, targetTR.position);
            Vector3 targetWorldPos = _mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distance));

            transform.position = Vector3.Lerp(startPos, targetWorldPos, counter);
            spriteTR.localScale = Vector3.Lerp(startScale, endScale, counter);

            yield return null;
        }

        transform.position = targetTR.position;
        EnergyManager.Instance.ModifyEnergy(banknotValue);
        Despawn();
    }

    private float CalculateZOffset(Camera cam)
    {
        const float baseDistance = 5f;
        float fovFactor = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return baseDistance / fovFactor;
    }
}
