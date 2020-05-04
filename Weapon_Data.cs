using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Data : MonoBehaviour
{
    [SerializeField]
    public Shootable_Object shootableObject;

    public Vector3 getShootOrigin() {
        Vector3 shootOrigin = transform.childCount != 0 ? transform.GetChild(0).transform.position : Instant_Reference.getRightHandPosition();
        return shootOrigin;
    }
}
