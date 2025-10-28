using System.Collections.Generic;
using UnityEngine;

public class SlotPanel : MonoBehaviour
{
    [Header("References")]
    public List<Item> itemInsideSlot;
    public List<Slot> allSlots;
    public ItemCreatorSubPanel itemCreatorSubPanel;

    [Header("Settings")]
    public int slotPanelId;
    public bool recycleActive = false;

    private void Start()
    {
        if (!recycleActive)
            SlotManager.Instance.slotPanels.Add(this);
    }

    public void AddInsideSlotGuns(Item _item)
    {
        if (_item == null)
            return;

        UpdateInsideGuns();
    }

    public void RemoveInsideGunsSlot(Item _item)
    {
        if (_item == null)
            return;

        itemInsideSlot.Remove(_item);
        UpdateInsideGuns();
    }

    public void UpdateInsideGuns()
    {
        List<GunSystem> currentGunsInSlot = new List<GunSystem>();
        List<Item> itemList = new List<Item>();
        foreach (var _item in itemInsideSlot)
        {
            currentGunsInSlot.Add(_item.itemGunList[_item.itemMergeLevel]);
            itemList.Add(_item);
        }

        foreach (var cn in PlayerController.Instance.activeCannons)
        {
            if (cn.gunId == slotPanelId)
            {
                cn.CreateGun(currentGunsInSlot, itemList);
            }
        }
    }
    public void SlotPanelReset()
    {
        foreach (var itm in itemInsideSlot)
        {
            if (itm == null)
                continue;

            itm.gameObject.SetActive(true);
            itm.GoSubPanel();
        }

        foreach (var slot in allSlots)
        {
            if (slot != null)
                slot.currentItem = null;
        }

        itemInsideSlot.Clear();
    }
}
