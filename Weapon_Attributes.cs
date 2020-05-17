using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeInstance : MonoBehaviour
{
    protected delegate void SpecialAttribute(GameObject hitGb);
    protected delegate void DamageAttribute(GameObject hitGb, float damage);
    protected static Dictionary<GameObject, List<SpecialAttribute>> specialAttributeStream = new Dictionary<GameObject, List<SpecialAttribute>>();
    protected static Dictionary<GameObject, DamageAttribute> damageAttributeStream = new Dictionary<GameObject, DamageAttribute>();
    public static Dictionary<GameObject, HealthModifier> linearHealthStream = new Dictionary<GameObject, HealthModifier>();
    public static Dictionary<GameObject, TimeBased> speedStream = new Dictionary<GameObject, TimeBased>();
    protected static bool hasObjectMotion = false;


    protected static void AddAttribute(GameObject gb, SpecialAttribute c)
    {
        if (specialAttributeStream.ContainsKey(gb))
        {
            specialAttributeStream[gb].Add(c);
        }
        else
        {
            specialAttributeStream.Add(gb, new List<SpecialAttribute>());
            specialAttributeStream[gb].Add(c);
        }
    }

    protected static void AddSpeedStream(GameObject gb, TimeBased t)
    {
        if (speedStream.ContainsKey(gb))
        {
            speedStream[gb].restartTime();
        }
        else
        {
            speedStream.Add(gb, t);
        }
    }

    protected static void AddDamageAttribute(GameObject gb, DamageAttribute d)
    {
        if (!damageAttributeStream.ContainsKey(gb))
            damageAttributeStream.Add(gb, d);
    }

    public static void AddHealthStream(GameObject gb, HealthModifier h)
    {
        if (linearHealthStream.ContainsKey(gb)) { linearHealthStream[gb].RestartEvent(); }
        else
        {
            linearHealthStream.Add(gb, h);
        }
    }

    public static void callAttribute(GameObject hitGb, GameObject contactGb, float damage)
    {
        if (specialAttributeStream.ContainsKey(contactGb))
        {
            foreach (SpecialAttribute c in specialAttributeStream[contactGb])
            {
                c?.Invoke(hitGb);
            }
        }
        damageAttributeStream[contactGb]?.Invoke(hitGb, damage);
    }
}


public class LinearHealthModifier
{
    float damage, rate, duration, runTime, runCounter;
    GameObject hitGb;
    Vector3 hitPos;
    TimeBased t;

    public void linearHealthChange(GameObject _hitGb, Vector3 _hitPos, float _damage, float _rate, float _duration)
    {
        damage = _damage;
        rate = _rate;
        duration = _duration;
        hitGb = _hitGb;
        hitPos = _hitPos;
        runCounter = rate;
        Event_Controller.TimedEvent(() => AttributeInstance.linearHealthStream.Remove(_hitGb), DepleteHealth, duration, out t);
    }

    private void DepleteHealth()
    {
        if(hitGb == null)
        {
            t.killTime(true);
        }
        runTime += Time.deltaTime;
        if (runTime > runCounter)
        {
            runCounter += rate;
            Health_Base.changeEntityHeath(hitGb, hitPos, damage, false, Health_Base.DamageType.EnemyNormal);
        }
    }

    public void RestartEvent()
    {
        t.restartTime();
    }
}


public class PlayerHealthModifier : AttributeInstance
{
    public void startModifier(GameObject gb, GameObject gbOrigin, float healingAmount)
    {
        if (gb.layer == 9)
        {
            //Health_Base.changeEntityHeath(Instant_Reference.playerReference, gbOrigin.transform.position, -healingAmount, false);
        }
    }
}
