using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Motion : MonoBehaviour
{
    private string flightMode;
    private float flightTime, runningTime, lerpPct;
    private Vector3 startPoint, finalPoint, hitPoint;

    private delegate void flightModeDelegate();
    private event flightModeDelegate linearFlightEvent;
    private event flightModeDelegate quadraticFlightEvent;

    // set only when motion is needed and as soon as component is instantiated
    public void setMotionInfo(string _flightMode, float _force, float _time)
    {
        flightMode = _flightMode;
        flightTime = _time;
        startPoint = Instant_Reference.rightHandPosition;
        finalPoint = Instant_Reference.getHitPoint(_force);
    }

    // straight interpolation
    public void LinearFlight()
    {
        runningTime = runningTime + Time.deltaTime;
        lerpPct = runningTime/flightTime;
        transform.position = Vector3.Lerp(startPoint, finalPoint , lerpPct);
        Debug.DrawRay(startPoint, finalPoint - startPoint);
    }

    // calculate vacuum flight path
    void QuadraticFlight()
    {

    }

    void HomingFlight()
    {

    }

    void EndFlight()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        LinearFlight();
        if (runningTime > flightTime) { EndFlight(); }
    }
}
