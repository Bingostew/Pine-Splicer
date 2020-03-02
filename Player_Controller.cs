using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: FIX: 
        //-Throwable Scriptable
        //-Jitter Collider
public class Player_Controller : MonoBehaviour
{
    [SerializeField]
    private float walkingAcceleration, runningAcceleration, jumpingAccerleration, crawlingAccerleration;
    [SerializeField]
    private float walkingMaxSpeed, runningMaxSpeed, jumpingMaxSpeed, crawlingMaxSpeed;
    [SerializeField]
    private float jumpingForce;
    [SerializeField]
    private float xMouseSensitivity, yMouseSensitivity;

    private static float acceleration, maxSpeed;
    private static float movementSpeed;
    
    public static float speedModifier = 1;

    private const int kMaxSpeedDivisor = 20;
    private const float kMaxHeadRotorLimit = 290;
    private const float kMinHeadRotorLimit = 90;

    private int playerAttackNumber = -1;
    private bool readyToJump;
    private Vector3 rawVelocity;
    private Rigidbody playerRB;
    public Camera playerCamera;
    private Transform headRotor;

    private Vector3 cameraRot, playerRot, moveVertical, moveHorizontal;

    private void Start()
    {
        Event_Controller.idleEvent += idlePlayer;

        ConfigureSpeed();

    }
    private void Awake()
    {
        // setting player components
        playerRB = GetComponent<Rigidbody>();
        playerCamera = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Camera>();
        headRotor = transform.GetChild(0);
    }

    // set playerModeEnum to a mode
    private void ConfigurePlayerMode()
    {
        Event_Controller.attacking = Input.GetButton("Fire1") ? true : false;

        // see ColliderTrigger Region for "isJumping" boolean control.

        Event_Controller.crawling = !Event_Controller.jumping && Input.GetKey(KeyCode.C) ? true : false;

        Event_Controller.walking = !Event_Controller.jumping && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            && (Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.S)) ? true : false;

        Event_Controller.running = !Event_Controller.jumping && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            && (Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.S)) ? true : false;

        Event_Controller.idling = !Input.anyKey ? true : false;

        if (Input.GetKeyDown(KeyCode.Space) && !readyToJump) {   
            instantJumpPlayer();
        }
    }

    #region jumpingTriggers
    private void OnCollisionStay(Collision collision)
    {
        readyToJump = collision.gameObject.layer == 8 ? false : readyToJump;
        Event_Controller.jumping = false;
    }

    private void OnCollisionExit(Collision collision)
    {
    Event_Controller.jumping = true;
    readyToJump = collision.gameObject.layer == 8 ? true : readyToJump;
    }
    #endregion // Trigger to toggle boolean for jumping

    // movement of forward, left, right, and backwards at walking and running speed.
    private void ConfigureSpeed()
    {
        Event_Controller.jumpEvent += () =>
        {
            acceleration = jumpingAccerleration;
            maxSpeed = jumpingMaxSpeed / kMaxSpeedDivisor;
        };

        Event_Controller.runEvent += () =>
         {
             acceleration = runningAcceleration;
             maxSpeed = runningMaxSpeed / kMaxSpeedDivisor;
         };

        Event_Controller.walkEvent +=() =>
        {
            acceleration = walkingAcceleration;
            maxSpeed = walkingMaxSpeed / kMaxSpeedDivisor;
        };

        Event_Controller.crawlEvent += () =>
        {
            acceleration = crawlingAccerleration;
            maxSpeed = crawlingMaxSpeed / kMaxSpeedDivisor;
        };
        // increase scale for optimization
    }

    // set up to move player with corresponding speed
    private void movePlayer() {
        // getting axis of player movement
        float _XMove = Input.GetAxisRaw("Horizontal");
        float _YMove = Input.GetAxisRaw("Vertical");
        // getting the value of the vector by multiplying the value of axis and a normal vector
        moveHorizontal = _XMove * transform.right;
        moveVertical = _YMove * transform.forward;
        // setting acceleration rate
        movementSpeed += acceleration * Time.deltaTime;
        movementSpeed = movementSpeed > maxSpeed ? maxSpeed : movementSpeed < 0 ? 0 : movementSpeed;
    }

    // set up for rotation, allow camera and player to rotate according to mouse movements
    private void rotatePlayer()
    {
        // get value of axis when move mouse
        float _XRot = Input.GetAxis("Mouse Y");
        float _YRot = Input.GetAxis("Mouse X");
        // debugging player rotation limit
        float headRotorZ = headRotor.transform.localEulerAngles.z;
        cameraRot = headRotorZ < kMinHeadRotorLimit || headRotorZ > kMaxHeadRotorLimit 
                             || (headRotorZ >= kMinHeadRotorLimit && headRotorZ < 180 && _XRot < 0) 
                             || (headRotorZ <= kMaxHeadRotorLimit && headRotorZ > 180 &&  _XRot > 0)
                            ? new Vector3(0, 0, _XRot) * xMouseSensitivity : Vector3.zero;
        playerRot = new Vector3(0, _YRot, 0) * yMouseSensitivity;
    }

    // calling rotatePlayer and movePlayer after modifier
    private void playerFunction()
    {
        movePlayer();
        rotatePlayer();
        // modifier
        float modMovementSpeed = movementSpeed * speedModifier;
        // move player
        rawVelocity = (moveHorizontal + moveVertical).normalized * modMovementSpeed;
        playerRB.MovePosition(playerRB.position + rawVelocity);
        // rotate players
        headRotor.transform.Rotate(cameraRot);
        playerRB.MoveRotation(playerRB.rotation * Quaternion.Euler(playerRot));
    }

    // preform functions while no keys are pressed
    private void idlePlayer()
    {
        movementSpeed = 0;
    }

    // add force to player for jumping 
    private void instantJumpPlayer() {
        playerRB.AddForce(Vector3.up * jumpingForce, ForceMode.Impulse);
        movementSpeed /= 2;
    }

    // alter speed while crawling(set speed to 0 when stop moving during crawl mode)
    private void instantCrawlPlayer()
    {
        movementSpeed = 0;
    }

    /// <summary>
    /// Returns in order: [acceleration, max speed, movement speed]
    /// </summary>
    public static float[] getPlayerData()
    {
        return new float[] { acceleration, maxSpeed, movementSpeed };
    }

    private void FixedUpdate()
    {
        playerFunction();
    }

    private void Update()
    {
        ConfigurePlayerMode(); 
    }
}
