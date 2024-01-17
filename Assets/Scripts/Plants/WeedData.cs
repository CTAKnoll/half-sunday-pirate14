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
        public WeedStage Stage;
        public int Health;

        private Timeline Timeline;
        public WeedData()
        {
            Timeline = ServiceLocator.GetService<Timeline>();
            Health = 3;
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
            if (Health <= 0)
                Cleanup();
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
        }

        public override string ToString()
        {
            return $"Weed: {Stage}\nHealth: {Health}";
        }
    }
}