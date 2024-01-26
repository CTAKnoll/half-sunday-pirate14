using System;
using System.Collections.Generic;
using Services;
using Utils;

public class Timeline : IService
{
    public const int START_YEAR = 1600;
    public const int START_MONTH = 1;
    public const int START_DAY = 1;
    public const float DAY_IN_REALTIME = 0.075f;

    public static readonly DateTime START_DATE = new (START_YEAR, START_MONTH, START_DAY);
    
    public static DateTime Now;
    private PriorityQueue<(object, Action), DateTime> TimelineEvents;
    public event Action<DateTime> DateChanged;
    public event Action MarketCrashed;

    private Ticker Ticker;
    private Action DayBreaks;
    
    public Timeline()   
    {
        Now = START_DATE;
        DayBreaks = MoveToNextDay;
        TimelineEvents = new();
        Ticker = ServiceLocator.LazyLoad<Ticker>();
        AddTimelineEvent(this, CrashTheMarket, FromNow(37, 2));
    }

    public void StartTheWorld()
    {
        Ticker.AddTickable(DayBreaks, DAY_IN_REALTIME);
    }

    public void StopTheWorld()
    {
        Ticker.RemoveTickable(DayBreaks);
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
            if(timeEvent.Element.Item1.Equals(owner))
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
