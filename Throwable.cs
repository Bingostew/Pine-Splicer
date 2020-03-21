using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using StatEffect;
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
    private Throwable_Object.MotionMode flightMode;
    private string[] attributeList;
    private Vector2[] attributeValues;
    private int[] attributeIndex;

    private void OnEnable()
    {
        //Instant_Reference.playerReference.GetComponent<Player_Controller>().SendMessage("setPlayerAttackInt", 1);
        // 1 is "throwable" in "playerAttackEnum"
        #region EvokeInfo
        damage = throwableObject.ThrowableDamage;   force = throwableObject.ThrowableForce;     time = throwableObject.ThrowableTime;
        ddoInc = throwableObject.ThrowableDdoInfo.ThrowableDdoIncrements;
        ddoPer = throwableObject.ThrowableDdoInfo.ThrowableDdoPercentage;
        flightMode = throwableObject.FlightMode;
        attributeList = throwableObject.ThrowableAttributes.ThrowableAttributeList;
        attributeValues = throwableObject.ThrowableAttributes.ThrowableAttributeValues;
        attributeIndex = new int[attributeList.Length];
        #endregion
        Event_Controller.addAttackStream(ThrowObject);
        DebugStats();
        AddAttribute();
    }

    private void Start()
    {
        Event_Controller.addAttackStream(BeginAttributes);
    }

    //start the attribute of the weapons
    public void BeginAttributes()
    {
        AttributeInstance.beginModifier(ref Player_Controller.speedModifier, attributeIndex[0]);
    }

    public void ThrowObject()
    {
        weapon.transform.SetParent(null);
    
        switch (flightMode)
        {
            case Throwable_Object.MotionMode.linear:
                weapon.AddComponent<Object_Motion>().setLinearFlight(force, time);
                break;
            case Throwable_Object.MotionMode.quadratic:
                weapon.AddComponent<Object_Motion>().setQuadraticFlight(force);
                break;
        }
        
    }

    // Any addition attributes special to a particular weapon
    private void AddAttribute ()
    {
        for (int i = 0; i < attributeList.Length; i ++)
        {
            switch (attributeList[i])
            {
                case "s": attributeIndex[i] = AttributeInstance.moveSpeedInstance(attributeValues[i].x, attributeValues[i].y); break;
            }
        }
    }

    #region debugger
    void DebugStats()
    {
        if(time == 0)
        {
            Debug.LogError("Object Motion Flight Time can not be 0", this);
        }

        if(attributeValues.Length != attributeList.Length)
        {
            Debug.LogError("Missing either values for attribute or extra values for attributes", this);
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
    }

    private void FixedUpdate()
    {
    }
}
