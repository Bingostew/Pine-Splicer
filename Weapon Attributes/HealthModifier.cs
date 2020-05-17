using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthModifier : AttributeInstance
{
    [SerializeField]
    private float damage;
    [SerializeField]
    private float rate;
    [SerializeField]
    private float duration;
    LinearHealthModifier h = new LinearHealthModifier();

    private void OnEnable()
    {
        AddAttribute(gameObject, startModifier);
    }

    public void startModifier(GameObject hitGb)
    {
        if (!linearHealthStream.ContainsKey(Weapon_Control.GetHealthBaseObject(hitGb)))
        {
            h.linearHealthChange(Weapon_Control.GetHealthBaseObject(hitGb), hitGb.transform.position, damage, rate, duration);
        }
        AddHealthStream(Weapon_Control.GetHealthBaseObject(hitGb), this);
    }

    public void RestartEvent()
    {
        h.RestartEvent();
    }
}
