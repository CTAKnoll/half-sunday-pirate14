using System;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Services;
using UnityEngine;

namespace UI.Plants
{
    public class PlotController : UIController<PlotView, PlotModel>
    {
        private TulipData Tulip;
        private WeedData Weed;
        private Coroutine SpreadingCoroutine;
        public bool IsPlanted => Tulip != null;
        public bool IsWeeded => Weed != null;

        public bool IsSpreading => IsWeeded && Weed.Stage == WeedData.WeedStage.Spreading;
        public PlotSpreadDirection SpreadDirection = PlotSpreadDirection.NotSpreading;
        
        public enum PlotSpreadDirection
        {
            NotSpreading,
            Up,
            Down,
            Left,
            Right,
        }

        private Dictionary<PlotSpreadDirection, PlotController> DirectionalPlots;

        private AudioService _audio;
        
        public PlotController(PlotView view) : base(view)
        {
            Ticker.AddTrigger(GetPlotConnections, 0.1f);
            UiDriver.RegisterForTap(View, OnClick);
            Model.DebugText = "UNPLANTED\nUNWEEDED";
            Ticker.AddTickable(ModifyDisplay, 0.1f);
            _audio = ServiceLocator.GetService<AudioService>();
            UpdateViewAtEndOfFrame();
        }

        // need to wait for all plots to be created before we get the connections, or else the controllerdb wont find them :)
        private void GetPlotConnections()
        {
            DirectionalPlots = new Dictionary<PlotSpreadDirection, PlotController>()
            {
                [PlotSpreadDirection.Up] = ControllerDb.GetControllerFromView(View.AboveLinkedPlot, out var up) ? (PlotController) up : null,
                [PlotSpreadDirection.Down] = ControllerDb.GetControllerFromView(View.BelowLinkedPlot, out var down) ? (PlotController) down : null,
                [PlotSpreadDirection.Left] = ControllerDb.GetControllerFromView(View.LeftLinkedPlot, out var left) ? (PlotController) left : null,
                [PlotSpreadDirection.Right] = ControllerDb.GetControllerFromView(View.RightLinkedPlot, out var right) ? (PlotController) right : null,
            };
        }

        public void OnClick()
        {
            if (IsWeeded)
            {
                Weed.Damage();
            }
            else if (IsPlanted && Tulip.CanHarvest && !IsWeeded)
            {
                Tulip.Harvest();
                Tulip = null;
            }
        }

        public void ModifyDisplay()
        {
            Model.DebugText = $"{Tulip?.ToString() ?? "UNPLANTED"}\n{Weed?.ToString() ?? "UNWEEDED"}";
            UpdateViewAtEndOfFrame();
        }

        public void PlantTulip(TulipData tulip)
        {
            Tulip = tulip;
            Tulip.OnDeath += () =>
            {
                Tulip = null;
                Model.TulipShowing = false;
                UpdateViewAtEndOfFrame();
            };
            Tulip.StageChanged += UpdateTulipVisual;
            tulip.Plant();

            UpdateTulipVisual();
            _audio.PlayOneShot(View.sfx_planted);
        }

        public void PlantWeed(WeedData weed)
        {
            Weed = weed;
            Weed.OnSpreading += SpreadWeeds;
            Weed.OnStageChanged += UpdateWeedVisual;
            Weed.OnDeath += () =>
            {
                Weed = null;
                SpreadDirection = PlotSpreadDirection.NotSpreading;
                Model.WeedShowing = false;
                UpdateViewAtEndOfFrame();
            };
            
            weed.Plant();
            UpdateWeedVisual();
        }

        private void UpdateTulipVisual()
        {
            Model.TulipShowing = IsPlanted;
            Model.TulipImage = ServiceLocator.GetService<TulipArtServer>().GetBaseSprite(Tulip.Stage);
            UpdateViewAtEndOfFrame();
        }
        
        private void UpdateWeedVisual()
        {
            Model.WeedShowing = IsWeeded;
            Model.WeedImage = ServiceLocator.GetService<WeedArtServer>().GetBaseSprite(Weed.Stage);
            UpdateViewAtEndOfFrame();
        }
        
        // Note: This is ass
        private void SpreadWeeds()
        {
            System.Random gen = new System.Random();
            Array directions = Enum.GetValues(typeof(PlotSpreadDirection));
            SpreadDirection = (PlotSpreadDirection) directions.GetValue(gen.Next(directions.Length));
            PlotController contr;
            while (!DirectionalPlots.TryGetValue(SpreadDirection, out contr) || contr == null || contr.IsWeeded)
            {
                if (DirectionalPlots.Values.All(plot => plot?.IsWeeded ?? true))
                    break;
                SpreadDirection = (PlotSpreadDirection) directions.GetValue(gen.Next(directions.Length));
            }
            
            if(!contr?.IsWeeded ?? false)
                Timeline.AddTimelineEvent(Weed,  PlantAndSpread, Timeline.FromNow(0, 2));
        }

        private void PlantAndSpread()
        {
            if(SpreadDirection != PlotSpreadDirection.NotSpreading)
                DirectionalPlots[SpreadDirection].PlantWeed(new WeedData());
            SpreadWeeds();
        }
    }
}