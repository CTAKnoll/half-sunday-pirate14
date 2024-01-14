using System;
using System.Collections.Generic;
using Services;
using Utils;

public class Timeline : IService
{
    public const int START_YEAR = 1600;
    public const int START_MONTH = 1;
    public const int START_DAY = 1;

    private static readonly DateTime START_DATE = new DateTime(START_YEAR, START_MONTH, START_DAY);
    
    public static DateTime Now;
    private PriorityQueue<Action, DateTime> TimelineEvents;
    
    public Timeline()
    {
        Now = START_DATE;
        TimelineEvents = new();
        ServiceLocator.GetService<Ticker>().AddTickable(MoveToNextDay, 0.1f);
    }

    public void AddTimelineEvent(Action callback, DateTime eventTime) => TimelineEvents.Enqueue(callback, eventTime);

    public static DateTime FromStart(int years, int months, int days = 0) =>
        START_DATE.AddYears(years).AddMonths(months).AddDays(days);
    public static DateTime FromNow(int years, int months, int days = 0) =>
        Now.AddYears(years).AddMonths(months).AddDays(days);
    
    private void MoveToNextDay()
    {
        Now = Now.AddDays(1);
        TryDequeueTimelineEvent();
    }

    private void TryDequeueTimelineEvent()
    {
        TimelineEvents.TryPeek(out Action callback, out DateTime eventTime);
        if (eventTime <= Now)
        {
            TimelineEvents.Dequeue();
            callback.Invoke();
        }
    }
}
