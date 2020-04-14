using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Object_Motion : MonoBehaviour
{
    private string flightMode;
    private float flightForce;
    private float weaponDamage;
    private float blastRadius;
    private string[] layer;
    private float flightTime, lerpPct;
    private Vector3 startPoint, finalPoint, hitPoint;

    private delegate void FlightDelegate();
    private FlightDelegate flight; // declare as static- or won't function

    private Ray throwableXZAxis;
    private float instantPos;
    private float runningTime = 0;
    private const float kPositionFrame = .01f;
    private float kAngle;

    private ParticleSystem deathParticle;
    private char[] attributeL;
    private Vector3[] attributeV;

    private void OnEnable()
    {
        flight = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_force"></param>
    /// <param name="damage"></param>
    /// <param name="blastRange">radius of affect upon object landing</param>
    /// <param name="startingPoint">point an object start</param>
    /// <param name="_finalPoint">point the object will land on</param>
    /// <param name="particle">particle upon death</param>
    /// <param name="attributeList"></param>
    /// <param name="attributeValue"></param>
    /// <param name="_layer">layer the object can hit</param>
    /// <param name="flightMode">"quadratic" is quadratic. "linear" is linear</param>
    /// <param name="angle">Optional: angle to shoot in quadratic motion</param>
    /// <param name="_time">Optional: time to complete linear interpolation</param>
    public void setFlight(float _force, float damage, float blastRange, Vector3 startingPoint, Vector3 _finalPoint,
        ParticleSystem hitParticle, char[] attributeList, Vector3[] attributeValue, string[] _layer, string flightMode, float angle = 0, float _time = 0) {
        #region instantiating datas
        flightTime = _time;
        flightForce = _force;
        weaponDamage = damage;
        blastRadius = blastRange;
        deathParticle = hitParticle;
        layer = _layer;
        attributeL = attributeList;
        attributeV = attributeValue;
        #endregion
        startPoint = startingPoint;
        finalPoint = _finalPoint;
        throwableXZAxis = Instant_Reference.getHitParallelRay(startingPoint, finalPoint, _force);
        kAngle = angle;
        instantPos = startingPoint.y;

        if (flightMode == "quadratic") { flight += QuadraticFlight; }
        else { flight += LinearFlight; }
    }

    // straight interpolation
    public void LinearFlight()
    {
        runningTime = runningTime + Time.deltaTime;
        lerpPct = runningTime / flightTime;
        transform.position = Vector3.Lerp(startPoint, finalPoint, lerpPct);
        if (runningTime > flightTime) { endFlight(); }
    }

    // calculate vacuum flight path
    void QuadraticFlight()
    {
        Debug.DrawRay(throwableXZAxis.origin, throwableXZAxis.direction);
        transform.position = new Vector3(throwableXZAxis.GetPoint(Mathf.Cos(Mathf.PI * kAngle / 180) * flightForce * runningTime).x,
                            ((Mathf.Sin(kAngle * Mathf.PI / 180) * flightForce * runningTime) - (.5f * 20f * runningTime * runningTime)) + instantPos,
                            throwableXZAxis.GetPoint(Mathf.Cos(Mathf.PI * kAngle / 180) * flightForce * runningTime).z);
        runningTime += kPositionFrame;
    }


    void endFlight()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        bool hit = false;
        int layermask = LayerMask.GetMask(layer);
        for (int i = 0; i < layer.Length; i++)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer(layer[i])) {hit = true; }
        }
        if(hit) { 
            foreach (Collider c in Physics.OverlapSphere(transform.position, blastRadius, layermask))
            {
                Health_Base.changeEntityHeath(c.gameObject, weaponDamage);
                Instant_Reference.playerRightHand.GetComponent<Weapon_Control>().AddAttributes(c.gameObject, attributeL, attributeV);
            }
            endFlight();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Flight delegate containing desired flight type
        flight?.Invoke();
    }
}
