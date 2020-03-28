using System;
using System.Collections.Generic;
using UnityEngine;
using StatEffect;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "Throwable Object SO", menuName = "Scriptable Weapon(Throwable)")]


public class Throwable_Object : ScriptableObject
{
    public enum MotionMode
    {
        linear,
        quadratic,
        homing
    }
    public MotionMode FlightMode;
    public float ThrowableDamage;
    public float ThrowableForce;
    public float ThrowableTime;
    public float ThrowableBlastRange;
    public ParticleSystem ThrowableParticle;
    public ThrowableDdoInfo ThrowableDdoInfo;
    public ThrowableAttributes ThrowableAttributes;
}

[Serializable]
public class ThrowableAttributes
{
    public char[] ThrowableAttributeList;
    public Vector3[] ThrowableAttributeValues;
}

[Serializable]
public class ThrowableDdoInfo
{
    public bool ThroableDdo;
    public int[] ThrowableDdoIncrements;
    public int[] ThrowableDdoPercentage;

}