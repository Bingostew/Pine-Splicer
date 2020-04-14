using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instant_Reference : MonoBehaviour
{
    public GameObject r_playerReference;
    public GameObject r_UIController;
    public static GameObject playerReference;
    public static GameObject playerRightHand;
    public static GameObject UIController;
    public static Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        playerReference = r_playerReference;
        UIController = r_UIController;
        mainCamera = playerReference.GetComponent<Player_Controller>().playerCamera;
        playerRightHand = mainCamera.transform.GetChild(1).gameObject;
    }
    // Update is called once per frame

    public static float FixArcSine(float arcsine)
    {
        if(arcsine <= 90 && arcsine >= -90){ return arcsine; }
        else { return arcsine - 180; }
    }

    public static string[] GetPlayerLayermask()
    {
        return new string[] { "Terrain", "Entity" };
    }

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
        return new Ray(getRightHandPosition(), getPlayerHitPoint(range) - getRightHandPosition());
    }

    /// <summary>
    /// returns a ray on the xz plane with constant y-value upon hitting a raycast target
    /// </summary>
    /// <param name="origin">origin of the ray</param>
    /// <param name="destination">where the ray hits</param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static Ray getHitParallelRay(Vector3 origin, Vector3 destination, float range)
    {
        return new Ray(origin, new Vector3(
            getHitPoint(origin, destination - origin, range).x - origin.x,
            0,
            getHitPoint(origin, destination - origin, range).z - origin.z));
    }


    public static Vector3 getRightHandToHitVector(float range)
    {
        return new Ray(getRightHandPosition(), 
            getPlayerHitPoint(range) - getRightHandPosition()).GetPoint(range);
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

    public static Vector3 getPlayerHitPoint(float range)
    {
        LayerMask mask = LayerMask.GetMask("Terrain", "Entity");
        RaycastHit hitPoint; // never instantiate a rayCastHit Object outside of the method scope

        if (Physics.Raycast(getPlayerCamStraightRay(), out hitPoint, range, mask))
        {
            return hitPoint.point;
        }
        else
        {
            return getPlayerCamStraightRay().GetPoint(range);
        }
    }

    //Retrieve the point hit by the straightray.
    public static Vector3 getHitPoint(Vector3 origin, Vector3 direction, float range){
        LayerMask mask = LayerMask.GetMask("Terrain", "Entity");
        RaycastHit hitPoint; // never instantiate a rayCastHit Object outside of the method scope

        if (Physics.Raycast(new Ray(origin, direction), out hitPoint, range,mask))
        {
            return hitPoint.point;
        }
        else
        {
            return new Ray(origin, direction).GetPoint(range);
        }
    }

}
