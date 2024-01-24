﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Plants;
using Services;
using UnityEngine;
using Utils;
using Yarn.Unity;
using static Plants.TulipData;

namespace Stonks
{
    public class Economy : IService
    {
        private float StartingFunds = 100;
        public float Funds { get; private set; }

        public Dictionary<TulipVarietal, TulipEconomy> TulipEconomyData;

        public bool FilterToOwned = false;
        public event Action<float> FundsChanged;
        public event Action<TulipVarietal> VarietalAdded;
        public event Action<TulipVarietal> SentToGarden;

        private static IncidentsManager incManager;
        
        public Economy()
        {
            Funds = StartingFunds;
            TulipEconomyData = new();
            ServiceLocator.GetService<Timeline>().MarketCrashed += CrashTheMarket;
        }

        public SortedList<DateTime, TulipEconomy.PriceSnapshot> GetPriceData(TulipVarietal varietal, DateTime start, DateTime end)
        {
            return new SortedList<DateTime, TulipEconomy.PriceSnapshot>(TulipEconomyData[varietal].PriceHistory
                .Where(elem => elem.Key >= start && elem.Key <= end)
                .ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        public float GetCurrentPrice(TulipVarietal varietal)
        {
            return TulipEconomyData[varietal].Price;
        }

        [YarnFunction("get_market_price")]
        public static float GetCurrentPriceFromName(string kindName, string colorName)
        {
            var economy = ServiceLocator.GetService<Economy>();

            TulipKind kind = Enum.Parse<TulipKind>(kindName);
            TulipColor color = ColorToStringMapping
                .First
                ((kvp) => { return kvp.Value.ToLower().Equals(colorName.ToLower()); })
                .Key;

            return economy.TulipEconomyData[new TulipVarietal(color, kind)].Price;
        }

        [YarnFunction("get_avg_price")]
        public static float Average()
        {
            ServiceLocator.TryGetService(out Economy economy);
            return economy.TulipEconomyData.Average(data => data.Value.Price);
        }

        [YarnFunction("bank_balance")]
        public static float GetBankBalance()
        {
            ServiceLocator.TryGetService(out Economy economy);
            return economy.Funds;
        }

        public float GetAveragePrice()
        {
            return TulipEconomyData.Average(data => data.Value.Price);
        }

        public bool BuyTulip(TulipData data)
        {
            var buyPrice = 5;
            if (buyPrice > Funds)
                return false;
            Funds -= buyPrice;
            FundsChanged?.Invoke(Funds);
            return true;
        }

        public bool SellTulip(TulipData data)
        {
            Funds += GetCurrentPrice(data.Varietal).RoundToDecimalPlaces(2);
            FundsChanged?.Invoke(Funds);
            return true;
        }

        public bool SendTulipToGarden(TulipData data, float mod)
        {
            var price =GetCurrentPrice(data.Varietal);
            var normalized = price / GetAveragePrice();
            ServiceLocator.GetService<FeverMode>().Awareness.Value += normalized * mod;
            SentToGarden?.Invoke(data.Varietal);
            return true;
        }

        public void FilterToOwnedTulips(bool doFilter)
        {
            FilterToOwned = doFilter;
        }

        public void EnsureVarietal(TulipData data)
        {
            if (!TulipEconomyData.ContainsKey(data.Varietal))
            {
                TulipEconomyData.Add(data.Varietal, new TulipEconomy(data.Varietal));
                VarietalAdded?.Invoke(data.Varietal);
            }
        }

        private void CrashTheMarket()
        {
            TulipEconomy.TickMinimum = 0.9f;
            TulipEconomy.TickMaximum = 0.98f;
        }
        
    }
}