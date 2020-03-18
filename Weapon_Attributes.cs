using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;

namespace StatEffect
{
    public class AttributeInstance
    {
        private static int speedModifierStreamIndex = -1;
        private static Dictionary<int, MoveSpeedModifier> speedModifierStream = new Dictionary<int, MoveSpeedModifier>();

        public static int moveSpeedInstance( float slownessLength, float slownessPct)
        {
            MoveSpeedModifier s = new MoveSpeedModifier(slownessLength, slownessPct);
            speedModifierStreamIndex++;
            speedModifierStream.Add(speedModifierStreamIndex, s);
            return speedModifierStreamIndex;
        }

        public static void beginModifier(ref float targetModifier, int modifierIndex)
        {
            speedModifierStream[0].startModifier(ref targetModifier);
        }
    }
    
    // Each attribute class contains: 
    // - Method to handle change- either through modifiers or directly change static variables
    // - Event handling: add/ subtract event
    // - Timer

    public class MoveSpeedModifier : AttributeInstance
    {
        private float runningTime = 0;
        private float slownessLength = 0;
        private float slownessPercent = 0;

        public MoveSpeedModifier(float slownessLth, float slownessPct)
        {
            slownessPercent = slownessPct;
            slownessLength = slownessLth;
        }

        public void startModifier(ref float targetModifier)
        {
            targetModifier = slownessPercent;
            Event_Controller.statChangeEvent += slowTime;
        }

        private void slowTime()
        {
            runningTime += Time.deltaTime;
            if(runningTime > slownessLength)
            {
                Player_Controller.speedModifier = 1;
                Event_Controller.statChangeEvent -= slowTime;
            }
        }
    }
}