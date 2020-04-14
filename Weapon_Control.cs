using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatEffect;

public class Weapon_Control : MonoBehaviour
{
    // all variables are static to prevent child making an instance
    protected static GameObject heldWeapon;
    protected static Shootable_Object shootableWeapon;
    protected static Weapon_Data heldWeaponData;
    // Start is called before the first frame update
    private static int kBurstDivisor = 75;
    void OnEnable()
    {
        toggleWeaponScripts();
    }

    public void toggleWeaponScripts()
    {
        heldWeapon = transform.GetChild(0).GetChild(0).gameObject;
        heldWeaponData = heldWeapon.GetComponent<Weapon_Data>();

        switch (heldWeapon.tag)
        {
            case "Shootable":
                shootableWeapon = heldWeapon.GetComponent<Weapon_Data>().shootableObject;
                GetComponent<Shootable>().enabled = true;
                break;
        }
    }

    public static float PlayerAngle()
    {
        float angle = Vector3.Angle(Instant_Reference.getPlayerStraightRay().direction, Instant_Reference.getPlayerCamStraightRay().direction);
        if (Instant_Reference.getPlayerCamStraightRay().direction.y < 0) { angle *= -1; }
        return angle;
    }

    public void AddAttributes(GameObject gb, char[] attributeList, Vector3[] attributeValues)
    {
        for (int i = 0; i < attributeList.Length; i++)
        {
            AttributeInstance.beginModifier(attributeValues[i].x, attributeValues[i].y, attributeList[i], gb, attributeValues[i].z);
        }
    }

    public static Vector3 CalculateBurst(Vector3 pathOrigin, Vector3 pathDirection, float rayLength, float burst)
    {
        float burstMultiple = rayLength / kBurstDivisor;
        Ray hitRay = new Ray(pathOrigin, pathDirection);
        Ray xperpendicularRay = new Ray(hitRay.GetPoint(rayLength),
           new Vector3(-hitRay.direction.z, 0, hitRay.direction.x));

        float xBurstVec = Random.value * burst * burstMultiple - Random.value * burst * burstMultiple;
        float yBurstVec = Random.value * burst * burstMultiple - Random.value * burst * burstMultiple;
        Ray preFinalRay = new Ray(hitRay.origin, xperpendicularRay.GetPoint(xBurstVec) - hitRay.origin);
        Ray yperpendicularRay = new Ray(xperpendicularRay.GetPoint(xBurstVec),
            Vector3.Cross(preFinalRay.direction, xperpendicularRay.direction));

        return yperpendicularRay.GetPoint(yBurstVec);
    }
}
