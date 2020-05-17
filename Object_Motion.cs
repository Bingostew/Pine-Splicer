using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Object_Motion : MonoBehaviour
{
    public GameObject hitObject;

    private string flightMode;
    private float flightForce;
    private float weaponDamage, weaponCrit;
    private float blastRadius;
    private string[] layer;
    private float flightTime, lerpPct;
    private Vector3 startPoint, finalPoint, hitPoint, newAngle, hitNormal;

    private delegate void FlightDelegate();
    private FlightDelegate flight;
    private FlightDelegate flightReset;

    private Ray throwableXZAxis;
    private float instantPos;
    private float runningTime = 0;
    private float cumulativeDistance = 0;
    private float positionFrame = 0;
    private float kAngle;

    public static float verticalAccerlation = 10f;
    private const float kDetectionDivisor = 15;
    private const float kMaxPositionFrame = 2;
    private const float kMaxInelasticFactor = .5f;

    private ParticleSystem deathParticle;

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
    public void setFlight(float _force, float damage, float critDamage, float blastRange, Vector3 startingPoint, Vector3 _finalPoint,
        ParticleSystem hitParticle, string[] _layer, string flightMode, float angle = 0, float _time = 0) {
        #region instantiating datas
        flightTime = _time;
        flightForce = _force;
        weaponDamage = damage;
        weaponCrit = critDamage;
        blastRadius = blastRange;
        deathParticle = hitParticle;
        layer = _layer;
        positionFrame = Time.deltaTime * (kMaxPositionFrame - flightTime);
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
        float t = Mathf.Abs(2 * Mathf.Sin(kAngle) * flightForce / verticalAccerlation);
        transform.position = GetQuadraticCoordinates(runningTime);
        runningTime += positionFrame;
    }

    private Vector3 GetQuadraticCoordinates(float rt)
    {
        return new Vector3(throwableXZAxis.GetPoint(Mathf.Cos(Mathf.PI * kAngle / 180) * flightForce * rt).x,
                            ((Mathf.Sin(kAngle * Mathf.PI / 180) * flightForce * rt) - (.5f * verticalAccerlation * rt  * rt)) + instantPos,
                            throwableXZAxis.GetPoint(Mathf.Cos(Mathf.PI * kAngle / 180) * flightForce * rt).z);
    }

    private void LinearReset()
    {
        throwableXZAxis = new Ray(transform.position, Quaternion.AngleAxis(Vector3.Angle(hitNormal, startPoint - finalPoint), 
            Vector3.Cross(startPoint - finalPoint, hitNormal)) * hitNormal);
    }

    private void QuadraticReset()
    {
        Vector3 b = GetQuadraticCoordinates(runningTime - 4 * Time.deltaTime);
        instantPos = transform.position.y;
        newAngle = Quaternion.AngleAxis(Vector3.Angle(hitNormal, b - transform.position), Vector3.Cross(b - transform.position, hitNormal)) * hitNormal;
        kAngle = Vector3.Angle(newAngle, new Vector3(newAngle.x, 0, newAngle.z));
        float inelasticDepletion = Vector3.Angle(hitNormal, b - transform.position) / 100;
        flightForce *= inelasticDepletion > kMaxInelasticFactor ? inelasticDepletion : kMaxInelasticFactor; // reduction of force due to inelastic collisions

    }

    public void RestartFlight(float powerReduction, float bounceRandomness)
    {

        flightReset?.Invoke();
        flightForce -= powerReduction; // reduction of force due to friction
        finalPoint = Weapon_Control.CalculateBurst(startPoint, throwableXZAxis.GetPoint(flightForce - powerReduction) - startPoint, flightForce, bounceRandomness);
        throwableXZAxis = new Ray(transform.position, Weapon_Control.CalculateBurst(
            transform.position, new Vector3(newAngle.x, 0 ,newAngle.z), flightForce, bounceRandomness) - transform.position);
        runningTime = 0;
    }

    public void endFlight()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void DetectQuadraticHit()
    {
        Vector3 v = GetQuadraticCoordinates(runningTime - 2* Time.deltaTime);
        RaycastHit h;

        if (Physics.Raycast(new Ray(v, transform.position-v), out h, flightForce / kDetectionDivisor, LayerMask.GetMask(layer)))
        {
            hitNormal = h.normal;
            hitPoint = h.point;
            OnHit(h);
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
                hitNormal = h.normal;
                startPoint = h.point;
                hitPoint = h.point;
                OnHit(h);
            }
        }
    }

    private void OnHit(RaycastHit h) // get all object hit from given point. Customized to fit blastradius
    {
        if (blastRadius == 0)
        {
            Weapon_Control.OnObjectHit(h.transform.gameObject, gameObject, weaponDamage, weaponCrit);
        }
        else
        {
            Collider[] hitObjects = Physics.OverlapSphere(h.point, blastRadius);
            List<GameObject> objectList = new List<GameObject>();
            foreach (Collider c in hitObjects)
            {
                if (!objectList.Contains(Weapon_Control.GetHealthBaseObject(c.gameObject))){ objectList.Add(Weapon_Control.GetHealthBaseObject(c.gameObject)); }
            }
            foreach (GameObject gb in objectList)
            {
                Weapon_Control.OnObjectHit(gb, gameObject, weaponDamage, weaponCrit);
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Flight delegate containing desired flight type
        flight?.Invoke();
        Debug.DrawRay(throwableXZAxis.origin, throwableXZAxis.direction);
    }
}
