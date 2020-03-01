using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instant_Reference : MonoBehaviour
{
    public GameObject r_playerReference;
    public static GameObject playerReference;

    public static Camera mainCamera;
    public static Vector3 playerCamStraightVec;
    public static Vector3 playerPosition;
    public static Ray playerCamStraightRay;
    public static Vector3 rightHandPosition;
    public static Vector3 hitPoint;
    // Start is called before the first frame update
    void Start()
    {
        playerReference = r_playerReference;
        mainCamera = playerReference.GetComponent<Player_Controller>().playerCamera;
    }
    // Update is called once per frame
    void Update()
    {
        playerCamStraightVec = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 1));
        playerCamStraightRay = new Ray(mainCamera.transform.position, playerCamStraightVec- mainCamera.transform.position);
        playerPosition = playerReference.transform.position;
        rightHandPosition = mainCamera.transform.GetChild(1).GetChild(0).position;
        Debug.DrawRay(rightHandPosition, playerCamStraightVec - rightHandPosition);
    }

    
    public static Vector3 getHitPoint(float range){
        LayerMask mask = LayerMask.GetMask("8");
        RaycastHit hitPoint; // never instantiate a rayCastHit Object outside of the method scope

        if (Physics.Raycast(playerCamStraightRay, out hitPoint, range, mask))
        {
            return hitPoint.transform.position;
        }
        else
        {
            return playerCamStraightRay.GetPoint(range);
        }
    }
}
