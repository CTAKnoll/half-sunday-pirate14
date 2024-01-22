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
        
        public readonly static Dictionary<Color, string> ColorToStringMapping = new Dictionary<Color, string>
        {
            [Color.red] = "Red",
            [Color.blue] = "Blue",
            [Color.green] = "Green",
            [Color.yellow] = "Yellow",
            [Color.magenta] = "Magenta",
        };
        
        private static Dictionary<TulipKind, string> KindToStringMapping = new Dictionary<TulipKind, string>
        {
            [TulipKind.Plain] = "Plain",
            [TulipKind.Fancy] = "Fancy",
            [TulipKind.Spotted] = "Spotted",
            [TulipKind.Striped] = "Striped",
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

            //NOTE ::: This could generate a tulip that has never been seen before, and will add it to the economy
            // use sparingly
            public static string GetRandomTulipKind()
            {
                var rnd = new System.Random();
                var kinds = Enum.GetNames(typeof(TulipKind));
                int randIdx = rnd.Next(1,kinds.Length);

                return kinds[randIdx];
            }

            public static string GetRandomTulipColor()
            {
                TulipData random = new TulipData(TulipColor.Random, TulipKind.Random);
                return $"{KindToStringMapping[random.Kind]} {ColorToStringMapping[random.Color]}";
            }
            
            public static string GetRandomSeenTulip()
            {
                var keys = ServiceLocator.GetService<Economy>().TulipEconomyData.Keys.ToList();
                TulipVarietal randomSeen = keys[new System.Random().Next(keys.Count)];
                return $"{KindToStringMapping[randomSeen.Kind]} {ColorToStringMapping[randomSeen.Color]}";
            }

            public static string GetRandomUnseenTulip()
            {
                List<TulipVarietal> varietals = ServiceLocator.GetService<Economy>().TulipEconomyData.Keys.ToList();
                List<Color> seenColors = varietals.Select((varietal) => { return varietal.Color; }).ToList();
                List<TulipKind> seenKinds = varietals.Select((varietal) => { return varietal.Kind; }).ToList();

                var kind = GetUnseenKind(seenKinds);
                var color = GetUnseenColor(seenColors);
                return $"{KindToStringMapping[kind]} {ColorToStringMapping[color]}";
            }

            static Color GetUnseenColor(List<Color> seen)
            {
                System.Random rand = new System.Random();

                var allColors = ColorToStringMapping.Keys.ToList();
                var unseenColors = allColors.FindAll((color) => { return !seen.Contains(color); });

                if(unseenColors.Count == 0)
                {
                    Debug.Log("No unseen colors remain. Returning seen color...");
                    return seen[rand.Next(seen.Count)];
                }

                return unseenColors[rand.Next(unseenColors.Count)];
            }

            static TulipKind GetUnseenKind(List<TulipKind> seen)
            {
                System.Random rand = new System.Random();
                var allKinds = Enum.GetValues(typeof(TulipKind)).Cast<TulipKind>();

                var unseenKinds = allKinds.Where
                    ((kind) => { return !seen.Contains(kind); })
                    .ToList();

                if (unseenKinds.Count == 0)
                {
                    Debug.Log("No unseen kinds remain. Returning seen kind...");
                    return seen[rand.Next(seen.Count)];
                }

                return unseenKinds[rand.Next(unseenKinds.Count)];
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

        private static bool _initialized;
        
        protected TulipData()
        {
            InitYarnFunctions();
        }

        public TulipData(TulipColor color, TulipKind kind, TulipStage stage = TulipStage.Bulb, TulipOwner owner = TulipOwner.Shop) : this()
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

        public TulipData(TulipVarietal ofKind, TulipStage stage = TulipStage.Bulb, TulipOwner owner = TulipOwner.Shop) : this()
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

        protected static void InitYarnFunctions()
        {
            if(_initialized) 
                return;

            var dialogueRunner = ServiceLocator.GetService<IncidentsManager>().Dialogue;

            dialogueRunner.AddFunction("random_tulip", TulipVarietal.GetRandomSeenTulip);
            dialogueRunner.AddFunction("random_tulip_type", () => { return TulipVarietal.GetRandomSeenTulip().Split(" ")[0]; });
            dialogueRunner.AddFunction("random_tulip_color", () => { return TulipVarietal.GetRandomSeenTulip().Split(" ")[1]; });

            dialogueRunner.AddFunction("new_random_tulip", TulipVarietal.GetRandomUnseenTulip);
            dialogueRunner.AddFunction("new_random_tulip_type", () => { return TulipVarietal.GetRandomUnseenTulip().Split(" ")[0]; });
            dialogueRunner.AddFunction("new_random_tulip_color", () => { return TulipVarietal.GetRandomUnseenTulip().Split(" ")[1]; });

            _initialized = true;
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