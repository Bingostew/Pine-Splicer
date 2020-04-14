using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "Enemy Object SO", menuName = "Scriptable Enemy")]


public class Enemy_Object : ScriptableObject
{
    public enum MoveMode
    {
        aggressive,
        passive,
        swirl
    }

    public enum EnemyAttacMode
    {
        Melee,
        Shoot,
        Throw
    }

    public float EnemyHealth;
    public float EnemySpeed;
    public float EnemyStopRange;
    public MoveMode EnemyMoveMode;
    public EnemyAttacMode EnemyAttackMethod;
    public GameObject EnemyWeapon;
    public EnemyAttributes EnemyAttributes;
}

[Serializable]
public class EnemyAttributes
{
    public string[] EnemyAttributeList;
    public Vector2[] EnemyAttributeValues;
}