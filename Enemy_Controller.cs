using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Enemy_Controller : MonoBehaviour, ObjectHealth
{
    [SerializeField]
    private Enemy_Object enemyObject;
    private Shootable_Object enemyShootable;

    private GameObject enemyWp;
    private CharacterController enemyCC;

    private delegate void EnemyDelegate();
    private event EnemyDelegate moveMode;
    private event EnemyDelegate attackMode;

    private Ray enemyToPlayerRay;

    private float enemySpeed, enemyStopRange;
    private float runningTime;
    private string[] layermask = new string[] { "Player", "Terrain"};

    public float enemySpeedModifier = 1;

    void Start()
    {
        #region stat declaration
        enemySpeed = enemyObject.EnemySpeed;
        enemyStopRange = enemyObject.EnemyStopRange;
        enemyWp = enemyObject.EnemyWeapon;
        enemyShootable = enemyWp.GetComponent<Weapon_Data>().shootableObject;
        #endregion
        enemyCC = GetComponent<CharacterController>();

        enemySpeedModifier = 1;

        SetAttackMode();
        switch (enemyObject.EnemyMoveMode)
        {
            case Enemy_Object.MoveMode.aggressive: moveMode += AggressiveMovement; break;
        }
        SpawnHealth(enemyObject.EnemyHealth);
    }

    public void SpawnHealth(float health)
    {
        Health_Base.addEntityHealth(gameObject, health);
    }

    private void SetAttackMode()
    {
        switch (enemyObject.EnemyAttackMethod)
        {
            case Enemy_Object.EnemyAttacMode.Shoot:
                attackMode += ShootAttack;
                break;
        }
    }

    // charges at player with linear speed
    private void AggressiveMovement()
    {        Vector3 enemyToPlayerVec = new Vector3(Instant_Reference.getPlayerPosition().x, 0 , Instant_Reference.getPlayerPosition().z)  
            - new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(Instant_Reference.getPlayerPosition(), transform.position) > enemyStopRange)
        {
            enemyCC.SimpleMove(enemyToPlayerVec * Time.deltaTime * enemySpeed * enemySpeedModifier);
        }
    }


    private void ShootAttack()
    {
        enemyToPlayerRay = new Ray(transform.position, Instant_Reference.getPlayerPosition() - transform.position);
        // arcsin((ax)/(2m^2)
        float arcAngle = Mathf.Asin(20 * Vector3.Distance(transform.position, 
            new Vector3(Instant_Reference.getPlayerPosition().x, 0 ,Instant_Reference.getPlayerPosition().z)) /
            (2 * enemyShootable.ShootableForce * enemyShootable.ShootableForce)) * 180  / Mathf.PI;
        GameObject enemyWeapon = Instantiate(enemyWp, transform.position, Quaternion.identity);
        enemyWeapon.GetComponent<Object_Motion>().setFlight(
            enemyShootable.ShootableForce,
            enemyShootable.ShootableDamage,
            enemyShootable.ShootableBlastRange,
            transform.position,
            Weapon_Control.CalculateBurst(transform.position, enemyToPlayerRay.GetPoint(enemyShootable.ShootableForce) - transform.position,
            enemyShootable.ShootableForce, enemyShootable.ShootableBurst.BaseBurst[0]),
            enemyShootable.HitParticle,
            enemyShootable.ShootableAttributes.ShootableAttributeList, enemyShootable.ShootableAttributes.ShootableAttributeValues,
            layermask, enemyShootable.BulletMode.ToString(), arcAngle, enemyShootable.ShootableTime);
    }

    private void FireRate()
    {
        runningTime += Time.deltaTime;
        if(runningTime > enemyShootable.ShootableFireRate)
        {
            runningTime = 0;
            attackMode?.Invoke();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveMode?.Invoke();
        FireRate();
    }
}
