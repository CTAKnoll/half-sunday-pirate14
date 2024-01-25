using System;
using Services;

namespace Plants
{
    public class WeedData : Plantable
    {
        public enum WeedStage
        {
            Sprout,
            Mature,
            Spreading
        }

        public event Action OnDeath;
        public event Action OnSpreading;
        public event Action OnStageChanged;
        public WeedStage Stage;
        public int Health;

        private Timeline Timeline;
        public WeedData()
        {
            Timeline = ServiceLocator.LazyLoad<Timeline>();
            Health = 1;
            Stage = WeedStage.Sprout;
        }

        public void Plant()
        {
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 2));
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 4));
        }

        public void Damage()
        {
            Health -= 1;
            Timeline.AddTimelineEvent(this, Heal, Timeline.FromNow(0, 1, 15));
            if (Health <= 0)
                Cleanup();
        }

        public void Heal()
        {
            Health += 1;
        }
        
        private void Cleanup()
        {
            Timeline.RemoveAllEvents(this);
            OnDeath?.Invoke();
        }
        
        private void AdvanceStage()
        {
            if (Stage < WeedStage.Spreading)
                Stage = (WeedStage)((int) Stage + 1);
            if (Stage == WeedStage.Spreading)
                OnSpreading?.Invoke();
            OnStageChanged?.Invoke();
        }

        public override string ToString()
        {
            return $"Weed: {Stage}\nHealth: {Health}";
        }
    }
}