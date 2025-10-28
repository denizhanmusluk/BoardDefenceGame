using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotBuyButton : MonoBehaviour
{
    public Button button;
    public int id;
    public TextMeshProUGUI costText;
    public List<int> costList;
    private int currentCost;

    private void Start()
    {
        int costLvl = Globals.currentLevelIndex;
        if (costLvl >= costList.Count)
            costLvl = costList.Count - 1;

        currentCost = costList[costLvl];
        costText.text = currentCost.ToString("F0");
    }

    public void BuyClick()
    {
        SlotBuyManager.Instance.BuySlot(id, currentCost);
    }

    public void ButtonActiveCheck()
    {
        button.interactable = Globals.currentEnergy >= currentCost;
    }
}
