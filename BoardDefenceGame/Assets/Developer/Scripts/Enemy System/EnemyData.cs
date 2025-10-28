using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public GameObject enemyPrefab;
    public float enemyHealth;
    public float enemyDamage;
    public float _attackCooldown;
    public float moveSpeed;
}
