using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemPool")]
public class ItemPoolSettings : ScriptableObject
{
    public List<Item> itemPoolList;
}
