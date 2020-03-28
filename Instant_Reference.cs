using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instant_Reference : MonoBehaviour
{
    public GameObject r_playerReference;
    public static GameObject playerReference;
    public static GameObject playerRightHand;

    public static Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        playerReference = r_playerReference;
        mainCamera = playerReference.GetComponent<Player_Controller>().playerCamera;
        playerRightHand = mainCamera.transform.GetChild(1).gameObject;
    }
    // Update is called once per frame

    public static Ray getPlayerCamStraightRay()
    {
        return new Ray(mainCamera.transform.position, getPlayerCamStraightVector() - mainCamera.transform.position);
    }

    public static Ray getPlayerStraightRay()
    {
        return new Ray(mainCamera.transform.position, 
            new Vector3(getPlayerCamStraightVector().x - mainCamera.transform.position.x,
                        0,
                        getPlayerCamStraightVector().z - mainCamera.transform.position.z));
    }

    public static Ray getRightHandToHitRay(float range)
    {
        return new Ray(getRightHandPosition(), getHitPoint(range) - getRightHandPosition());
    }

    public static Ray getRightHandToHitRayParallel(float range, Vector3 position)
    {
        return new Ray(position, new Vector3(
                                                            getHitPoint(range).x - getRightHandPosition().x,
                                                            0,
                                                            getHitPoint(range).z - getRightHandPosition().z));
    }

    public static Vector3 getRightHandToHitVector(float range)
    {
        return new Ray(getRightHandPosition(), getHitPoint(range) - getRightHandPosition()).GetPoint(range);
    }

    public static Vector3 getPlayerCamStraightVector()
    {
        return mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 1));
    }

    public static Vector3 getPlayerPosition()
    {
        return playerReference.transform.position;
    }

    public static Vector3 getRightHandPosition()
    {
        return playerRightHand.transform.position;
    }

    //Retrieve the point hit by the straightray.
    public static Vector3 getHitPoint(float range){
        LayerMask mask = LayerMask.GetMask("Terrain");
        RaycastHit hitPoint; // never instantiate a rayCastHit Object outside of the method scope

        if (Physics.Raycast(getPlayerCamStraightRay(), out hitPoint, range,mask))
        {
            return hitPoint.point;
        }
        else
        {
            return getPlayerCamStraightRay().GetPoint(range);
        }
    }
}
