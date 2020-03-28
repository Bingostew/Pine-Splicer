using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;

namespace StatEffect
{
    public class AttributeInstance
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value1"> time the modifier lasts </param>
        /// <param name="value2"></param>
        /// <param name="value3"> optional value depend on the modifier </param>
        /// <param name="modifier"> type of modifier </param>
        /// <param name="gb"> gameObject the modifier affects </param>
        public static void beginModifier(float value1, float value2, char modifier, GameObject gb, float value3 = 0)
        {
            switch (modifier) {
                case 's':
                    MoveSpeedModifier s = new MoveSpeedModifier(value1, value2);
                    s.startModifier(gb);
                    break;
                case 'l':
                    LinearHealthModifier l = new LinearHealthModifier(value1, value2, value3);
                    l.startModifier(gb);
                    break;
            }
        }
    }
    
    // Each attribute class contains: 
    // - Method to handle change- either through modifiers or directly change static variables
    // - Event handling: add/ subtract event
    // - Timer

    public class MoveSpeedModifier : AttributeInstance
    {
        private float slownessLength = 0;
        private float slownessPercent = 0;
        private GameObject modifiedGb;
        public MoveSpeedModifier(float slownessLth, float slownessPct)
        {
            slownessPercent = slownessPct;
            slownessLength = slownessLth;
        }

        public void startModifier(GameObject gb)
        {
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
            Event_Controller.TimedEvent(resetModifier, null, slownessLength);
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
        }
    }
   
    public class LinearHealthModifier
    {
        private float depletion, rate, duration;
        private float runTime, runCounter;
        private GameObject modifiedGb;
        public LinearHealthModifier( float time, float damage, float timeIncrement)
        {
            rate = timeIncrement;
            depletion = damage;
            duration = time;
        }
        public void startModifier(GameObject gb)
        {
            modifiedGb = gb;
            Event_Controller.TimedEvent(null, depleteHealth, duration);
        }
        private void depleteHealth()
        {
            runTime += Time.deltaTime;
            if (runTime > runCounter)
            {
                runCounter += rate;
                Health_Base.changeEntityHeath(modifiedGb, depletion);
            }
        }

    }
}