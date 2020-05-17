using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDetonatorModifier : AttributeInstance
{
    [SerializeField]
    private bool timedDamage;

    // Start is called before the first frame update
    private void OnEnable()
    {
        AddDamageAttribute(gameObject, startModifier);
    }

    public void startModifier(GameObject _hitGb, float metaDamage)
    {
        if (GetComponent<Object_Motion>()) { GetComponent<Object_Motion>().endFlight(); }
        Health_Base.changeEntityHeath(Weapon_Control.GetHealthBaseObject(_hitGb), _hitGb.transform.position, metaDamage, timedDamage, Weapon_Control.GetDamageType(_hitGb));
    }
}
