using System;
using System.Collections.Generic;
using UnityEngine;

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
    public char[] AttributeInitial;
    public ThrowableDdoInfo ThrowableDdoInfo;
}

[Serializable]
public class ThrowableDdoInfo
{
    public bool ThroableDdo;
    public int[] ThrowableDdoIncrements;
    public int[] ThrowableDdoPercentage;

}