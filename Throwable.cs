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

    private Throwable_Object.MotionMode flightMode;
    private char[] attributeList;
    private Vector3[] attributeValues;
    private int[] attributeIndex;

    private void OnEnable()
    {
        //Instant_Reference.playerReference.GetComponent<Player_Controller>().SendMessage("setPlayerAttackInt", 1);
        // 1 is "throwable" in "playerAttackEnum"
        #region EvokeInfo
        flightMode = throwableObject.FlightMode;
        attributeList = throwableObject.ThrowableAttributes.ThrowableAttributeList;
        attributeValues = throwableObject.ThrowableAttributes.ThrowableAttributeValues;
        attributeIndex = new int[attributeList.Length];
        #endregion
        Event_Controller.addAttackStream(ThrowObject);
        DebugStats();
    }

    // initiate object flight
    public void ThrowObject()
    {
        weapon = transform.GetChild(0).GetChild(0).gameObject;
        weapon.transform.SetParent(null);
        if (weapon.GetComponent<Object_Motion>() == null) { weapon.AddComponent<Object_Motion>(); }

        if (weapon.GetComponent<Object_Motion>() != null)
        {
            print(flightMode);
            switch (flightMode)
            {
                case Throwable_Object.MotionMode.linear:
                    weapon.GetComponent<Object_Motion>().setLinearFlight(
                        throwableObject.ThrowableForce,
                        throwableObject.ThrowableTime,
                        throwableObject.ThrowableDamage,
                        throwableObject.ThrowableBlastRange,
                        throwableObject.ThrowableParticle);
                    break;
                case Throwable_Object.MotionMode.quadratic:
                    weapon.GetComponent<Object_Motion>().setQuadraticFlight(
                        throwableObject.ThrowableForce, 
                        throwableObject.ThrowableDamage,
                        throwableObject.ThrowableBlastRange,
                        throwableObject.ThrowableParticle);
                    break;
            }
        }

        weapon.GetComponent<Animation>().Play();
    }

    // Any addition attributes special to a particular weapon
    public void AddAttribute (GameObject gb)
    {
        for (int i = 0; i < attributeList.Length; i ++)
        {
            AttributeInstance.beginModifier(attributeValues[i].x, attributeValues[i].y, attributeList[i], gb, attributeValues[i].z);
        }
    }


    #region debugger
    void DebugStats()
    {
        if(throwableObject.ThrowableTime == 0)
        {
            Debug.LogError("Object Motion Flight Time can not be 0", this);
        }

        if(attributeValues.Length != attributeList.Length)
        {
            Debug.LogError("Missing either values for attribute or extra values for attributes", this);
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
    }
}
