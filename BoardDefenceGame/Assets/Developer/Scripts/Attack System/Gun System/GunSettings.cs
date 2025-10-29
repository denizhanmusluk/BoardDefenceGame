using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item/GunSettings")]
public class GunSettings : ScriptableObject
{
    public float attackDamage;
    public float verticalRange;
    public float horizontalRange;
    public float attackCooldown;
}
