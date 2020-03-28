using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Enemy_Controller : MonoBehaviour, ObjectHealth
{
    [SerializeField]
    private Enemy_Object enemyObject;

    private CharacterController enemyCC;

    private delegate void EnemyMovementDelegate();
    private event EnemyMovementDelegate moveMode;

    private float enemySpeed, enemyStopRange;

    public float enemySpeedModifier = 1;

    void Start()
    {
        #region stat declaration
        enemySpeed = enemyObject.EnemySpeed;
        enemyStopRange = enemyObject.EnemyStopRange;
        #endregion
        enemyCC = GetComponent<CharacterController>();
        enemySpeedModifier = 1;

        switch (enemyObject.EnemyMoveMode)
        {
            case Enemy_Object.MoveMode.aggressive: SetAggroMove(); break;
        }

        SpawnHealth(100);
    }

    public void SpawnHealth(float health)
    {
        Health_Base.addEntityHealth(gameObject, health);
    }

    
    private void SetAggroMove()
    {
        moveMode += AggressiveMovement;
    }

    // charges at player with linear speed
    private void AggressiveMovement()
    {        Vector3 enemyToPlayerVec = new Vector3(Instant_Reference.getPlayerPosition().x, 0 , Instant_Reference.getPlayerPosition().z)  
            - new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(Instant_Reference.getPlayerPosition(), transform.position) > enemyStopRange)
        {
            enemyCC.SimpleMove(enemyToPlayerVec * Time.deltaTime * enemySpeed * enemySpeedModifier);
        }
        print("en speed" + enemySpeedModifier);
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        print(Health_Base.getEntityHeath(gameObject));
        moveMode?.Invoke();
    }
}
