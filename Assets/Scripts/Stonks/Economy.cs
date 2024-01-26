using System;
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
        private static readonly float DEFAULT_TULIP_PRICE = 10;
        private float StartingFunds = 10000;
        public static float DukeTitleCost = 10000;
        public float Funds { get; private set; }

        public Dictionary<TulipVarietal, TulipEconomy> TulipEconomyData;

        public bool FilterToOwned = false;
        public TulipVarietal Focused;
        public event Action<float> FundsChanged;
        public event Action<TulipVarietal> VarietalAdded;
        public event Action<TulipVarietal> SentToGarden;

        private AlertText AlertText;
        private Timeline Timeline;

        public Economy()
        {
            Funds = StartingFunds;
            TulipEconomyData = new();
            Timeline = ServiceLocator.LazyLoad<Timeline>();
            Timeline.MarketCrashed += CrashTheMarket;
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
            var economy = ServiceLocator.LazyLoad<Economy>();

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
            if (TulipEconomyData.Count == 0)
                return DEFAULT_TULIP_PRICE;
            return TulipEconomyData.Average(data => data.Value.Price).RoundToDecimalPlaces(2);
        }

        public void BuyFuture(DateTime payoutDate)
        {
            ServiceLocator.TryGetService(out AlertText);
            if (Funds <= GetAveragePrice())
            {
                AlertText.Alert($"Too poor to spend ${GetAveragePrice()} on a Tulip Future!", 5f);
                return;
            }

            Funds -= GetAveragePrice();
            FundsChanged.Invoke(Funds);
            AlertText.Alert($"Bought a future for ${GetAveragePrice()}, pays out {payoutDate.ToString("MMMM yyyy")}", 5f);
            Timeline.AddTimelineEvent(this, PayoutFuture, payoutDate);
        }

        public void BuyFuture(DateTime payoutDate, Action onPayoutCallback)
        {
            ServiceLocator.TryGetService(out AlertText);
            if (Funds <= GetAveragePrice())
            {
                AlertText.Alert($"Too poor to spend ${GetAveragePrice()} on a Tulip Future!", 5f);
                return;
            }

            Funds -= GetAveragePrice();
            FundsChanged.Invoke(Funds);
            AlertText.Alert($"Bought a future for ${GetAveragePrice()}, pays out {payoutDate.ToString("MMMM yyyy")}", 5f);

            Timeline.AddTimelineEvent(
                this, 
                () => { PayoutFuture(); 
                        onPayoutCallback(); 
                }, 
                payoutDate);
        }

        private void PayoutFuture()
        {
            AlertText.Alert($"Your future is due! Paid out ${GetAveragePrice()}", 5f);
            Funds += GetAveragePrice();
            FundsChanged.Invoke(Funds);
        }

        public bool BuyTulip(TulipData data)
        {
            var buyPrice = TulipEconomyData[data.Varietal].Price * FloatExtensions.RandomBetween(0.25f, 0.5f);
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
            ServiceLocator.LazyLoad<FeverMode>().Awareness.Value += normalized * mod;
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

        public void WinCompetition(Competitions.CompetitionResults result)
        {
            Funds += result.PlayerPayout;
            FundsChanged?.Invoke(Funds);
        }

        public bool BuyDukeTitle()
        {
            if (Funds >= DukeTitleCost)
            {
                Funds -= DukeTitleCost;
                FundsChanged?.Invoke(Funds);
                Timeline.CrashTheMarket();
                return true;
            }
            return false;
        }

        private void CrashTheMarket()
        {
            TulipEconomy.TickMinimum = 0.9f;
            TulipEconomy.TickMaximum = 0.98f;
        }
        
    }
}