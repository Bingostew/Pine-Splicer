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

    public static List<PlayerModeDelegate[]> timeBasedStream = new List<PlayerModeDelegate[]>();
    public static int timeBasedIndex;

    public static event PlayerModeDelegate instantEvent;
    public static event PlayerModeDelegate attackEvent;
    public static event PlayerModeDelegate jumpEvent;
    public static event PlayerModeDelegate crawlEvent;
    public static event PlayerModeDelegate walkEvent;
    public static event PlayerModeDelegate runEvent;
    public static event PlayerModeDelegate idleEvent;
    public static event PlayerModeDelegate timeBasedEvent;
    private static bool attackStreamReady, jumpStreamReady, walkStreamReady, runStreamReady, crawlStreamReady;
    public static bool attacking, jumping, crawling, walking, running, idling, statChanging;

    /// <summary>
    /// instantly calls an event without update
    /// </summary>
    /// <param name="a"></param>
    public static void QuickEvent(PlayerModeDelegate a)
    {
        instantEvent += a;
        instantEvent?.Invoke();
        instantEvent -= a;
    }

    /// <summary>
    /// Event that updates for a given time
    /// </summary>
    /// <param name="endTimeEvent"> event that is called once after time is up</param>
    /// <param name="time">time for the event to update, in seconds</param>
    public static void TimedEvent(PlayerModeDelegate endTimeEvent, PlayerModeDelegate constantTimeEvent, float time)
    {
        new TimeBased(time, endTimeEvent, constantTimeEvent);
    }

    public static void addAttackStream(PlayerModeDelegate a){attackModeStream.Add(a);}
    public static void addRunStream(PlayerModeDelegate a) { runModeStream.Add(a); }
    public static void addWalkStream(PlayerModeDelegate a) { walkModeStream.Add(a); }
    public static void addCrawlStream(PlayerModeDelegate a) { crawlModeStream.Add(a); }
    public static void addJumpStream(PlayerModeDelegate a) { jumpModeStream.Add(a); }

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
    private static void InstantWalkEvent()
    {
        foreach (PlayerModeDelegate a in walkModeStream)
        {
            walkEvent += a;
            walkEvent?.Invoke();
            walkEvent -= a;
        }
        walkStreamReady = false;
    }
    private static void InstantRunEvent()
    {
        foreach (PlayerModeDelegate a in runModeStream)
        {
            runEvent += a;
            runEvent?.Invoke();
            runEvent -= a;
        }
        runStreamReady = false;
    }
    private static void InstantJumpEvent()
    {
        foreach (PlayerModeDelegate a in jumpModeStream)
        {
            jumpEvent += a;
            jumpEvent?.Invoke();
            jumpEvent -= a;
        }
        jumpStreamReady = false;
    }
    private static void instantCrawlEvent()
    {
        foreach (PlayerModeDelegate a in crawlModeStream)
        {
            crawlEvent += a;
            crawlEvent?.Invoke();
            crawlEvent -= a;
        }
        crawlStreamReady = false;
    }

    void FixedUpdate()
    {
        if (attacking) {
            if (attackStreamReady) { instantAttackEvent(); }
            attackEvent?.Invoke();
        }
        else { attackStreamReady = true; }

        if (jumping) {
            if (jumpStreamReady) { InstantJumpEvent(); }
            jumpEvent?.Invoke(); 
        }
        else { jumpStreamReady = true; }

        if (crawling) {
            if (crawlStreamReady) { instantCrawlEvent(); }
            crawlEvent?.Invoke();
        }
        else { crawlStreamReady = true; }

        if (walking) {
            if (walkStreamReady) { InstantWalkEvent(); }
            walkEvent?.Invoke(); }
        else { walkStreamReady = true; }

        if (running) {
            if (runStreamReady) { InstantRunEvent(); }
            runEvent?.Invoke(); }
        else { runStreamReady = true; }

        if (idling) { idleEvent?.Invoke(); }
        timeBasedEvent?.Invoke();
    }
}


// class for a calculating time in TimedEvent
public class TimeBased
{
    float runTime, endTime;
    int currentTimeIndex;

    public TimeBased(float _endTime, Event_Controller.PlayerModeDelegate endTimeEvent, Event_Controller.PlayerModeDelegate runningEvent) {
        Event_Controller.timeBasedStream.Add(new Event_Controller.PlayerModeDelegate[] { endTimeEvent, runningEvent });
        currentTimeIndex = Event_Controller.timeBasedStream.ToArray().Length - 1;
        Event_Controller.timeBasedEvent += startTime;
        Event_Controller.timeBasedEvent += Event_Controller.timeBasedStream[currentTimeIndex][1];
        endTime = _endTime;
    }

    public void startTime()
    {
        runTime += Time.deltaTime;
        if(runTime > endTime)
        {
            runTime = 0;
            finishTime();
            Event_Controller.timeBasedEvent -= startTime;
        }
    }

    private void finishTime()
    {
        Event_Controller.QuickEvent(Event_Controller.timeBasedStream[currentTimeIndex][0]);
        Event_Controller.timeBasedEvent -= Event_Controller.timeBasedStream[currentTimeIndex][1];
    }
}