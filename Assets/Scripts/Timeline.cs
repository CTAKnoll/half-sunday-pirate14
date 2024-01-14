using System;
using System.Collections.Generic;
using Services;
using Utils;

public class Timeline : IService
{
    public const int START_YEAR = 1600;
    public const int START_MONTH = 1;
    public const int START_DAY = 1;
    
    public DateTime Now;
    private PriorityQueue<Action, DateTime> TimelineEvents;
    
    public Timeline()
    {
        Now = new DateTime(START_YEAR, START_MONTH, START_DAY);
        TimelineEvents = new();
        ServiceLocator.GetService<Ticker>().AddTickable(MoveToNextDay, 0.1f);
    }

    public void AddTimelineEvent(Action callback, DateTime eventTime) => TimelineEvents.Enqueue(callback, eventTime);

    private void MoveToNextDay()
    {
        Now = Now.AddDays(1);
        TryDequeueTimelineEvent();
    }

    private void TryDequeueTimelineEvent()
    {
        TimelineEvents.TryPeek(out Action callback, out DateTime eventTime);
        if (eventTime >= Now)
        {
            TimelineEvents.Dequeue();
            callback.Invoke();
        }
    }
}
