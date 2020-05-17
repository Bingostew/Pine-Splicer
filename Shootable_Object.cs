using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "Shootable Object SO", menuName = "Scriptable Weapon(Shootable)")]
public class Shootable_Object : ScriptableObject
{
    public enum BulletType
    {
        linear,
        quadratic,
        hitscan
    }
    public BulletType BulletMode;
    public float ShootableDamage;
    public float ShootableCriticalDamage;
    public float ShootableForce;
    public float ShootableTime;
    public float ShootableFireRate;
    public float ShootableAmount;
    public float ShootableLatency;
    public float ShootableBlastRange;
    public float ReloadSpeed;
    public int Durability;
    public int Ammo;
    public GameObject Bullet;
    public RectTransform Crosshair;
    public ParticleSystem HitParticle;
    public ParticleSystem ShootParticle;
    public ShootableBurst ShootableBurst;
    public ShootableDdoInfo ShootableDdoInfo;
    public ShootableAttributes ShootableAttributes;
}

[Serializable]
public class ShootableBurst
{
    public Vector2 BaseBurst;
    public Vector2 WalkBurst;
    public Vector2 RunBurst;
    public Vector2 AimBurst;
    public Vector2 JumpBurst;
    public Vector2 AttackBurstIncrement;
    public float MaxAttackBurst;
    public float BurstRestorationSpeed;
    public float UnAttackBurstSpeed;
}

[Serializable]
public class ShootableAttributes
{
    public char[] ShootableAttributeList;
    public Vector4[] ShootableAttributeValues;
}

[Serializable]
public class ShootableDdoInfo
{
    public bool ShootableDdo;
    public int[] ShootableDdoIncrements;
    public int[] ShootableDdoPercentage;

}