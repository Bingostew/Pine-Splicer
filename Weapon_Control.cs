using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatEffect;

public class Weapon_Control : MonoBehaviour
{
    // all variables are static to prevent child making an instance
    protected static GameObject heldWeapon;
    protected static Shootable_Object shootableWeapon;
    protected static GameObject[] slotWeapons = new GameObject[5];
    protected static Weapon_Data heldWeaponData;

    protected static int weaponSlot ;
    private static int kBurstDivisor = 50;

    void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            slotWeapons[i] = transform.GetChild(i).gameObject;
        }
        toggleWeaponScripts(1);
    }
    
    public void toggleWeaponScripts(int slot)
    {
        if (weaponSlot != slot)
        {
            weaponSlot = slot;
            if (heldWeapon != null) { heldWeapon.SetActive(false); ToggleEnable(false); }
            GetComponent<Shootable>().enabled = false;
            heldWeapon = slotWeapons[slot - 1];
            heldWeaponData = null;

            if (heldWeapon != null)
            {
                heldWeapon.SetActive(true);
                heldWeaponData = heldWeapon.GetComponent<Weapon_Data>();
                ToggleEnable(true);
            }
        }
    }

    private void ToggleEnable(bool enable)
    {
        switch (heldWeapon.tag)
        {
            case "Shootable":
                shootableWeapon = heldWeapon.GetComponent<Weapon_Data>().shootableObject;
                GetComponent<Shootable>().enabled = enable;
                break;
        }
    }


    public static float PlayerAngle()
    {
        float angle = Vector3.Angle(Instant_Reference.getPlayerStraightRay().direction, Instant_Reference.getPlayerCamStraightRay().direction);
        if (Instant_Reference.getPlayerCamStraightRay().direction.y < 0) { angle *= -1; }
        return angle;
    }

    public void AddAttributes(GameObject gb, char[] attributeList, Vector4[] attributeValues, float weaponDamage, GameObject gbOrigin = null)
    { 
        for (int i = 0; i < attributeList.Length; i++)
        {
            AttributeInstance.beginModifier(attributeValues[i].x, attributeValues[i].y, weaponDamage, attributeList[i], gb, gbOrigin, attributeValues[i].z, attributeValues[i].w);
        }
    }

    public static Vector3 CalculateBurst(Vector3 pathOrigin, Vector3 pathDirection, float rayLength, float burst)
    {
        float burstMultiple = rayLength / kBurstDivisor;
        Ray hitRay = new Ray(pathOrigin, pathDirection);
        Ray xperpendicularRay = new Ray(hitRay.GetPoint(rayLength),
           new Vector3(-hitRay.direction.z, 0, hitRay.direction.x));

        float xBurstVec = burstMultiple * ( Random.value * burst - Random.value * burst);
        float yBurstVec = burstMultiple * ( Random.value * burst - Random.value * burst);
        Ray preFinalRay = new Ray(hitRay.origin, xperpendicularRay.GetPoint(xBurstVec) - hitRay.origin);
        Ray yperpendicularRay = new Ray(xperpendicularRay.GetPoint(xBurstVec),
            Vector3.Cross(preFinalRay.direction, xperpendicularRay.direction));

        return yperpendicularRay.GetPoint(yBurstVec);
    }

}
