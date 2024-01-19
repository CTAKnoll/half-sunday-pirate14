using System;
using System.Collections.Generic;
using Plants;
using Services;
using static Plants.TulipData;

namespace UI.Stonks
{
    public class Economy : IService
    {
        private float StartingFunds = 100;
        public float Funds { get; private set; }

        public Dictionary<TulipVarietal, TulipEconomy> TulipEconomyData;
        public event Action<float> FundsChanged;
        public Economy()
        {
            Funds = StartingFunds;
            TulipEconomyData = new();

            TulipVarietal redPlain = new TulipVarietal(TulipColor.Red, TulipKind.SolidColor);
            TulipEconomyData.Add(redPlain, new TulipEconomy(redPlain));
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