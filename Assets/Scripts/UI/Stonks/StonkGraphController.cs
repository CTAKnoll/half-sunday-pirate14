﻿using System;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Services;
using Stonks;
using UI.Containers;
using UI.Model.Templates;
using UnityEngine;
using Utils;
using Yarn;

namespace UI.Stonks
{
    public class StonkGraphController : UIController<StonkGraphView, StonkGraphModel>
    {
        public TimeSpan Reachback = TimeSpan.FromDays(180);
        public Dictionary<TulipData.TulipVarietal, TulipEconomyLineController> StonkLines;
        private TulipInventoryController TulipInventory;

        private TulipEconomyLineTemplate Template;

        public StonkGraphController(StonkGraphView view) : base(view)
        {
            StonkLines = new();
            Template = ServiceLocator.LazyLoad<TemplateServer>().TulipEconomyLine;

            foreach (var varietal in Economy.TulipEconomyData)
            {
                AddNewLine(varietal.Key);
            }
            
            Timeline.AddRecurring(this, UpdateLineRenderers, TimeSpan.FromDays(3));
            Economy.VarietalAdded += AddNewLine;
        }

        private void AddNewLine(TulipData.TulipVarietal varietal)
        {
            StonkLines.Add(varietal, AddChild(new TulipEconomyLineController(Template, (RectTransform) View.transform, varietal)));
            UpdateLineRenderers();
        }

        private void UpdateLineRenderers()
        {
            if(TulipInventory == null)
                ServiceLocator.TryGetService(out TulipInventory);
            
            Dictionary<TulipData.TulipVarietal, List<TulipEconomy.PriceSnapshot>> histories = new();
            float min = float.MaxValue;
            float max = float.MinValue;
            foreach (var line in StonkLines)
            {
                var history = Economy.GetPriceData(line.Key, Timeline.Now - Reachback, Timeline.Now).Values.ToList();
                var localMin = history.Aggregate(float.MaxValue, (curr, next) => next.Price < curr ? next.Price : curr);
                var localMax = history.Aggregate(float.MinValue, (curr, next) => next.Price > curr ? next.Price : curr);
                min = Mathf.Min(min, localMin);
                max = Mathf.Max(max, localMax);
                histories.Add(line.Key, history);
            }

            Model.MinValue = min.RoundToDecimalPlaces(2);
            Model.MaxValue = max.RoundToDecimalPlaces(2);
            
            foreach (var line in StonkLines)
            {
                bool visibleNow = !Economy.FilterToOwned || TulipInventory.HasItem(line.Key);
                line.Value.BuildLine(histories[line.Key], min, max, visibleNow);
            }
            UpdateViewAtEndOfFrame();
        }
    }
}