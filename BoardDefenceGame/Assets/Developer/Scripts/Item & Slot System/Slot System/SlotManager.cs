using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : Singleton<SlotManager>
{
    [Header("References")]
    public List<SlotPanel> slotPanels = new();
    public List<Item> allItemSlot = new();

    [Header("State")]
    public bool allSlotIsFull = false;

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
    }
    #endregion

    #region Slot Panel Management

    public void CheckAllSlotPanel()
    {
        List<Item> itemsToRemove = new();

        foreach (var item in allItemSlot)
        {
            if (item == null) continue;

            if (item.slotCount > item.targetSlotList.Count)
            {
                itemsToRemove.Add(item);

                if (item.targetSlot != null && item.targetSlot.slotPanel != null)
                {
                    var panel = item.targetSlot.slotPanel;
                    panel.itemInsideSlot.Remove(item);
                    panel.RemoveInsideGunsSlot(item);
                    panel.UpdateInsideGuns();
                }
            }
        }

        foreach (var item in itemsToRemove)
        {
            item.GoSubPanel();
            allItemSlot.Remove(item);

            foreach (var slot in item.targetSlotList)
            {
                if (slot != null)
                    slot.currentItem = null;
            }
        }
    }

    public void SlotPanelReset(int slotID)
    {
        if (slotID < 0 || slotID >= slotPanels.Count)
            return;

        var panel = slotPanels[slotID];

        foreach (var item in panel.itemInsideSlot)
        {
            if (item == null) continue;

            item.gameObject.SetActive(true);
            item.GoSubPanel();
            allItemSlot.Remove(item);
        }

        foreach (var slot in panel.allSlots)
        {
            if (slot != null)
                slot.currentItem = null;
        }

        panel.itemInsideSlot.Clear();
        StartCoroutine(CheckAllSlot_Delay());
    }

    private IEnumerator CheckAllSlot_Delay()
    {
        yield return new WaitForSeconds(0.1f);
        CheckAllSlotPanel();
    }

    #endregion

    #region Slot Visuals

    private IEnumerator OpenDelay(int handID)
    {
        yield return new WaitForSeconds(0.1f);

        if (handID < 0 || handID >= slotPanels.Count)
            yield break;

        foreach (var slot in slotPanels[handID].allSlots)
        {
            if (slot != null)
                slot.NormalColorInit();
        }
    }

    #endregion

    #region State Check

    public void AllSlotFullCheck()
    {
        allSlotIsFull = true;

        foreach (var panel in slotPanels)
        {
            if (panel == null || panel.allSlots.Count == 0)
                continue;

            if (panel.allSlots[0].currentItem == null)
            {
                allSlotIsFull = false;
                break;
            }
        }
    }

    #endregion
}
