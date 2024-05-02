using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Services;
using UnityEngine;
using UnityEngine.Scripting;
using DG.Tweening;

namespace UI.Plants
{
    public class PlotController : UIController<PlotView, PlotModel>
    {
        private TulipData Tulip;
        private WeedData Weed;
        private Coroutine SpreadingCoroutine;
        public bool IsPlanted => Tulip != null;
        
        private float FlyCompletion = 0;
        private Vector3 PickedInitialPos;
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

        private WeedArtServer WeedArtServer;
        private TulipArtServer TulipArtServer;
        private Plots Plots;
        private AlertText AlertText;
        private MainThreadScheduler MainThread;
        
        public PlotController(PlotView view) : base(view)
        {
            ServiceLocator.TryGetService(out Plots);
            ServiceLocator.TryGetService(out MainThread);
            ServiceLocator.TryGetService(out _audio);
            ServiceLocator.TryGetService(out WeedArtServer);
            ServiceLocator.TryGetService(out TulipArtServer);
            ServiceLocator.TryGetService(out AlertText);
            
            MainThread.ExecuteInLateUpdate(GetPlotConnections);
            
            UiDriver.RegisterForTap(View, OnClick);
            Model.DebugText = "UNPLANTED\nUNWEEDED";

            if (WeedArtServer != null)
            {
                Model.WeedAlertUpImage = WeedArtServer.GetSpreadSprite(PlotSpreadDirection.Up);
                Model.WeedAlertDownImage = WeedArtServer.GetSpreadSprite(PlotSpreadDirection.Down);
                Model.WeedAlertLeftImage = WeedArtServer.GetSpreadSprite(PlotSpreadDirection.Left);
                Model.WeedAlertRightImage = WeedArtServer.GetSpreadSprite(PlotSpreadDirection.Right);
            }

            Model.PickedIconVisible = false;
            PickedInitialPos = View.PickedTulipImage.gameObject.transform.position;
            Model.PickedIconPos = PickedInitialPos;

            Timeline.GamePaused += () => View.Active = false;
            Timeline.GameUnpaused += (_) => View.Active = true;

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
            if (!View.Active)
            {
                // TODO: Play a failed audio effect here
                return;
            }

            if (IsWeeded)
            {
                Weed.Damage();
                Audio.PlayOneShot(View.sfx_damageWeed);
            }
            else if (IsPlanted && Tulip.CanHarvest && !IsWeeded)
            {
                bool success = Tulip.Harvest();
                if (success)
                {
                    Audio.PlayOneShot(View.sfx_harvested);
                    TulipTween();
                }
                else
                {
                    AlertText.Alert("You cannot harvest into a full inventory!", 5f);
                }
            }
            else if (IsPlanted && Tulip.Stage == TulipData.TulipStage.Dead)
            {
                Tulip = null;
                UpdateTulipVisual();
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
            Weed.OnStageChanged += HandleWeedStage;
            Weed.OnDeath += () =>
            {
                Weed = null;
                SpreadDirection = PlotSpreadDirection.NotSpreading;
                
                Model.WeedShowing = false;
                Model.WeedAlertUp = false;
                Model.WeedAlertDown = false;
                Model.WeedAlertLeft = false;
                Model.WeedAlertRight = false;
                
                Audio.PlayOneShot(View.sfx_killWeed);
                UpdateViewAtEndOfFrame();
            };
            
            weed.Plant();
            UpdateWeedVisual();
        }

        private void TulipTween()
        {
            var endPos = Plots.PickedInventoryTarget.gameObject.transform.position;
            Model.TulipShowing = false;
            Model.PickedIconVisible = true;
            Model.PickedIconImage = TulipArtServer.GetBaseSprite(Tulip.Varietal, Tulip.Stage);

            void OnTweenComplete()
            {
                Model.PickedIconPos = PickedInitialPos;
                Model.PickedIconVisible = false;
                Tulip = null;
            }

            View.TweenToInventory(Model, endPos, OnTweenComplete);
        }

        private void UpdateTulipVisual()
        {
            Model.TulipShowing = IsPlanted && FlyCompletion == 0;
            if (Model.TulipShowing)
            {
                Model.TulipImage = TulipArtServer.GetBaseSprite(Tulip.Varietal, Tulip.Stage);
            }
            UpdateViewAtEndOfFrame();
        }
        
        private void UpdateWeedVisual()
        {
            Model.WeedShowing = IsWeeded;
            if (IsWeeded)
            {
                Model.WeedImage = WeedArtServer.GetBaseSprite(Weed.Stage);
            }
            UpdateViewAtEndOfFrame();
        }

        private void HandleWeedStage()
        {
            if (Weed != null && (int)Weed.Stage == (int)WeedData.WeedStage.Mature)
            {
                Tulip?.ChokeWithWeed(Weed);
            }
            UpdateWeedVisual();
        }
        
        // Note: This is ass
        // RE: No, this is patrick
        private void SpreadWeeds()
        {
            if (Weed == null)
                return;

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

            switch (SpreadDirection)
            {
                case PlotSpreadDirection.Up: Model.WeedAlertUp = true; break;
                case PlotSpreadDirection.Down: Model.WeedAlertDown = true; break;
                case PlotSpreadDirection.Left: Model.WeedAlertLeft = true; break;
                case PlotSpreadDirection.Right: Model.WeedAlertRight = true; break;
            }
            
            if(!contr?.IsWeeded ?? false)
                Timeline.AddTimelineEvent(Weed,  PlantAndSpread, Timeline.FromNow(0, 2));
            
            UpdateViewAtEndOfFrame();
        }

        private void PlantAndSpread()
        {
            if(SpreadDirection != PlotSpreadDirection.NotSpreading)
                DirectionalPlots[SpreadDirection]?.PlantWeed(new WeedData());
            SpreadWeeds();
        }
    }
}