using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ItemBrokenList : MonoBehaviour
{
    public List<BrokenPart> brokenList = new List<BrokenPart>();

    public void ItemBrake()
    {
        transform.parent = null;
        foreach(var prt in brokenList)
        {
            prt.Brake();
        }
    }
}
