using System;
using Services;
using UnityEngine;
using Utils;

namespace Plants
{
    public class TulipData : Plantable
    {
        private Timeline Timeline; 
        public enum TulipKind
        {
            SolidColor,
        }

        public enum TulipStage
        {
            Bulb, 
            Planted,
            Shoot,
            Bud,
            Bloom,
            FullBloom,
            Overripe,
            Dead,
            Choking,
            Picked
        }

        public Color Color { get; }
        public TulipKind Kind { get; }
        public TulipStage Stage { get; private set; }

        public bool IsPlanted => Stage != TulipStage.Bulb;
        public bool CanHarvest => Stage.IsOneOf(TulipStage.Bloom, TulipStage.FullBloom, TulipStage.Overripe);

        public TulipData(Color color, TulipKind kind)
        { 
            Color = color;
            Kind = kind;
            Stage = TulipStage.Bulb;

            Timeline = ServiceLocator.GetService<Timeline>();
        }
        
        public void Plant()
        {
            // The bloom is planted
            AdvanceStage();
            
            // schedule the next few stages
            Timeline.AddTimelineEvent(AdvanceStage, Timeline.FromNow(0, 1)); //shoot
            Timeline.AddTimelineEvent(AdvanceStage, Timeline.FromNow(0, 2)); //bud
            Timeline.AddTimelineEvent(AdvanceStage, Timeline.FromNow(0, 3)); //bloom
            Timeline.AddTimelineEvent(AdvanceStage, Timeline.FromNow(0, 4)); //fullbloom
            Timeline.AddTimelineEvent(AdvanceStage, Timeline.FromNow(0, 5)); //overripe
            Timeline.AddTimelineEvent(AdvanceStage, Timeline.FromNow(0, 6)); //dead
        }

        private void AdvanceStage()
        {
            if (Stage < TulipStage.Dead)
                Stage = (TulipStage)((int) Stage + 1);
            if (Stage > TulipStage.Dead)
                Stage = (TulipStage)((int) Stage - 1);
        }
        
        

        public override string ToString()
        {
            return $"{Color.ToString()}\n{Enum.GetName(typeof(TulipStage), Stage)}\n{Enum.GetName(typeof(TulipKind), Kind)}";
        }
    }
}