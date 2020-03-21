using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Motion : MonoBehaviour
{
    private string flightMode;
    private float flightForce;
    private float flightTime, lerpPct;
    private Vector3 startPoint, finalPoint, hitPoint;

    private delegate void FlightDelegate();
    private FlightDelegate flight;

    private Ray throwableXZAxis;
    private float instantRHYPos;
    float runningTime = 0;
    private const float kPositionFrame = .01f;
    float kAngle;

    // set only when motion is needed and as soon as component is instantiated
    public void setLinearFlight( float _force, float _time)
    {
        flight = LinearFlight;
        flightTime = _time;
        startPoint = Instant_Reference.getRightHandPosition();
        finalPoint = Instant_Reference.getRightHandToHitVector(_force);
    }

    public void setQuadraticFlight(float _force)  
    {
        flight = QuadraticFlight;
        flightForce = _force;
        instantRHYPos = Instant_Reference.getRightHandPosition().y;
        throwableXZAxis = Instant_Reference.getRightHandToHitRayParallel(_force, transform.position);
        kAngle = Vector3.Angle(Instant_Reference.getPlayerStraightRay().direction, Instant_Reference.getPlayerCamStraightRay().direction);
        if (Instant_Reference.getPlayerCamStraightRay().direction.y < 0) { kAngle *= -1; }
    }

    // straight interpolation
    public void LinearFlight()
    {
        runningTime = runningTime + Time.deltaTime;
        lerpPct = runningTime/flightTime;
        transform.position = Vector3.Lerp(startPoint, finalPoint , lerpPct);
        if(runningTime > flightTime) { Destroy(this); }
    }

    // calculate vacuum flight path
    void QuadraticFlight()
    { 
        transform.position = new Vector3(throwableXZAxis.GetPoint(Mathf.Cos(Mathf.PI * kAngle / 180) * flightForce * runningTime).x,
                            ((Mathf.Sin(kAngle * Mathf.PI / 180) * flightForce * runningTime) - (.5f * 20f * runningTime * runningTime)) + instantRHYPos,
                            throwableXZAxis.GetPoint(Mathf.Cos(Mathf.PI * kAngle / 180) * flightForce * runningTime).z);
        runningTime += kPositionFrame;
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
        //Flight delegate containing desired flight type
        flight();
    }
}
