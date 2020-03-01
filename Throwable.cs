using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using WeaponAttributes;
/*
 * Top Class for weapons: Throwables, Shootables, and Melees
 * Use as template for all throwables.
 */

public class Throwable : MonoBehaviour
{
    [SerializeField]
    private Throwable_Object throwableObject;
    [SerializeField]
    private GameObject weapon;


    private float damage, force, time;
    private int[] ddoInc, ddoPer;
    private string flightMode;

    private void OnEnable()
    {
        //Instant_Reference.playerReference.GetComponent<Player_Controller>().SendMessage("setPlayerAttackInt", 1);
        // 1 is "throwable" in "playerAttackEnum"
        #region EvokeInfo
        damage = throwableObject.ThrowableDamage;   force = throwableObject.ThrowableForce;     time = throwableObject.ThrowableTime;
        ddoInc = throwableObject.ThrowableDdoInfo.ThrowableDdoIncrements;
        ddoPer = throwableObject.ThrowableDdoInfo.ThrowableDdoPercentage;
        flightMode = throwableObject.FlightMode.ToString();
        #endregion
        Event_Controller.attackEvent += () => ThrowObject();
        AttributeInstance.slownessInstance(3, 2f);
    }

    public void ThrowObject()
    {
        weapon.transform.SetParent(null);
        if (!weapon.GetComponent<Object_Motion>())
        {
            weapon.AddComponent<Object_Motion>().setMotionInfo(flightMode, force, time);
        }
    }

    // Any addition attributes special to a particular weapon
    private void AddAttribute (Action a)
    {
        
    }

    #region debugger
    void DebugStats()
    {
        if(time == 0)
        {
            Debug.LogError("Object Motion Flight Time can not be 0", this);
        }
    }
    #endregion

    #region Quadratic Curve Example Code
    /* void Start()
     {
         ln = GetComponent<LineRenderer>();
         ln.positionCount = 50;
         test();
     }
     void test()
     {
         for (float i = 1; i < 51; i++)
         {
             float t = i / 50;
             pos[(int)i - 1] = calculateLinearPt(t, new Vector3(1, 1, 2), new Vector3(10, 10, 10));
             pos[(int)i - 1] = calculateQuadraticPt(t, new Vector3(1, 1, 2), new Vector3(10, 10, 10), new Vector3(2, 2, 20));
             print(i / 50);
         }
         ln.SetPositions(pos);
     }
     Vector3 calculateLinearPt(float t, Vector3 p0, Vector3 p1)
     {
         return p0 + t * (p1 - p0);
     }
     Vector3 calculateQuadraticPt(float t, Vector3 p0, Vector3 p1, Vector3 p2)
     {
         return ((1 - t) * (1 - t) * p0) + (2 * (1 - t) * t * p1) + ((t * t) * p2);
     }
     */
    #endregion

    // Update is called once per frame
    void Update()
    {
        DebugStats();
    }

    private void FixedUpdate()
    {
    }
}
