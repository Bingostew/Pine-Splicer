using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Event_Controller : MonoBehaviour
{
    public delegate void PlayerModeDelegate();
    public static event PlayerModeDelegate attackEvent;
    public static event PlayerModeDelegate jumpEvent;
    public static event PlayerModeDelegate crawlEvent;
    public static event PlayerModeDelegate walkEvent;
    public static event PlayerModeDelegate runEvent;
    public static event PlayerModeDelegate idleEvent;
    public static event PlayerModeDelegate statChangeEvent;
    public static bool attacking, jumping, crawling, walking, running, idling, statChanging;

    /*
    public static void instantAttack()
    {
        if (attacking) attackEvent?.Invoke();
    }

    public static void instantJump()
    {
        if (jumping) jumpEvent?.Invoke();
    }

    public static void instantCrawl()
    {
        if (crawling) crawlEvent?.Invoke();
    }

    public static void instantWalk(Action walkAction)
    {
        walkEvent += () => walkAction();
        walkEvent?.Invoke(); 
        walkEvent -= () => walkAction();
    }

    public static void instantRun()
    {
        if (running) runEvent?.Invoke();
    }

    public static void instantIdle()
    {
        if (idling) idleEvent?.Invoke();
    }
    */
    void Update()
    {
        if (attacking) { attackEvent?.Invoke(); }
        if (jumping) { jumpEvent?.Invoke(); }
        if (crawling) { crawlEvent?.Invoke(); }
        if (walking) { walkEvent?.Invoke(); }
        if (running) { runEvent?.Invoke(); }
        if (idling) { idleEvent?.Invoke(); }
         statChangeEvent?.Invoke();
    }
}
