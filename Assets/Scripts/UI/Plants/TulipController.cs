using System;
using System.Collections.Generic;
using Plants;
using Services;
using Stonks;
using UI.Containers;
using UI.Model;
using UI.Model.Templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utils;

namespace UI.Plants
{
    public class TulipController : UIController<TulipView, TulipModel>, UIPlantable, BucketConsumable
    {

        public event Action<TulipController, IUIController> Consumed;
        
        public readonly TulipData Data;
        Plantable UIPlantable.PlantingData => Data;

        private Vector3 DragOffset;
        private Vector3 InitialPosition;

        private AudioService _audio;
        private Economy Economy;

        public bool IsHeld;
        
        public TulipController(TulipTemplate template, Transform parent, TulipData data) : base(template, parent)
        {
            Data = data;
            Model.ScreenPos = parent.transform.position;
            Model.IconSprite = ServiceLocator.LazyLoad<TulipArtServer>().GetBaseSprite(data.Varietal, data.Stage);
            UiDriver.RegisterForHold(View, OnHoldStarted, OnHoldEnded, OnDrag, 0f);
            UiDriver.RegisterForFocus(View, OnHoverStart, OnHoverEnd);
            
            Economy = ServiceLocator.LazyLoad<Economy>();
            ServiceLocator.TryGetService(out _audio);

            View.Active = !Timeline.IsPaused;
            Timeline.GamePaused += () =>
            {
                if (IsHeld) // force return to origin is hard because of timing; we could amke this a coroutine?
                {
                    UiDriver.ForceRelease();
                }
                View.Active = false;
            };
            Timeline.GameUnpaused += (_) => View.Active = true;

            UpdateViewAtEndOfFrame();
           
        }

        protected void CreateBespokeTooltip() 
        {
            View.TooltipText = Data.ToString();

            if (Data.Stage == TulipData.TulipStage.Picked)
                View.TooltipText += $"\nCurrent: {Economy.GetCurrentPrice(Data.Varietal).RoundToDecimalPlaces(2)}";

            if (!Data.OwnedByPlayer)
                View.TooltipText += $"\nPrice: {Economy.QueryTulipPrice(Data).RoundToDecimalPlaces(2)}";
            
            if (TooltipServer == null)
                ServiceLocator.TryGetService(out TooltipServer);
            
            TooltipServer.SpawnTooltip(View.TooltipText);
        }
        
        private void OnHoldStarted()
        {
            if (!View.Active)
                return; 
            
            IsHeld = true;

            InitialPosition = View.transform.position;
            DragOffset = InitialPosition - MainCamera.ScreenToWorldPoint(UiDriver.PointerPosition);

            _audio.PlayOneShot(View.sfx_pick_up);
        }

        private void OnDrag()
        {
            Model.ScreenPos = MainCamera.ScreenToWorldPoint(UiDriver.PointerPosition) + DragOffset;
            UpdateViewAtEndOfFrame();
        }

        private void OnHoldEnded()
        {
            IsHeld = false;
            if (IsOverBucket(out Bucket consumer) && ControllerDb.GetControllerFromView(consumer.interactable, out IUIController contr))
            {
                if (contr is PlotController plot) // we're dropping on a plot
                {
                    if (!ConsumedByPlot(plot))
                        return;
                }
                else if (contr is TulipInteractionController bucket) // we're dropping on a simple bucket
                {
                    if (!ConsumedByTulipInteraction(bucket))
                        return;
                }
                else if (contr is BulbInventoryController bulbInventory) // we're dropping on the inventory
                {
                    if (!ConsumedByBulbInventory(bulbInventory))
                        return;
                }
                else if (contr is TulipInventoryController tulipInventory) // we're dropping on the inventory
                {
                    if (!ConsumedByTulipInventory(tulipInventory))
                        return;
                }
                Close();
            }
            else
            {
                ReturnToOrigin();
            }
        }

        private void OnHoverStart()
        {
            Economy.Focused = Data.Varietal;
            CreateBespokeTooltip();
        }

        private void OnHoverEnd()
        {
            Economy.Focused = null;
            TooltipServer.DisposeTooltip();
        }
        
        public bool IsOverBucket(out Bucket consumer)
        {
            consumer = null;
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Pointer.current.position.value;
            List<RaycastResult> results = new();
            UiDriver.Root.raycaster.Raycast(pointerData, results);
            foreach (RaycastResult result in results)
            {
                consumer = result.gameObject.GetComponent<Bucket>();
                if (consumer != null)
                    return true;
            }
            return false;
        }
        
        private void ReturnToOrigin()
        {
            Model.ScreenPos = InitialPosition;
            UpdateViewAtEndOfFrame();
            _audio.PlayOneShot(View.sfx_plant_failed);
        }

        private bool ShopCheck() => (!Data.OwnedByPlayer && Economy.Funds >= Data.BuyPrice) || Data.OwnedByPlayer;

        private bool ConsumedByPlot(PlotController plot)
        {
            if (Data.Stage != TulipData.TulipStage.Bulb)
            {
                throw new Exception("How did you do this?!");
            }
            if (plot.IsPlanted || plot.IsWeeded || !ShopCheck() || !Purchase()) // you cant grow there, or you cant purchase
            {
                ReturnToOrigin();
                return false;
            }
                    
            plot.PlantTulip(Data);
            Consumed?.Invoke(this, plot);
            return true;
        }

        private bool ConsumedByBulbInventory(BulbInventoryController bulbInventory)
        {
            if (!ShopCheck()) 
            {
                ReturnToOrigin();
                return false;
            }
            bool success = bulbInventory.AddItem(Data);
            if (success)
            {
                if (Purchase())
                {
                    Consumed?.Invoke(this, bulbInventory);
                    return true;
                }
                else
                {
                    ReturnToOrigin();
                    return false;
                }
            }
            else
            {
                ReturnToOrigin();
                return false;
            }
        }

        private bool ConsumedByTulipInventory(TulipInventoryController tulipInventory)
        {
            ReturnToOrigin();
            return false;
        }

        private bool ConsumedByTulipInteraction(TulipInteractionController interaction)
        {
            Debug.Log($"{Data.Stage != TulipData.TulipStage.Picked} {!Data.OwnedByPlayer}");
            if (Data.Stage != TulipData.TulipStage.Picked || !Data.OwnedByPlayer || !interaction.FireInteraction(Data))
            {
                ReturnToOrigin();
                return false;
            }
            Consumed?.Invoke(this, interaction);
            return true;
        }

        private bool Purchase()
        {
            if (Data.OwnedByPlayer)
                return true;
            if (!Economy.BuyTulip(Data))
                return false;
            Data.Owner = TulipData.TulipOwner.Player;
            return true;
        }

        public override void Close()
        {
            base.Close();
            TooltipServer.DisposeTooltip();
        }
    }
}