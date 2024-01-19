using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Services;
using UnityEngine.UI;
using static Plants.TulipData;

namespace Stonks
{
    public class Economy : IService
    {
        private float StartingFunds = 100;
        public float Funds { get; private set; }

        public Dictionary<TulipVarietal, TulipEconomy> TulipEconomyData;
        public event Action<float> FundsChanged;
        public event Action<TulipVarietal> VarietalAdded;
        public Economy()
        {
            Funds = StartingFunds;
            TulipEconomyData = new();

            TulipVarietal redPlain = new TulipVarietal(TulipColor.Red, TulipKind.SolidColor);
            TulipEconomyData.Add(redPlain, new TulipEconomy(redPlain));
            VarietalAdded?.Invoke(redPlain);
        }

        public SortedList<DateTime, TulipEconomy.PriceSnapshot> GetPriceData(TulipVarietal varietal, DateTime start, DateTime end)
        {
            return new SortedList<DateTime, TulipEconomy.PriceSnapshot>(TulipEconomyData[varietal].PriceHistory
                .Where(elem => elem.Key >= start && elem.Key <= end)
                .ToDictionary(kv => kv.Key, kv => kv.Value));

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
            Funds += 10;
            FundsChanged?.Invoke(Funds);
            return true;
        }
    }
}