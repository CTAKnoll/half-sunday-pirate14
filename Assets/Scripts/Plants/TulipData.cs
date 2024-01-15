using System;
using Services;
using UnityEngine;

namespace Plants
{
    public class TulipData : Plantable
    {
        public enum TulipKind
        {
            SolidColor,
        }

        public enum TulipStage
        {
            Bulb, 
            Planted,
            FullBloom,
            Dead,
        }

        public Color Color { get; }
        public TulipKind Kind { get; }
        public TulipStage Stage { get; private set; }

        public bool CanHarvest => Stage == TulipStage.FullBloom;

        public TulipData(Color color, TulipKind kind)
        { 
            Color = color;
            Kind = kind;
            Stage = TulipStage.Bulb;
        }
        
        public void ScheduleAdvanceStage(DateTime time)
        {
            ServiceLocator.GetService<Timeline>().AddTimelineEvent(AdvanceStage, time);
        }

        private void AdvanceStage()
        {
            if (Stage != TulipStage.Dead)
                Stage = (TulipStage)((int) Stage + 1);
        }
    }
}