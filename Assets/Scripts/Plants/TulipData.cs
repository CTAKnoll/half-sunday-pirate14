using System;
using Services;
using UI.Containers;
using UI.Plants;
using UnityEngine;
using Utils;

namespace Plants
{
    public class TulipData : Plantable, Containable<TulipData, TulipController>
    {
        private Timeline Timeline; 
        public enum TulipKind
        {
            Empty,
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

        public static TulipData Empty = new (Color.clear, TulipKind.Empty);
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

        public TulipController Serve()
        {
            // TODO: Use templates to create a Tulip this way
            return new TulipController(new TulipView(), this);
        }

        public override string ToString()
        {
            return $"{Color.ToString()}\n{Enum.GetName(typeof(TulipStage), Stage)}\n{Enum.GetName(typeof(TulipKind), Kind)}";
        }
    }
}