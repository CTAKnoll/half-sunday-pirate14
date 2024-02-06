using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Services;
using UnityEngine;
using Utils;

public class Timeline : IService
{
    public const int START_YEAR = 1600;
    public const int START_MONTH = 1;
    public const int START_DAY = 1;

    public enum GameSpeed
    {
        Paused,
        VerySlow,
        Slow,
        Normal, 
        Fast
    }

    private const float DAY_IN_REALTIME = 0.1f;
    public float DayIntervalInRealTime => IsPaused ? -1 : DAY_IN_REALTIME / SpeedMultTable[Speed];
    public bool IsPaused = false;
    public GameSpeed Speed => IsPaused ? GameSpeed.Paused : SetSpeed;
    
    private GameSpeed SetSpeed = GameSpeed.Normal;
    private readonly Dictionary<GameSpeed, float> SpeedMultTable = new()
    {
        [GameSpeed.Paused] = 0,
        [GameSpeed.VerySlow] = 0.25f,
        [GameSpeed.Slow] = 0.5f,
        [GameSpeed.Normal] = 1f,
        [GameSpeed.Fast] = 2f,
    };
    

    private WaitForSeconds DayPasses;

    public static readonly DateTime START_DATE = new (START_YEAR, START_MONTH, START_DAY);
    public static DateTime CRASH_DATE {  get; private set; }
    
    public static DateTime Now;
    private PriorityQueue<(object, Action), DateTime> TimelineEvents;
    public event Action<DateTime> DateChanged;
    public event Action MarketCrashed;
    public event Action GamePaused;
    public event Action<GameSpeed> GameUnpaused;
    public event Action<GameSpeed> SpeedChanged;
    
    private MainThreadScheduler MainThread;
    private Coroutine ZaWarudo;
    
    public Timeline()   
    {
        Now = START_DATE;
        TimelineEvents = new();
        ServiceLocator.TryGetService(out MainThread);

        CRASH_DATE = FromNow(37, 2);
        DayPasses = new WaitForSeconds(DayIntervalInRealTime);

        AddTimelineEvent(this, CrashTheMarket, CRASH_DATE);
    }

    public void SetGameSpeed(GameSpeed speed)
    {
        if (speed == GameSpeed.Paused)
        {
            IsPaused = true;
            GamePaused?.Invoke();
            return;
        }

        if (IsPaused)
            GameUnpaused?.Invoke(speed);
        else
            SpeedChanged?.Invoke(speed);
        
        IsPaused = false;
        SetSpeed = speed;
        DayPasses = new WaitForSeconds(DayIntervalInRealTime);
    }

    public void TogglePause()
    {
        if(IsPaused)
            SetGameSpeed(SetSpeed);
        else
            SetGameSpeed(GameSpeed.Paused);
    }

    public DateTime GetDateBeforeInRealtime(float realtimeSeconds, DateTime date)
    {
        var timespan = GetTimespanFromSeconds(realtimeSeconds);
        return date.Subtract(timespan);
    }

    public TimeSpan GetTimespanFromSeconds(float realtimeSeconds)
    {
        var daysPerSecond = 1 / DayIntervalInRealTime;
        return TimeSpan.FromDays(daysPerSecond * realtimeSeconds);
    }

    public void StartTheWorld()
    {
        ZaWarudo = MainThread.StartCoroutine(TheWorld());
    }

    public void StopTheWorld()
    {
        MainThread.StopCoroutine(ZaWarudo);
    }

    public IEnumerator TheWorld()
    {
        while (true)
        {
            if (!IsPaused)
            {
                yield return DayPasses;
                MoveToNextDay();
            }
        }
    }

    public void AddTimelineEvent(object owner, Action callback, DateTime eventTime) => TimelineEvents.Enqueue((owner, callback), eventTime);

    public void AddRecurring(object owner, Action callback, TimeSpan span)
    {
        TimelineEvents.Enqueue((owner, () =>
        {
            callback();
            AddRecurring(owner, callback, span);
        }), FromNow(span));
    }

    public void RemoveAllEvents(object owner)
    {
        List<(object, Action)> toDequeue = new();
        foreach(var timeEvent in TimelineEvents.UnorderedItems)
        {
            if (timeEvent.Element.Item1 == null || timeEvent.Element.Item1.Equals(owner))
                toDequeue.Add(timeEvent.Element);
        }

        foreach (var removeEvent in toDequeue)
        {
            TimelineEvents.Remove(removeEvent, out _, out _);
        }
    }

    public static DateTime FromStart(int years, int months, int days = 0) =>
        START_DATE.AddYears(years).AddMonths(months).AddDays(days);
    public static DateTime FromNow(int years, int months, int days = 0) =>
        Now.AddYears(years).AddMonths(months).AddDays(days);
    public static DateTime FromNow(TimeSpan span) => Now + span;
    
    private void MoveToNextDay()
    {
        Now = Now.AddDays(1);
        DateChanged?.Invoke(Now);
        TryDequeueTimelineEvent();
    }

    private void TryDequeueTimelineEvent()
    {
        if (TimelineEvents.TryPeek(out (object, Action) payload, out DateTime eventTime) && eventTime <= Now)
        {
            TimelineEvents.Dequeue();
            payload.Item2.Invoke();
            TryDequeueTimelineEvent();
        }
    }

    public void CrashTheMarket()
    {
        MarketCrashed?.Invoke();
    }
}
