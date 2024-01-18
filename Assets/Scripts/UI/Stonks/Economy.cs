using System;
using Plants;
using Services;
using UnityEngine;

namespace UI.Stonks
{
    public class Economy : IService
    {
        private float StartingFunds = 100;
        public float Funds { get; private set; }

        public event Action<float> FundsChanged;
        public Economy()
        {
            Funds = StartingFunds;
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
            Debug.Log("SELLING");
            Funds += 10;
            FundsChanged?.Invoke(Funds);
            return true;
        }
    }
}