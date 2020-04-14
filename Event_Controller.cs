using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Event_Controller : MonoBehaviour
{
    public delegate void PlayerModeDelegate();
    private static List<PlayerModeDelegate> attackModeStream = new List<PlayerModeDelegate>();
    private static List<PlayerModeDelegate> jumpModeStream = new List<PlayerModeDelegate>();
    private static List<PlayerModeDelegate> runModeStream = new List<PlayerModeDelegate>();
    private static List<PlayerModeDelegate> walkModeStream = new List<PlayerModeDelegate>();
    private static List<PlayerModeDelegate> crawlModeStream = new List<PlayerModeDelegate>();
    private static List<PlayerModeDelegate> aimModeStream = new List<PlayerModeDelegate>();
    private static List<PlayerModeDelegate> idleModeStream = new List<PlayerModeDelegate>();
    private static List<PlayerModeDelegate> stateChangeStream = new List<PlayerModeDelegate>();

    public static event PlayerModeDelegate instantEvent;
    public static event PlayerModeDelegate attackEvent;
    public static event PlayerModeDelegate jumpEvent;
    public static event PlayerModeDelegate crawlEvent;
    public static event PlayerModeDelegate walkEvent;
    public static event PlayerModeDelegate runEvent;
    public static event PlayerModeDelegate idleEvent;
    public static event PlayerModeDelegate aimEvent;
    public static event PlayerModeDelegate timeBasedEvent;
    private static bool attackStreamReady, jumpStreamReady, walkStreamReady, runStreamReady, crawlStreamReady, aimStreamReady, idleStreamReady;
    public static bool attacking, jumping, crawling, walking, running, idling, aiming, statChanging;

    /// <summary>
    /// Event that updates for a given time
    /// </summary>
    /// <param name="endTimeEvent"> event that is called once after time is up</param>
    /// <param name="time">time for the event to update, in seconds</param>
    public static void TimedEvent(PlayerModeDelegate endTimeEvent, PlayerModeDelegate constantTimeEvent, float time, out TimeBased timeBasedClass)
    {
        TimeBased t = new TimeBased(time, endTimeEvent, constantTimeEvent);
        timeBasedClass = t;
    }

    public static void addIdleStream(PlayerModeDelegate a) { idleModeStream.Add(a); }
    public static void addStateChangeStream(PlayerModeDelegate a) { stateChangeStream.Add(a); }
    public static void addAttackStream(PlayerModeDelegate a){attackModeStream.Add(a);}
    public static void addRunStream(PlayerModeDelegate a) { runModeStream.Add(a); }
    public static void addWalkStream(PlayerModeDelegate a) { walkModeStream.Add(a); }
    public static void addCrawlStream(PlayerModeDelegate a) { crawlModeStream.Add(a); }
    public static void addJumpStream(PlayerModeDelegate a) { jumpModeStream.Add(a); }
    public static void addAimStream(PlayerModeDelegate a) { aimModeStream.Add(a); }
    public static void removeAimStream(PlayerModeDelegate a) { aimModeStream.Remove(a); }
    public static void removeAttackStream(PlayerModeDelegate a) { attackModeStream.Remove(a); }
    public static void removeRunStream(PlayerModeDelegate a) { runModeStream.Remove(a); }
    public static void removeWalkStream(PlayerModeDelegate a) { walkModeStream.Remove(a); }
    public static void removeCrawlStream(PlayerModeDelegate a) { crawlModeStream.Remove(a); }
    public static void removeJumpStream(PlayerModeDelegate a) { jumpModeStream.Remove(a); }
    public static void removeStateChangeStream(PlayerModeDelegate a) { stateChangeStream.Remove(a); }
    public static void removeIdleStream(PlayerModeDelegate a) { idleModeStream.Remove(a); }

    private static void stateChangeEvent()
    {
        foreach(PlayerModeDelegate a in stateChangeStream)
        {
            a.Invoke();
        }
    }

    private static void instantAttackEvent()
    {
        foreach (PlayerModeDelegate a in attackModeStream)
        {
            a.Invoke();
        }
        attackStreamReady = false;
    }
    private static void InstantWalkEvent()
    {
        foreach (PlayerModeDelegate a in walkModeStream)
        {
            a.Invoke();
        }
        stateChangeEvent();
        walkStreamReady = false;
    }
    private static void InstantRunEvent()
    {
        foreach (PlayerModeDelegate a in runModeStream)
        {
            a.Invoke();
        }
        stateChangeEvent();
        runStreamReady = false;
    }
    private static void InstantJumpEvent()
    {
        foreach (PlayerModeDelegate a in jumpModeStream)
        {
            a.Invoke();
        }
        stateChangeEvent();
        jumpStreamReady = false;
    }
    private static void InstantCrawlEvent()
    {
        foreach (PlayerModeDelegate a in crawlModeStream)
        {
            a.Invoke();
        }
        stateChangeEvent();
        crawlStreamReady = false;
    }
    private static void InstantAimEvent()
    {
        foreach(PlayerModeDelegate a in aimModeStream)
        {
            a.Invoke();
        }
        stateChangeEvent();
        aimStreamReady = false;
    }
    private static void InstantIdleEvent()
    {
        foreach(PlayerModeDelegate a in idleModeStream)
        {
            a.Invoke();
        }
        stateChangeEvent();
        idleStreamReady = false;
    }

    void FixedUpdate()
    {
        if (attacking) {
            if (attackStreamReady) { attackStreamReady = false;  instantAttackEvent(); }
            attackEvent?.Invoke();
        }
        else if (!attacking){ attackStreamReady = true; }

        if (jumping) {
            if (jumpStreamReady) { jumpStreamReady = false; InstantJumpEvent(); }
            jumpEvent?.Invoke(); 
        }
        else { jumpStreamReady = true; }

        if (crawling) {
            if (crawlStreamReady) { crawlStreamReady = false; InstantCrawlEvent(); }
            crawlEvent?.Invoke();
        }
        else { crawlStreamReady = true; }

        if (walking) {
            if (walkStreamReady) { walkStreamReady = false; InstantWalkEvent(); }
            walkEvent?.Invoke(); }
        else { walkStreamReady = true; }

        if (running) {
            if (runStreamReady) { runStreamReady = false; InstantRunEvent(); }
            runEvent?.Invoke(); }
        else { runStreamReady = true; }

        if (aiming)
        {
            if (aimStreamReady) { aimStreamReady = false; InstantAimEvent(); }
            aimEvent?.Invoke();
        }
        else { aimStreamReady = true; }

        if (idling)
        {
            if (idleStreamReady) { idleStreamReady = false; InstantIdleEvent(); }
            idleEvent?.Invoke(); }
        else { idleStreamReady = true; }
        timeBasedEvent?.Invoke();
       // print(timeBasedEvent?.GetInvocationList().Length);
    }
}


// class for a calculating time in TimedEvent
public class TimeBased
{
    public float runTime, endTime;
    Event_Controller.PlayerModeDelegate endEvent;
    Event_Controller.PlayerModeDelegate continuousEvent;

    public TimeBased(float _endTime, Event_Controller.PlayerModeDelegate endTimeEvent, Event_Controller.PlayerModeDelegate runningEvent) {
        endTime = _endTime;
        endEvent = endTimeEvent;
        continuousEvent = runningEvent;
        Event_Controller.timeBasedEvent += startTime;
        Event_Controller.timeBasedEvent += continuousEvent;
    }

    public void startTime()
    {
        runTime += Time.deltaTime;
        if(runTime > endTime)
        {
            finishTime();
            endEvent?.Invoke();
        }
    }

    public void killTime(bool callEndEvent)
    {
        finishTime();
        if(callEndEvent) { endEvent?.Invoke(); }
    }

    public void restartTime()
    {
        runTime = 0;
    }

    private void finishTime()
    {
        Event_Controller.timeBasedEvent -= startTime;
        Event_Controller.timeBasedEvent -= continuousEvent;
    }
}