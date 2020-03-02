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

    void Update()
    {
        print(attacking);
        if (attacking) { attackEvent?.Invoke(); }
        if (jumping) { jumpEvent?.Invoke(); print("jumping"); }
        if (crawling) { crawlEvent?.Invoke(); }
        if (walking) { walkEvent?.Invoke(); print("walking"); }
        if (running) { runEvent?.Invoke(); print("running"); }
        if (idling) { idleEvent?.Invoke(); }
        statChangeEvent?.Invoke();
    }
}
