using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Event_Controller : MonoBehaviour
{
    public delegate void PlayerModeDelegate();
    private static List<PlayerModeDelegate> attackModeStream = new List<PlayerModeDelegate>();
    public static event PlayerModeDelegate instantEvent;
    public static event PlayerModeDelegate attackEvent;
    public static event PlayerModeDelegate jumpEvent;
    public static event PlayerModeDelegate crawlEvent;
    public static event PlayerModeDelegate walkEvent;
    public static event PlayerModeDelegate runEvent;
    public static event PlayerModeDelegate idleEvent;
    public static event PlayerModeDelegate statChangeEvent;
    private static bool attackStreamReady;
    public static bool attacking, jumping, crawling, walking, running, idling, statChanging;

    //Anonymous instant event
    public static void quickEvent(PlayerModeDelegate a)
    {
        instantEvent += a;
        instantEvent.Invoke();
        instantEvent -= a;
    }

    public static void addAttackStream(PlayerModeDelegate a)
    {
        attackModeStream.Add(a);
    }

    private static void instantAttackEvent()
    {
        foreach (PlayerModeDelegate a in attackModeStream)
        {
            attackEvent += a;
            attackEvent?.Invoke();
            attackEvent -= a;
        }
        attackStreamReady = false;
    }

    void FixedUpdate()
    {
        if (attacking){
            if (attackStreamReady) { instantAttackEvent(); }
            attackEvent?.Invoke();
        }
        else{attackStreamReady = true;}

        if (jumping) { jumpEvent?.Invoke(); }
        if (crawling) { crawlEvent?.Invoke(); }
        if (walking) { walkEvent?.Invoke(); }
        if (running) { runEvent?.Invoke(); }
        if (idling) { idleEvent?.Invoke(); }
        statChangeEvent?.Invoke();
    }
}
