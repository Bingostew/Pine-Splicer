using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WeaponAttributes
{
    public class AttributeInstance
    {
        public static SlownessAttributes slownessInstance(float slownessLength, float slownessPct)
        {
            SlownessAttributes s = new SlownessAttributes(slownessLength, slownessPct);
            return s;
        }
    }

    // Each attribute class contains: 
    // - Method to handle change- either through modifiers or directly change static variables
    // - Event handling: add/ subtract event
    // - Timer

    public class SlownessAttributes : AttributeInstance
    {
        private float runningTime = 0;
        private float slownessLength, slownessPercent;

        public SlownessAttributes(float slownessLth, float slownessPct)
        {
            slownessLength = slownessLth;
            slownessPercent = slownessPct;
            Event_Controller.statChangeEvent += slowTime;
        }
        private void slowTime()
        {
            runningTime += Time.deltaTime;
            Player_Controller.speedModifier = slownessPercent;
            if(runningTime > slownessLength)
            {
                Player_Controller.speedModifier = 1;
                Event_Controller.statChangeEvent -= slowTime;
            }
        }
    }
}