using EZ_Pooling;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [Header("References")]
    public Transform banknotePrefab;
    public RectTransform moneyUI_Target_TR;
    public RectTransform moneySpawnPosRECT;

    [Header("Settings")]
    [SerializeField] private float baseDistance = 5f;

    protected override void Awake()
    {
        base.Awake();
    }

    public void MoneyCreate(Vector3 createPos, int moneyCount, int banknoteValue, bool shakeActive, bool scaleActive)
    {
        if (moneyCount <= 0 || banknoteValue <= 0)
            return;

        for (int i = 0; i < moneyCount; i++)
        {
            SpawnManager.Instance.MoneySpawn(
                banknotePrefab,
                createPos,
                Quaternion.identity,
                moneyUI_Target_TR,
                banknoteValue,
                shakeActive,
                scaleActive,
                true
            );
        }
    }
    public void MoneyCreateUIPosToWorld(int moneyCount, int banknoteValue, bool shakeActive, bool scaleActive)
    {
        if (Camera.main == null || moneySpawnPosRECT == null)
            return;

        Camera cam = Camera.main;
        float distance = CalculateZOffset(cam.fieldOfView);

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, moneySpawnPosRECT.position);
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distance));
        worldPos.z = 0f;

        for (int i = 0; i < moneyCount; i++)
        {
            SpawnManager.Instance.MoneySpawn(
                banknotePrefab,
                worldPos,
                Quaternion.identity,
                moneyUI_Target_TR,
                banknoteValue,
                shakeActive,
                scaleActive,
                true
            );
        }
    }
    public void MoneyCreateWorld(Vector3 worldPos, int moneyCount, int banknoteValue, bool shakeActive, bool scaleActive)
    {
        if (moneyCount <= 0)
            return;

        for (int i = 0; i < moneyCount; i++)
        {
            SpawnManager.Instance.MoneySpawn(
                banknotePrefab,
                worldPos,
                Quaternion.identity,
                moneyUI_Target_TR,
                banknoteValue,
                shakeActive,
                scaleActive,
                true
            );
        }
    }

    private float CalculateZOffset(float camFov)
    {
        float fovFactor = Mathf.Tan(camFov * 0.5f * Mathf.Deg2Rad);
        return baseDistance / fovFactor;
    }
}
