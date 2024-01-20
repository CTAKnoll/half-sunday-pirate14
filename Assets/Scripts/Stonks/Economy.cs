using System;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Services;
using UnityEngine;
using Utils;
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
            
            TulipVarietal greenPlain = new TulipVarietal(TulipColor.Green, TulipKind.SolidColor);
            TulipEconomyData.Add(greenPlain, new TulipEconomy(greenPlain));
            
            TulipVarietal bluePlain = new TulipVarietal(TulipColor.Blue, TulipKind.SolidColor);
            TulipEconomyData.Add(bluePlain, new TulipEconomy(bluePlain));
        }

        public SortedList<DateTime, TulipEconomy.PriceSnapshot> GetPriceData(TulipVarietal varietal, DateTime start, DateTime end)
        {
            return new SortedList<DateTime, TulipEconomy.PriceSnapshot>(TulipEconomyData[varietal].PriceHistory
                .Where(elem => elem.Key >= start && elem.Key <= end)
                .ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        public float GetCurrentPrice(TulipVarietal varietal)
        {
            Debug.Log(varietal.Color + " " + varietal.Kind);
            return TulipEconomyData[varietal].Price;
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

        
    }
}