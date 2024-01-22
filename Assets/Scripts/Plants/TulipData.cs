using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using Stonks;
using UI.Containers;
using UI.Plants;
using UnityEngine;
using Utils;
using Yarn.Unity;

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
            Yellow,
            Magenta,
            Random
        }

        private static Dictionary<TulipColor, Color> TulipColorToColorMapping = new Dictionary<TulipColor, Color>
        {
            [TulipColor.Red] = Color.red,
            [TulipColor.Blue] = Color.blue,
            [TulipColor.Green] = Color.green,
            [TulipColor.Yellow] = Color.yellow,
            [TulipColor.Magenta] = Color.magenta,
        };
        
        private static Dictionary<Color, string> ColorToStringMapping = new Dictionary<Color, string>
        {
            [Color.red] = "Red",
            [Color.blue] = "Blue",
            [Color.green] = "Green",
            [Color.yellow] = "Yellow",
            [Color.magenta] = "Magenta",
        };
        
        public enum TulipKind
        {
            Plain,
            Fancy,
            Spotted,
            Striped,
            Random
        }

        public enum TulipStage
        {
            Bulb, 
            Planted,
            Sprout,
            Shoot,
            Bud,
            Bloom,
            FullBloom,
            OverBloom,
            Dead,
            Choking,
            Picked
        }

        public enum TulipOwner
        {
            Player,
            Shop
        }

        public class TulipVarietal : Containable<TulipVarietal, TulipController>
        {
            public Color Color;
            public TulipKind Kind;

            [YarnFunction("random_tulip_type")]
            public static string GetRandomTulip()
            {
                var rnd = new System.Random();
                int randIdx = rnd.Next(TulipColorToColorMapping.Keys.Count);

                var tulipColor = TulipColorToColorMapping.Keys.ToList()[randIdx];
                return Enum.GetName(typeof(TulipColor), tulipColor);
            }

            public TulipVarietal(TulipColor color, TulipKind kind)
            {
                Color = AssignColor(color);
                Kind = kind;
            }
            
            public TulipVarietal(Color color, TulipKind kind)
            {
                Color = color;
                Kind = kind;
            }

            public override bool Equals(object o)
            {
                if (o is not TulipVarietal varietal) return false;
                return Equals(varietal);
            }

            public bool Equals(TulipVarietal t)
            {
                return Color == t.Color && Kind == t.Kind;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Color, Kind);
            }

            public TulipController Serve(Transform parent)
            {
                return new TulipData(this).Serve(parent);
            }
        }
        
        public Color Color { get; }
        public TulipKind Kind { get; }
        public TulipStage Stage { get; private set; }
        
        public TulipOwner Owner { get; set; }

        private TulipVarietal _varietal;
        public TulipVarietal Varietal
        {
            get
            {
                if (_varietal == null)
                {
                    _varietal = new TulipVarietal(Color, Kind);
                }
                return _varietal;
            }
        }
        
        public bool UseBulbIcon => Stage == TulipStage.Bulb;
        public bool OwnedByPlayer => Owner == TulipOwner.Player;
        public bool IsPlanted => Stage != TulipStage.Bulb;
        public bool CanHarvest => Stage.IsOneOf(TulipStage.Bloom, TulipStage.FullBloom, TulipStage.OverBloom);
        
        public event Action OnDeath;
        public event Action StageChanged;

        public TulipInventoryController TulipInventory;
        private Economy Economy;
        
        public TulipData(TulipColor color, TulipKind kind, TulipStage stage = TulipStage.Bulb, TulipOwner owner = TulipOwner.Shop)
        { 
            Color = AssignColor(color);
            Kind = kind;
            Stage = stage;
            Owner = owner;

            Economy = ServiceLocator.GetService<Economy>();
            Economy.EnsureVarietal(this);

            Timeline = ServiceLocator.GetService<Timeline>();
            ServiceLocator.TryGetService(out TulipInventory);
        }

        public TulipData(TulipVarietal ofKind, TulipStage stage = TulipStage.Bulb, TulipOwner owner = TulipOwner.Shop)
        {
            Color = ofKind.Color;
            Kind = ofKind.Kind;
            Stage = stage;
            Owner = owner;
            
            Economy = ServiceLocator.GetService<Economy>();
            Economy.EnsureVarietal(this);
            
            Timeline = ServiceLocator.GetService<Timeline>();
            ServiceLocator.TryGetService(out TulipInventory);
        }
        
        public void Plant()
        {
            // The bloom is planted
            AdvanceStage();
            
            // schedule the next few stages
            Timeline.AddRecurring(this, AdvanceStage, TimeSpan.FromDays(30)); //schedule growing
        }

        public void Harvest()
        {
            ServiceLocator.TryGetService(out TulipInventory);

            Stage = TulipStage.Picked;
            Timeline.RemoveAllEvents(this);
            TulipInventory.AddItem(this);
        }

        protected static Color AssignColor(TulipColor color)
        {
            if (color == TulipColor.Random)
            {
                System.Random rnd = new ();
                int randIndex = rnd.Next(TulipColorToColorMapping.Values.Count);
                return TulipColorToColorMapping.Values.ToList()[randIndex];
            }
            return TulipColorToColorMapping[color];
        }
        
        protected static TulipKind AssignKind(TulipKind kind)
        {
            if (kind != TulipKind.Random)
                return kind;
            System.Random rnd = new ();
            int randIndex = rnd.Next(TulipColorToColorMapping.Values.Count - 1);
            return (TulipKind) Enum.GetValues(typeof(TulipKind)).GetValue(randIndex);
        }

        private void AdvanceStage()
        {
            if (Stage < TulipStage.Dead)
                Stage = (TulipStage)((int) Stage + 1);
            if (Stage > TulipStage.Dead)
                Stage = (TulipStage)((int) Stage - 1);

            if (Stage == TulipStage.Dead)
            {
                Timeline.RemoveAllEvents(this);
                Timeline.AddTimelineEvent(this, Cleanup, Timeline.FromNow(0, 1)); // remove self
            }
            

            StageChanged?.Invoke();
        }
        
        public void ChokeWithWeed(WeedData weed)
        {
            Timeline.RemoveAllEvents(this);
            TulipStage currStage = Stage;
            weed.OnDeath += () =>
            {
                if (this == null || Stage == TulipStage.Dead)
                    return;
                
                Timeline.RemoveAllEvents(this);
                Stage = currStage;
                Timeline.AddRecurring(this, AdvanceStage, TimeSpan.FromDays(30));
            };
            Stage =  TulipStage.Choking;
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 1));
            StageChanged?.Invoke();
        }

        private void Cleanup()
        {
            Timeline.RemoveAllEvents(this);
            OnDeath?.Invoke();
        }

        public TulipController Serve(Transform parent)
        {
            return new TulipController(ServiceLocator.GetService<TemplateServer>().Tulip, parent, this);
        }

        public override string ToString()
        {
            return $"{ColorToStringMapping[Color]}\n{Enum.GetName(typeof(TulipStage), Stage)}\n{Enum.GetName(typeof(TulipKind), Kind)}";
        }
    }
}