using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ItemCreatorSubPanel : Singleton<ItemCreatorSubPanel>
{

    public ItemPoolSettings itemPoolSetting;
    public Transform itemCreatePos;
    public List<Transform> itemSubSlotPosListTR;
    public List<Item> currentItemList;

    public List<Item> itemPoolList = new List<Item>();
    protected override void Awake()
    {
        base.Awake();
    }

    public void FirstItemCreator()
    {
        for (int i = 0; i < itemPoolSetting.itemPoolList.Count;i++)
        {
            itemPoolList.Add(itemPoolSetting.itemPoolList[i]);
        }
        ItemCreator(0);
    }

    int CalculateProbability()
    {
        int totalProbability = 0;

        foreach (var itm in itemPoolList)
        {
            totalProbability += itm.selectionProbability;
        }

        int randomSelectNumber = Random.Range(0, totalProbability);
        int cumulativeProbability = 0;

        for (int i = 0; i < itemPoolList.Count; i++)
        {
            cumulativeProbability += itemPoolList[i].selectionProbability;
            if (randomSelectNumber < cumulativeProbability)
            {
                return i;
            }
        }

        return 0;
    }
    public void ReRoll()
    {
        foreach(var item in currentItemList)
        {
            Destroy(item.gameObject);
        }

        currentItemList.Clear();
    }

    public void ItemCreator(int itemLevel)
    {
        int randomItemSelect = CalculateProbability();
        Item newItem;
        newItem = Instantiate(itemPoolList[randomItemSelect], itemCreatePos.position, itemCreatePos.transform.rotation);
        newItem.itemMergeLevel = itemLevel;
        currentItemList.Add(newItem);
        newItem.itemCreatorSubPanel = this;
        newItem.transform.parent = transform;
        newItem.GoSlot();
    }
}
