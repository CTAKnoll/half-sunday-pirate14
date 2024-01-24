using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Plants;
using Services;
using Utils;
using Random = System.Random;

namespace Stonks
{
    public class TulipEconomy
    {
        public static float TickMinimum = 0.99f;
        public static float TickMaximum = 1.01f;

        public static float FeverVolatileMin = 0.96f;
        public static float FeverVolatileMax = 1.05f;

        public static float FeverMinLevelUp = .99f;
        public static float FeverMaxLevelUp = 1.01f;
        
        public struct IncidentModifier
        {
            public float ModifierAmt;
            public DateTime ExpirationDate;

            public IncidentModifier(float mult, DateTime expiration)
            {
                ModifierAmt = mult;
                ExpirationDate = expiration;
            }
        }

        public readonly struct PriceSnapshot
        {
            public static readonly float YEARLY_INFLATION = 1.04f;
            private readonly float BasePrice;
            private readonly float InflationMult;
            private readonly float MarketMult;

            public PriceSnapshot(float basePrice)
            {
                BasePrice = basePrice;
                InflationMult = (float) Math.Pow(YEARLY_INFLATION, (Timeline.Now - Timeline.START_DATE).Days / 365f);
                MarketMult = 1;
                IncidentModifiers = new();
            }

            public PriceSnapshot(PriceSnapshot previous, float multiplier)
            {
                BasePrice = previous.BasePrice;
                InflationMult = (float) Math.Pow(YEARLY_INFLATION, (Timeline.Now - Timeline.START_DATE).Days / 365f);
                MarketMult = previous.MarketMult * multiplier;
                IncidentModifiers = previous.IncidentModifiers.Where(modifier => modifier.ExpirationDate >= Timeline.Now).ToList();
            }

            public readonly List<IncidentModifier> IncidentModifiers;
            public float Price => BasePrice * InflationMult * MarketMult *
                                  IncidentModifiers.Aggregate(1f, (current, next) => current * next.ModifierAmt);
        }


        public static readonly float BASE_TULIP_PRICE = 10f;
        public SortedList<DateTime, PriceSnapshot> PriceHistory;
        public float Price => PriceHistory.Last().Value.Price;
        public TulipData.TulipVarietal Varietal;
        
        private Timeline Timeline;
        private FeverMode FeverMode;

        private bool FeverModeActive = false;

        public TulipEconomy(TulipData.TulipVarietal varietal)
        {
            Timeline = ServiceLocator.LazyLoad<Timeline>();
            ServiceLocator.TryGetService(out FeverMode);
            
            PriceHistory = new();
            Varietal = varietal;
            
            PriceHistory.Add(Timeline.Now, new PriceSnapshot(BASE_TULIP_PRICE));
            FeverMode.FeverLevel.OnChanged += (_, _) => FeverModeOn();
            Timeline.AddTimelineEvent(this, ModifyPrice, Timeline.FromNow(0, 0, 3));
        }

        public void AddIncidentModifier(float mult, DateTime expiration)
        {
            PriceHistory.Last().Value.IncidentModifiers.Add(new IncidentModifier(mult, expiration));
        }

        private void ModifyPrice()
        {
            float feverLevelMin = (float)Math.Pow(FeverMinLevelUp, FeverMode.FeverLevel.Value);
            float feverLevelMax = (float)Math.Pow(FeverMaxLevelUp, FeverMode.FeverLevel.Value);
            float minVolatility = FeverModeActive ? FeverVolatileMin : 1;
            float maxVolatility = FeverModeActive ? FeverVolatileMax : 1;
            
            PriceHistory.Add(Timeline.Now, new PriceSnapshot(PriceHistory.Last().Value, 
                FloatExtensions.RandomBetween(TickMinimum * feverLevelMin * minVolatility, 
                    TickMaximum * feverLevelMax * maxVolatility)));
            Timeline.AddTimelineEvent(this, ModifyPrice, Timeline.FromNow(0, 0, 3));
        }

        private void FeverModeOn()
        {
            FeverModeActive = true;
            Timeline.AddTimelineEvent(this, () => FeverModeActive = false, Timeline.FromNow(0, 2));
        }
    }
}