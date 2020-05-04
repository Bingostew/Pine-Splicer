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
    private Vector3 startPoint, finalPoint, hitPoint, newAngle;

    private delegate void FlightDelegate();
    private FlightDelegate flight;
    private FlightDelegate flightReset;

    private Ray throwableXZAxis;
    private float instantPos;
    private float runningTime = 0;
    private float cumulativeDistance = 0;
    private float kAngle;

    private const float kPositionFrame = .01f;
    private const float verticalAccerlation = 20f;
    private const float kDetectionDivisor = 10;

    private ParticleSystem deathParticle;
    private char[] attributeL;
    private Vector4[] attributeV;

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
        ParticleSystem hitParticle, char[] attributeList, Vector4[] attributeValue, string[] _layer, string flightMode, float angle = 0, float _time = 0) {
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
        runningTime = 0;

        if (flightMode == "quadratic") { flight += QuadraticFlight; flight += DetectQuadraticHit; flightReset += QuadraticReset;  }
        else { flight += LinearFlight; flight += DetectLinearHit; flightReset += LinearReset; }
    }

    // straight interpolation
    public void LinearFlight()
    {
        runningTime += Time.deltaTime;
        lerpPct = runningTime / flightTime; 
        cumulativeDistance += Time.deltaTime;
        transform.position = Vector3.Lerp(startPoint, finalPoint, lerpPct);
        if (cumulativeDistance > flightTime) { endFlight(); }
    }

    // calculate vacuum flight path
    void QuadraticFlight()
    {
        transform.position = GetQuadraticCoordinates(runningTime);
        runningTime += kPositionFrame;
    }

    private Vector3 GetQuadraticCoordinates(float rt)
    {
        return new Vector3(throwableXZAxis.GetPoint(Mathf.Cos(Mathf.PI * kAngle / 180) * flightForce * rt).x,
                            ((Mathf.Sin(kAngle * Mathf.PI / 180) * flightForce * rt) - (.5f * verticalAccerlation * rt  * rt)) + instantPos,
                            throwableXZAxis.GetPoint(Mathf.Cos(Mathf.PI * kAngle / 180) * flightForce * rt).z);
    }

    private void LinearReset()
    {
        RaycastHit t;
        Vector3 c = finalPoint;
        Vector3 d = startPoint;

        if (Physics.Raycast(new Ray(startPoint, finalPoint - startPoint), out t, 100, LayerMask.GetMask(layer)))
        {
            throwableXZAxis = new Ray(transform.position, Quaternion.AngleAxis(Vector3.Angle(t.normal, d - c), Vector3.Cross(d - c, t.normal)) * t.normal);
        }
    }

    private void QuadraticReset()
    {
        RaycastHit h;
        Vector3 a = GetQuadraticCoordinates(runningTime + kPositionFrame);
        Vector3 b = GetQuadraticCoordinates(runningTime - kPositionFrame);
        instantPos = transform.position.y;
        if (Physics.Raycast(new Ray(b, a - b), out h, LayerMask.GetMask(layer)))
        {
            newAngle = Quaternion.AngleAxis(Vector3.Angle(h.normal, b - a), Vector3.Cross(b - a, h.normal)) * h.normal;
            kAngle = Vector3.Angle(newAngle, new Vector3(newAngle.x, 0, newAngle.z));
            print(kAngle);
        }
    }

    public void RestartFlight( float powerReduction, float bounceRandomness)
    {
        flightReset.Invoke();

        flightForce -= powerReduction;
        finalPoint = Weapon_Control.CalculateBurst(startPoint, throwableXZAxis.GetPoint(flightForce - powerReduction) - startPoint, flightForce, bounceRandomness);
        throwableXZAxis = new Ray(transform.position, Weapon_Control.CalculateBurst(
            transform.position, newAngle, flightForce, bounceRandomness) - transform.position);
        runningTime = 0;
    }

    public void endFlight()
    {
        Instantiate(deathParticle, hitPoint, Quaternion.identity);
        Destroy(gameObject);
    }

    private void DetectQuadraticHit()
    {
        Vector3 v = GetQuadraticCoordinates(runningTime - 5* kPositionFrame);
        RaycastHit h;

        Debug.DrawRay(v, (transform.position-v) );
        if (Physics.Raycast(new Ray(v, transform.position-v), out h, flightForce / kDetectionDivisor, LayerMask.GetMask(layer)))
        {
            hitPoint = h.point;
            Instant_Reference.playerRightHand.GetComponent<Weapon_Control>().AddAttributes(h.transform.gameObject, attributeL, attributeV, weaponDamage, gameObject);
        }
    }

    private void DetectLinearHit()
    {
        Vector3 v = Vector3.Lerp(startPoint, finalPoint, lerpPct - Time.deltaTime);
        RaycastHit h;

        if (Physics.Linecast(startPoint, transform.position, out h, LayerMask.GetMask(layer)))
        {
            if ((h.point - transform.position).magnitude < flightForce / flightTime / kDetectionDivisor)
            {
                Instant_Reference.playerRightHand.GetComponent<Weapon_Control>().AddAttributes(h.transform.gameObject, attributeL, attributeV, weaponDamage, gameObject);
                startPoint = h.point;
                hitPoint = h.point;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Flight delegate containing desired flight type
        flight?.Invoke();
    }
}
