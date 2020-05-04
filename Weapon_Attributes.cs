using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;

namespace StatEffect
{
    public class AttributeInstance
    {
        protected static Dictionary<GameObject, LinearHealthModifier> linearHealthStream = new Dictionary<GameObject, LinearHealthModifier>();
        protected static Dictionary<GameObject, MoveSpeedModifier> speedStream = new Dictionary<GameObject, MoveSpeedModifier>();


        public static void beginModifier(float value1, float value2, float damage, char modifier, GameObject gb, GameObject gbOrigin = null, float value3=0, float value4 = 0)
        { 
            switch (modifier) {
                case 's':
                    if (speedStream.ContainsKey(gb)) { speedStream[gb].RestartEvent(); }
                    else
                    {
                        MoveSpeedModifier s = new MoveSpeedModifier(value1, value2, damage);
                        speedStream.Add(gb, s);
                        s.startModifier(gb);
                    }
                    break;
                case 'l':
                    if (linearHealthStream.ContainsKey(gb)) { linearHealthStream[gb].RestartEvent(); }
                    else
                    {
                        LinearHealthModifier l = new LinearHealthModifier(value1, value2, value3, damage);
                        linearHealthStream.Add(gb, l);
                        l.startModifier(gb);
                    }
                    break;
                case 'b':
                    BounceShotModifier b = new BounceShotModifier();
                    b.startModifier(gbOrigin, gb, value1, value2, damage);
                    break;
                case 'g':
                    ShotgunModifier g = new ShotgunModifier();
                    g.startModifier(gb, gbOrigin, damage);
                    break;
                case 'd':
                    ContactDetonatorModifier d = new ContactDetonatorModifier();
                    d.startModifier(gbOrigin);
                    break;
                case 'p':
                    PlayerHealthModifier p = new PlayerHealthModifier();
                    p.startModifier(gb, value1);
                    break;
            }
        }
    }
    
    // Each attribute class contains: 
    // - Method to handle change- either through modifiers or directly change static variables
    // - Event handling: add/ subtract event

    public class MoveSpeedModifier : AttributeInstance
    {
        private float slownessLength = 0;
        private float slownessPercent = 0;
        private float metaDamage = 0;
        private GameObject modifiedGb;
        private TimeBased t;

        public MoveSpeedModifier(float slownessLth, float slownessPct, float _metaDamage)
        {
            slownessPercent = slownessPct;
            slownessLength = slownessLth;
            metaDamage = _metaDamage;
        }

        public void startModifier(GameObject gb)
        {
            Health_Base.changeEntityHeath(gb, metaDamage, false);
            modifiedGb = gb;
            switch (gb.tag)
            {
                case "Enemy":
                        modifiedGb.GetComponent<Enemy_Controller>().enemySpeedModifier *= slownessPercent;
                    break;
                case "Player":
                    Player_Controller.speedModifier *= slownessPercent;
                    break;
            }
            Event_Controller.TimedEvent(resetModifier, null, slownessLength, out t);
        }

        private void resetModifier()
        {
            if (modifiedGb != null)
            {
                switch (modifiedGb.tag)
                {
                    case "Enemy":
                        modifiedGb.GetComponent<Enemy_Controller>().enemySpeedModifier = 1;
                        break;
                    case "Player":
                        Player_Controller.speedModifier = 1;
                        break;
                }
            }
            speedStream.Remove(modifiedGb);
        }

        public void RestartEvent() { t.restartTime(); }
    }
   
    public class LinearHealthModifier : AttributeInstance
    {
        private float depletion, rate, duration;
        private float runTime, runCounter;
        private float metaDamage;
        private GameObject modifiedGb;
        private TimeBased t;

        public LinearHealthModifier( float time, float damage, float timeIncrement, float _metaDamage)
        {
            runCounter = timeIncrement;
            rate = timeIncrement;
            depletion = damage;
            duration = time;
            metaDamage = _metaDamage;
        }
        public void startModifier(GameObject gb)
        {
            Health_Base.changeEntityHeath(gb, metaDamage, false);
            modifiedGb = gb;
            Event_Controller.TimedEvent(Reset, DepleteHealth, duration, out t);
        }
        public void RestartEvent()
        {
            t.restartTime();
        }
        private void DepleteHealth()
        {
            runTime += Time.deltaTime;
            if (runTime > runCounter)
            {
                runCounter += rate;
                Health_Base.changeEntityHeath(modifiedGb, depletion, false);
            }
        }
        private void Reset()
        {
            linearHealthStream.Remove(modifiedGb);
        }
    }

    public class BounceShotModifier : AttributeInstance
    {
        public void startModifier(GameObject gbOrigin, GameObject gb,float power, float randomBounce, float metaDamage)
        {
            Health_Base.changeEntityHeath(gb, metaDamage, false);
            gbOrigin.GetComponent<Object_Motion>().RestartFlight( power, randomBounce);
        }
    }

    public class ShotgunModifier : AttributeInstance
    {
        public void startModifier(GameObject gb, GameObject gbOrigin, float metaDamage)
        {
            Health_Base.changeEntityHeath(gb, metaDamage, true);
        }
    }

    public class ContactDetonatorModifier : AttributeInstance
    {
        public void startModifier(GameObject gbOrigin)
        {
            gbOrigin.GetComponent<Object_Motion>().endFlight();
        }
    }

    public class PlayerHealthModifier : AttributeInstance
    {
        public void startModifier(GameObject gb, float healingAmount)
        {
            if (gb.layer == 9)
            {
                Health_Base.changeEntityHeath(Instant_Reference.playerReference, -healingAmount, false);
            }
        }
    }
}