using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceShotModifier : AttributeInstance
{

    [SerializeField]
    private float powerDepletion;
    [SerializeField]
    private float randomBounce;
    [SerializeField]
    private int bounceAmount;

    private int bounceCount;

    private void OnEnable()
    {
        AddDamageAttribute(gameObject, startModifier);
    }

    public void startModifier(GameObject hitGb, float metaDamage)
    {
        bounceCount++;
        Health_Base.changeEntityHeath(Weapon_Control.GetHealthBaseObject(hitGb), hitGb.transform.position, metaDamage, false, Weapon_Control.GetDamageType(hitGb));
        if (bounceCount <= bounceAmount)
        {
            GetComponent<Object_Motion>().RestartFlight(powerDepletion, randomBounce);
        }
        else
        {
            GetComponent<Object_Motion>().endFlight();
        }
    } 
}
