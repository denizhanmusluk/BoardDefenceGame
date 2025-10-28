using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotBuyManager : Singleton<SlotBuyManager>
{
    [Header("Prefabs & References")]
    public List<Cannon> cannonPrefabList = new List<Cannon>();
    public List<SlotPanel> slotPanelLisForCannon = new();
    public List<GameObject> buyPanelList = new();
    public List<Button> buyButtonList = new();
    public List<TextMeshProUGUI> buyTextList = new();

    [Header("Settings")]
    public List<int> cannonCost = new();
    public int defaultSlotCount = 8;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Globals.cannonBuyLevel = PlayerPrefs.GetInt("cannonlevel", 0);
    }

    public void StartGame()
    {
        FirstCreateCannon();
        CheckEnoughMoney();
    }

    public void CheckEnoughMoney()
    {
        for (int i = 0; i < buyButtonList.Count; i++)
        {
            if (i >= cannonCost.Count || i >= buyTextList.Count)
                continue;

            int cost = cannonCost[i];
            buyTextList[i].text = cost.ToString("F0");

            buyButtonList[i].interactable = Globals.moneyAmount >= cost;
        }
    }

    private void FirstCreateCannon()
    {
        int totalCannon = Mathf.Min(defaultSlotCount + Globals.cannonCount, cannonPrefabList.Count);

        for (int i = 0; i < totalCannon; i++)
        {
            if (i >= slotPanelLisForCannon.Count)
                continue;

            slotPanelLisForCannon[i].gameObject.SetActive(true);
            PlayerController.Instance.CreateCannon(cannonPrefabList[i], i);
        }
    }

    public void BuySlot(int slotID, int slotCost)
    {
        if (slotID < 0 || slotID >= slotPanelLisForCannon.Count)
            return;

        if (slotCost > Globals.currentEnergy)
            return;

        slotPanelLisForCannon[slotID].gameObject.SetActive(true);
        PlayerController.Instance.CreateCannon(cannonPrefabList[slotID], slotID);

        // Enerjiyi düþür
        EnergyManager.Instance.ModifyEnergy(-slotCost);
        CheckEnoughMoney();
    }

    public void BuyCannon_Button()
    {
        int buyLevel = Globals.cannonBuyLevel;

        if (buyLevel >= cannonCost.Count || Globals.moneyAmount < cannonCost[buyLevel])
            return;

        UIManager.Instance.MoneyUpdate(-cannonCost[buyLevel]);
        Globals.cannonBuyLevel++;

        if (Globals.cannonBuyLevel < slotPanelLisForCannon.Count)
        {
            slotPanelLisForCannon[Globals.cannonBuyLevel].gameObject.SetActive(true);
            PlayerController.Instance.CreateCannon(
                cannonPrefabList[Globals.cannonBuyLevel],
                Globals.cannonBuyLevel
            );
        }

        if (buyLevel < buyPanelList.Count)
            buyPanelList[buyLevel].SetActive(false);

        if (Globals.cannonBuyLevel < buyPanelList.Count)
            buyPanelList[Globals.cannonBuyLevel].SetActive(true);

        CheckEnoughMoney();
    }
}
