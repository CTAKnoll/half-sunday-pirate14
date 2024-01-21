using System;
using System.Collections.Generic;
using Plants;
using Services;
using UI.Containers;
using UI.Model;
using UI.Model.Templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

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
        public TulipController(TulipTemplate template, Transform parent, TulipData data) : base(template, parent)
        {
            Data = data;
            Model.ScreenPos = parent.transform.position;
            Model.Color = data.Color;
            Model.IconSprite = ServiceLocator.GetService<TulipArtServer>().GetBaseSprite(data.Stage);
            UiDriver.RegisterForHold(View, OnHoldStarted, OnHoldEnded, OnDrag, 0f);
            UpdateViewAtEndOfFrame();
            _audio = ServiceLocator.GetService<AudioService>();
        }
        
        private void OnHoldStarted()
        {
            InitialPosition = View.transform.position;
            DragOffset = InitialPosition - MainCamera.ScreenToWorldPoint(UiDriver.PointerPosition);

            _audio.PlayOneShot(View.sfx_pick_up);
        }

        private void OnDrag()
        {
            Model.ScreenPos = MainCamera.ScreenToWorldPoint(UiDriver.PointerPosition) + DragOffset;
            Debug.Log($"{UiDriver.PointerPosition} {DragOffset} {Model.ScreenPos} {View.transform.position}");
            UpdateViewAtEndOfFrame();
        }

        private void OnHoldEnded()
        {
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

        private bool ShopCheck()
        {
            if (Data.OwnedByPlayer) return true;
            return Purchase();
        }

        private bool ConsumedByPlot(PlotController plot)
        {
            if (Data.Stage != TulipData.TulipStage.Bulb)
            {
                throw new Exception("How did you do this?!");
            }
            if (plot.IsPlanted || !ShopCheck()) // you cant grow there, or you cant purchase
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
            if (!bulbInventory.Server.HasEmpty() || !ShopCheck()) 
            {
                ReturnToOrigin();
                return false;
            }
            bulbInventory.AddItem(Data);
            Consumed?.Invoke(this, bulbInventory);
            return true;
        }

        private bool ConsumedByTulipInventory(TulipInventoryController tulipInventory)
        {
            if (!tulipInventory.Server.HasEmpty() || !ShopCheck()) 
            {
                ReturnToOrigin();
                return false;
            }
            tulipInventory.AddItem(Data);
            Consumed?.Invoke(this, tulipInventory);
            return true;
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
            if (!Economy.BuyTulip(Data))
                return false;
            Data.Owner = TulipData.TulipOwner.Player;
            return true;
        }
    }
}