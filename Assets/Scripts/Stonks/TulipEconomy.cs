using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Plants;
using Services;
using UnityEngine;
using Utils;
using Yarn.Unity;
using Random = System.Random;

namespace Stonks
{
    public class TulipEconomy
    {
        public static float TickMinimum = 0.99f;
        public static float TickMaximum = 1.01f;

        public static float FeverVolatileMin = 0.97f;
        public static float FeverVolatileMax = 1.03f;

        public static float FeverMinLevelUp = 0.99f;
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
            public static readonly float YEARLY_INFLATION = 1.02f;
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
        
        public SortedList<DateTime, PriceSnapshot> PriceHistory;
        public float Price => PriceHistory.Last().Value.Price;
        public TulipData.TulipVarietal Varietal;
        
        private Timeline Timeline;
        private Economy Economy;
        private FeverMode FeverMode;

        private bool FeverModeActive = false;
        private bool IsHotStreaking = false;
        private bool IsColdStreaking = false;

        public TulipEconomy(TulipData.TulipVarietal varietal)
        {
            Timeline = ServiceLocator.LazyLoad<Timeline>();
            Economy = ServiceLocator.LazyLoad<Economy>();
            ServiceLocator.TryGetService(out FeverMode);
            
            PriceHistory = new();
            Varietal = varietal;
            
            PriceHistory.Add(Timeline.Now, new PriceSnapshot(Economy.GetAveragePrice()));
            FeverMode.FeverLevel.OnChanged += (_, _) => FeverModeOn();
            Timeline.AddTimelineEvent(this, ModifyPrice, Timeline.FromNow(0, 0, 3));
        }

        [YarnCommand("modify_price")]
        public static void AddIncidentModifier(string tulipInfo, float mult, string dur)
        {
            TimeSpan duration = TimeSpan.Parse(dur);
            string[] tulipSplit = tulipInfo.Split(" ");
            if (tulipSplit.Length == 0)
            {
                foreach (TulipData.TulipKind eachKind in Enum.GetValues(typeof(TulipData.TulipKind)))
                {
                    foreach (TulipData.TulipColor eachColor in Enum.GetValues(typeof(TulipData.TulipColor)))
                    {
                        AddIncidentModifier(new TulipData.TulipVarietal(eachColor, eachKind), mult, duration);
                    }
                }
            }
            if (tulipSplit.Length == 1)
            {
                bool maybeColor = Enum.TryParse(tulipSplit[0], out TulipData.TulipColor color);
                bool maybeKind = Enum.TryParse(tulipSplit[0], out TulipData.TulipKind kind);
                if (maybeColor)
                {
                    foreach (TulipData.TulipKind eachKind in Enum.GetValues(typeof(TulipData.TulipKind)))
                    {
                        AddIncidentModifier(new TulipData.TulipVarietal(color, eachKind), mult, duration);
                    }
                }
                if (maybeKind)
                {
                    foreach (TulipData.TulipColor eachColor in Enum.GetValues(typeof(TulipData.TulipColor)))
                    {
                        AddIncidentModifier(new TulipData.TulipVarietal(eachColor, kind), mult, duration);
                    }
                }  
            }
            else
            {
                AddIncidentModifier(new TulipData.TulipVarietal(Enum.Parse<TulipData.TulipColor>(tulipSplit[0]), 
                    Enum.Parse<TulipData.TulipKind>(tulipSplit[1])), mult, duration);
            }
        }

        public static void AddIncidentModifier(TulipData.TulipVarietal varietal, float mult, TimeSpan duration)
        {
            ServiceLocator.TryGetService(out Economy economy);
            economy.TulipEconomyData[varietal].PriceHistory.Last().Value.
                IncidentModifiers.Add(new IncidentModifier(mult, Timeline.Now + duration));
        }

        public void AddHotStreak(TimeSpan duration)
        {
            IsHotStreaking = true;
            Timeline.AddTimelineEvent(this, () => IsHotStreaking = false, Timeline.Now + duration);
        }

        public void AddColdStreak(TimeSpan duration)
        {
            IsColdStreaking = true;
            Timeline.AddTimelineEvent(this, () => IsColdStreaking = false, Timeline.Now + duration);
        }

        private void ModifyPrice()
        {
            
            float marketMultiplier = Price / Economy.Average();
            float crash = FloatExtensions.RandomBetween(1f, 100f) < Math.Pow(marketMultiplier, 3) ? 0.9f : 1f;
            float boost = FloatExtensions.RandomBetween(1f, 100f) < Math.Pow(1/marketMultiplier, 3) ? 1.1f : 1f;
            
            if (!IsHotStreaking && !IsColdStreaking)
            {
                var random = FloatExtensions.RandomBetween(0f, 1f);
                if (random > 0.995 || boost > 1.01f)
                {
                    AddHotStreak(TimeSpan.FromDays(60));
                }
                if (random < 0.005 || crash < .99f)
                {
                    AddColdStreak(TimeSpan.FromDays(60));
                }
            }

            float feverLevelMin = (float)Math.Pow(FeverMinLevelUp, FeverMode.FeverLevel.Value);
            float feverLevelMax = (float)Math.Pow(FeverMaxLevelUp, FeverMode.FeverLevel.Value);
            float minVolatility = FeverModeActive ? FeverVolatileMin : 1;
            float maxVolatility = FeverModeActive ? FeverVolatileMax : 1;
            float isHotStreaking = IsHotStreaking ? 0.01f + 0.0025f * FeverMode.FeverLevel.Value : 0f;
            float isColdStreaking = IsColdStreaking ? -0.01f - 0.0025f * FeverMode.FeverLevel.Value : 0f;
            
            PriceHistory.Add(Timeline.Now, new PriceSnapshot(PriceHistory.Last().Value, 
                FloatExtensions.RandomBetween(TickMinimum * feverLevelMin * minVolatility * crash + isHotStreaking + isColdStreaking, 
                    TickMaximum * feverLevelMax * maxVolatility * boost + isHotStreaking + isColdStreaking)));
            Timeline.AddTimelineEvent(this, ModifyPrice, Timeline.FromNow(0, 0, 3));
        }

        private void FeverModeOn()
        {
            FeverModeActive = true;
            Timeline.AddTimelineEvent(this, () => FeverModeActive = false, Timeline.FromNow(0, 2));
        }
    }
}