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
        private TulipArtServer ArtServer;

        public enum TulipColor
        {
            Red,
            White,
            Blue,
            Pink,
            Yellow,
            Purple,
            PurpleWhite,
            Sunshine,
            Tangerine, 
            Coral,
            Random
        }

        public static Dictionary<TulipColor, string> ColorToStringMapping = new Dictionary<TulipColor, string>
        {
            [TulipColor.Red] = "Red",
            [TulipColor.White] = "White",
            [TulipColor.Blue] = "Blue",
            [TulipColor.Pink] = "Pink",
            [TulipColor.Yellow] = "Yellow",
            [TulipColor.Purple] = "Purple",
            [TulipColor.PurpleWhite] = "PurpleWhite",
            [TulipColor.Sunshine] = "Sunshine",
            [TulipColor.Tangerine] = "Tangerine",
            [TulipColor.Coral] = "Coral",
        };
        
        public static Dictionary<TulipKind, string> KindToStringMapping = new Dictionary<TulipKind, string>
        {
            [TulipKind.Plain] = "Plain",
            //[TulipKind.Fancy] = "Fancy",
            //[TulipKind.Spotted] = "Spotted",
            //[TulipKind.Striped] = "Striped",
        };
        
        public enum TulipKind
        {
            Plain,
            //Fancy,
            //Spotted,
            //Striped,
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
            public TulipColor Color;
            public TulipKind Kind;

            public Color UnityColor;

            private TulipArtServer ArtServer;

            public static string GetRandomTulipKind()
            {
                var rnd = new System.Random();
                var kinds = Enum.GetNames(typeof(TulipKind));
                int randIdx = rnd.Next(1,kinds.Length);

                return kinds[randIdx];
            }

            //NOTE ::: This could generate a tulip that has never been seen before, and will add it to the economy
            // use sparingly
            public static string GetRandomTulipColor()
            {
                TulipData random = new TulipData(TulipColor.Random, TulipKind.Random);
                return $"{KindToStringMapping[random.Varietal.Kind]} {ColorToStringMapping[random.Varietal.Color]}";
            }

            public static TulipVarietal GetRandomTulipVarietal(bool seenOnly)
            {
                TulipVarietal varietal;
                float random = FloatExtensions.RandomBetween(0f, 1f);
                if (seenOnly || random > 0.5f)
                {
                    var keys = ServiceLocator.LazyLoad<Economy>().TulipEconomyData.Keys.ToList();
                    varietal = keys[new System.Random().Next(keys.Count)];
                }
                else
                {
                    List<TulipVarietal> varietals = ServiceLocator.LazyLoad<Economy>().TulipEconomyData.Keys.ToList();
                    List<TulipColor> seenColors = varietals.Select((varietal) => { return varietal.Color; }).ToList();
                    List<TulipKind> seenKinds = varietals.Select((varietal) => { return varietal.Kind; }).ToList();

                    var kind = GetUnseenKind(seenKinds);
                    var color = GetUnseenColor(seenColors);
                    varietal = new TulipVarietal(color, kind);
                }

                return varietal;
            }
            
            public static string GetRandomSeenTulip()
            {
                var keys = ServiceLocator.LazyLoad<Economy>().TulipEconomyData.Keys.ToList();
                TulipVarietal randomSeen = keys[new System.Random().Next(keys.Count)];
                return $"{KindToStringMapping[randomSeen.Kind]} {ColorToStringMapping[randomSeen.Color]}";
            }

            public static string GetRandomUnseenTulip()
            {
                List<TulipVarietal> varietals = ServiceLocator.LazyLoad<Economy>().TulipEconomyData.Keys.ToList();
                List<TulipColor> seenColors = varietals.Select((varietal) => { return varietal.Color; }).ToList();
                List<TulipKind> seenKinds = varietals.Select((varietal) => { return varietal.Kind; }).ToList();

                var kind = GetUnseenKind(seenKinds);
                var color = GetUnseenColor(seenColors);
                return $"{KindToStringMapping[kind]} {ColorToStringMapping[color]}";
            }

            static TulipColor GetUnseenColor(List<TulipColor> seen)
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
                ServiceLocator.TryGetService(out ArtServer);
                Color = AssignColor(color);
                Kind = AssignKind(kind);
                UnityColor = ArtServer.GetTulipColorData(this).Key1;
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
            
            public override string ToString()
            {
                return $"{ColorToStringMapping[Color]} {Enum.GetName(typeof(TulipKind), Kind)}";
            }
        }
        
        public TulipColor Color { get; }
        public Color UnityColor;
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

        private static bool _yarnInitialized;

        public float BuyPrice = -1;
        
        static TulipData()
        {            
            if(_yarnInitialized) 
                return;

            var dialogueRunner = ServiceLocator.LazyLoad<IncidentsManager>().Dialogue;
            InitYarnFunctions(dialogueRunner);
        }

        public TulipData(TulipColor color, TulipKind kind, TulipStage stage = TulipStage.Bulb, TulipOwner owner = TulipOwner.Shop)
        {
            ServiceLocator.TryGetService(out ArtServer);
            Color = AssignColor(color);
            Kind = AssignKind(kind);
            UnityColor = ArtServer.GetTulipColorData(Varietal).Key1;
            Stage = stage;
            Owner = owner;

            Economy = ServiceLocator.LazyLoad<Economy>();
            Economy.EnsureVarietal(this);

            Timeline = ServiceLocator.LazyLoad<Timeline>();
            ServiceLocator.TryGetService(out TulipInventory);
        }

        public TulipData(TulipVarietal ofKind, TulipStage stage = TulipStage.Bulb, TulipOwner owner = TulipOwner.Shop)
        {
            ServiceLocator.TryGetService(out ArtServer);
            Color = AssignColor(ofKind.Color);
            Kind = AssignKind(ofKind.Kind);
            UnityColor = ofKind.UnityColor;
            Stage = stage;
            Owner = owner;
            
            Economy = ServiceLocator.LazyLoad<Economy>();
            Economy.EnsureVarietal(this);
            
            Timeline = ServiceLocator.LazyLoad<Timeline>();
            ServiceLocator.TryGetService(out TulipInventory);
        }

        public static void InitYarnFunctions(DialogueRunner dialogueRunner)
        {
            //dialogueRunner.AddFunction("random_tulip", () => TulipVarietal.GetRandomSeenTulip());
            //dialogueRunner.AddFunction("random_tulip_kind", () => { return TulipVarietal.GetRandomSeenTulip().Split(" ")[0]; });
            //dialogueRunner.AddFunction("random_tulip_color", () => { return TulipVarietal.GetRandomSeenTulip().Split(" ")[1]; });

            //dialogueRunner.AddFunction("new_random_tulip", TulipVarietal.GetRandomUnseenTulip);
            //dialogueRunner.AddFunction("new_random_tulip_kind", () => { return TulipVarietal.GetRandomUnseenTulip().Split(" ")[0]; });
            //dialogueRunner.AddFunction("new_random_tulip_color", () => { return TulipVarietal.GetRandomUnseenTulip().Split(" ")[1]; });

            //_yarnInitialized = true;
        }
        public static string GetRandomTulipKind()
        {
            var rnd = new System.Random();
            var kinds = Enum.GetNames(typeof(TulipKind));
            int randIdx = rnd.Next(1, kinds.Length);

            return kinds[randIdx];
        }

        //NOTE ::: This could generate a tulip that has never been seen before, and will add it to the economy
        // use sparingly
        public static string GetRandomTulipColor()
        {
            TulipData random = new TulipData(TulipColor.Random, TulipKind.Random);
            return $"{KindToStringMapping[random.Varietal.Kind]} {ColorToStringMapping[random.Varietal.Color]}";
        }

        [YarnFunction("random_tulip")]
        public static string GetRandomSeenTulip()
        {
            var keys = ServiceLocator.LazyLoad<Economy>().TulipEconomyData.Keys.ToList();
            TulipVarietal randomSeen = keys[new System.Random().Next(keys.Count)];
            return $"{KindToStringMapping[randomSeen.Kind]} {ColorToStringMapping[randomSeen.Color]}";
        }

        [YarnFunction("random_tulip_kind")]
        public static string GetRandomSeenTulipKind()
        {
            return GetRandomSeenTulip().Split(" ")[0];
        }

        [YarnFunction("random_tulip_color")]
        public static string GetRandomSeenTulipColor()
        {
            return GetRandomSeenTulip().Split(" ")[1];
        }

        [YarnFunction("new_random_tulip")]
        public static string GetRandomUnseenTulip()
        {
            List<TulipVarietal> varietals = ServiceLocator.LazyLoad<Economy>().TulipEconomyData.Keys.ToList();
            List<TulipColor> seenColors = varietals.Select((varietal) => { return varietal.Color; }).ToList();
            List<TulipKind> seenKinds = varietals.Select((varietal) => { return varietal.Kind; }).ToList();

            var kind = GetUnseenKind(seenKinds);
            var color = GetUnseenColor(seenColors);
            return $"{KindToStringMapping[kind]} {ColorToStringMapping[color]}";
        }

        static TulipColor GetUnseenColor(List<TulipColor> seen)
        {
            System.Random rand = new System.Random();

            var allColors = ColorToStringMapping.Keys.ToList();
            var unseenColors = allColors.FindAll((color) => { return !seen.Contains(color); });

            if (unseenColors.Count == 0)
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

        [YarnFunction("new_random_tulip_kind")]
        public static string GetRandomUnseenTulipKind()
        {
            return GetRandomUnseenTulip().Split(" ")[0];
        }

        [YarnFunction("new_random_tulip_color")]
        public static string GetRandomUnseenTulipColor()
        {
            return GetRandomUnseenTulip().Split(" ")[1];
        }


        public void Plant()
        {
            // The bloom is planted
            AdvanceStage();
            
            // schedule the next few stages
            Timeline.AddRecurring(this, AdvanceStage, TimeSpan.FromDays(60)); //schedule growing
        }

        public bool Harvest()
        {
            ServiceLocator.TryGetService(out TulipInventory);

            bool success = TulipInventory.AddItem(this);
            if (success)
            {
                Stage = TulipStage.Picked;
                Timeline.RemoveAllEvents(this);
            }

            return success;
        }

        protected static TulipColor AssignColor(TulipColor color)
        {
            if (color != TulipColor.Random)
                return color;
            System.Random rnd = new ();
            int randIndex = rnd.Next(ColorToStringMapping.Values.Count - 1);
            return (TulipColor) Enum.GetValues(typeof(TulipColor)).GetValue(randIndex);
        }
        
        protected static TulipKind AssignKind(TulipKind kind)
        {
            if (kind != TulipKind.Random)
                return kind;
            System.Random rnd = new ();
            int randIndex = rnd.Next(ColorToStringMapping.Values.Count - 1);
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
                Timeline.AddTimelineEvent(this, Cleanup, Timeline.FromNow(0, 0, 90)); // remove self when dead
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
                StageChanged?.Invoke();
                Timeline.AddRecurring(this, AdvanceStage, TimeSpan.FromDays(60)); // Go back to not choking rate. This should be the same value as schedule in Plant
            };
            Stage =  TulipStage.Choking;
            Timeline.AddTimelineEvent(this, AdvanceStage, Timeline.FromNow(0, 0, 90)); //How long it takes to choke the plant out and transition to being dead.
            StageChanged?.Invoke();
        }

        private void Cleanup()
        {
            Timeline.RemoveAllEvents(this);
            OnDeath?.Invoke();
        }

        public TulipController Serve(Transform parent)
        {
            return new TulipController(ServiceLocator.LazyLoad<TemplateServer>().Tulip, parent, this);
        }

        public override string ToString()
        {
            return $"{ColorToStringMapping[Color]} {Enum.GetName(typeof(TulipKind), Kind)}";
        }
    }
}