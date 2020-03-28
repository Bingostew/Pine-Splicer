using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatEffect;

public class Object_Motion : MonoBehaviour
{
    private string flightMode;
    private float flightForce;
    private float weaponDamage;
    private float blastRadius;
    private float flightTime, lerpPct;
    private Vector3 startPoint, finalPoint, hitPoint;

    private delegate void FlightDelegate();
    private FlightDelegate flight; // declare as static- or won't function

    private Ray throwableXZAxis;
    private float instantRHYPos;
    private float runningTime = 0;
    private const float kPositionFrame = .01f;
    private float kAngle;

    private ParticleSystem deathParticle;

    private void OnEnable()
    {
        flight = null;
    }
    public void setLinearFlight( float _force, float _time, float damage, float blastRange, ParticleSystem particle)
    {
        #region instantiating data
        flight += LinearFlight;
        flightTime = _time;
        weaponDamage = damage;
        blastRadius = blastRange;
        deathParticle = particle;
        #endregion
        startPoint = Instant_Reference.getRightHandPosition();
        finalPoint = Instant_Reference.getRightHandToHitVector(_force);
    }

    public void setQuadraticFlight(float _force, float damage, float blastRange, ParticleSystem particle)  
    {
        #region instantiating data
        flight += QuadraticFlight;
        flightForce = _force;
        deathParticle = particle;
        weaponDamage = damage;
        blastRadius = blastRange;
        #endregion
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 2) {
            int layermask = 1 << 1;
            foreach(Collider c in Physics.OverlapSphere(other.ClosestPoint(transform.position), blastRadius, layermask))
            {
                Health_Base.changeEntityHeath(c.gameObject, weaponDamage);
                Instant_Reference.playerRightHand.GetComponent<Throwable>().AddAttribute(c.gameObject);
            }
            Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Flight delegate containing desired flight type
        flight?.Invoke();
    }
}
