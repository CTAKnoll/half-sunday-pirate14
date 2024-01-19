using System;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Services;
using UnityEngine;
using Random = System.Random;

namespace UI.Stonks
{
    public class TulipEconomy
    {
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
                CreationDate = Timeline.Now;
            }

            public PriceSnapshot(PriceSnapshot previous, float multiplier)
            {
                BasePrice = previous.BasePrice;
                InflationMult = (float) Math.Pow(YEARLY_INFLATION, (Timeline.Now - Timeline.START_DATE).Days / 365f);
                MarketMult = previous.MarketMult * multiplier;
                IncidentModifiers = previous.IncidentModifiers.Where(modifier => modifier.ExpirationDate >= Timeline.Now).ToList();
                CreationDate = Timeline.Now;
                
                Debug.Log($"New Delicious Price! {Price}");
            }

            public readonly List<IncidentModifier> IncidentModifiers;
            public readonly DateTime CreationDate;
            public float Price => BasePrice * InflationMult * MarketMult *
                                  IncidentModifiers.Aggregate(1f, (current, next) => current * next.ModifierAmt);
        }


        public static readonly float BASE_TULIP_PRICE = 10f;
        public Stack<PriceSnapshot> PriceHistory;
        public float Price => PriceHistory.Peek().Price;
        public TulipData.TulipVarietal Varietal;
        
        private Random Random;
        private Timeline Timeline;

        public TulipEconomy(TulipData.TulipVarietal varietal)
        {
            Random = new Random();
            Timeline = ServiceLocator.GetService<Timeline>();
            PriceHistory = new();
            Varietal = varietal;
            
            PriceHistory.Push(new PriceSnapshot(BASE_TULIP_PRICE));
            Timeline.AddTimelineEvent(this, ModifyPrice, Timeline.FromNow(0, 0, 3));
        }

        public void AddIncidentModifier(float mult, DateTime expiration)
        {
            PriceHistory.Peek().IncidentModifiers.Add(new IncidentModifier(mult, expiration));
        }

        private void ModifyPrice()
        {
            float modifier = Random.Next(9900, 10100) / 10000f;
            PriceHistory.Push(new PriceSnapshot(PriceHistory.Peek(), modifier));
            Timeline.AddTimelineEvent(this, ModifyPrice, Timeline.FromNow(0, 0, 3));
        }
        
    }
}