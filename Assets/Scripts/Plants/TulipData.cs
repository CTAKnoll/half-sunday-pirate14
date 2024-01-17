using System;
using System.Collections.Generic;
using System.Linq;
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

        public enum TulipColor
        {
            Green,
            Red,
            Blue,
            Random,
            Empty
        }

        private static Dictionary<TulipColor, Color> TulipColorToColorMapping = new Dictionary<TulipColor, Color>
        {
            [TulipColor.Red] = Color.red,
            [TulipColor.Blue] = Color.blue,
            [TulipColor.Green] = Color.green,
        };
        
        private static Dictionary<Color, string> ColorToStringMapping = new Dictionary<Color, string>
        {
            [Color.red] = "Red",
            [Color.blue] = "Blue",
            [Color.green] = "Green",
        };
        
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

        public enum TulipOwner
        {
            Player,
            Shop
        }

        public Color Color { get; }
        public TulipKind Kind { get; }
        public TulipStage Stage { get; private set; }
        
        public TulipOwner Owner { get; set; }

        public bool UseBulbIcon => Stage == TulipStage.Bulb;
        public bool OwnedByPlayer => Owner == TulipOwner.Player;
        public bool IsPlanted => Stage != TulipStage.Bulb;
        public bool CanHarvest => Stage.IsOneOf(TulipStage.Bloom, TulipStage.FullBloom, TulipStage.Overripe);

        public static TulipData Empty = new (TulipColor.Empty, TulipKind.Empty);

        public Action OnDeath;

        public TulipInventoryController TulipInventory;
        
        public TulipData(TulipColor color, TulipKind kind)
        { 
            Color = AssignColor(color);
            Kind = kind;
            Stage = TulipStage.Bulb;
            Owner = TulipOwner.Shop;

            Timeline = ServiceLocator.GetService<Timeline>();
            ServiceLocator.TryGetService(out TulipInventory);
        }
        
        public void Plant()
        {
            // The bloom is planted
            AdvanceStage();
            
            // schedule the next few stages
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 1)); //shoot
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 2)); //bud
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 3)); //bloom
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 4)); //fullbloom
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 5)); //overripe
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 6)); //dead
            Timeline.AddTimelineEvent(this, Cleanup, Timeline.FromNow(0, 7)); // kill self
        }

        public void Harvest()
        {
            if (TulipInventory == null)
                throw new Exception("Expected TulipInventory to exist!");

            Stage = TulipStage.Picked;
            Timeline.RemoveAllEvents(this);
            TulipInventory.AddItem(this);
        }

        private Color AssignColor(TulipColor color)
        {
            if (color == TulipColor.Random)
            {
                System.Random rnd = new ();
                int randIndex = rnd.Next(TulipColorToColorMapping.Values.Count);
                return TulipColorToColorMapping.Values.ToList()[randIndex];
            }
            if (color == TulipColor.Empty)
            {
                return Color.clear;
            }
            return TulipColorToColorMapping[color];
        }

        private void AdvanceStage()
        {
            if (Stage < TulipStage.Dead)
                Stage = (TulipStage)((int) Stage + 1);
            if (Stage > TulipStage.Dead)
                Stage = (TulipStage)((int) Stage - 1);
        }

        private void Cleanup()
        {
            Timeline.RemoveAllEvents(this);
            OnDeath?.Invoke();
        }

        public TulipController Serve(Transform parent)
        {
            Debug.Log($"Serving Tulip at {parent.name}");
            return new TulipController(ServiceLocator.GetService<TemplateServer>().Tulip, parent, this);
        }

        public override string ToString()
        {
            return $"{ColorToStringMapping[Color]}\n{Enum.GetName(typeof(TulipStage), Stage)}\n{Enum.GetName(typeof(TulipKind), Kind)}";
        }
    }
}