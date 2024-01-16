using System;
using System.Collections.Generic;
using Plants;
using UI.Containers;
using UI.Model;
using UI.Model.Templates;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Plants
{
    public class TulipController : UIController<TulipView, TulipModel>, UIPlantable, BucketConsumable
    {

        public event Action<TulipController, IUIController> Consumed;
        
        private TulipData Data;
        Plantable UIPlantable.PlantingData => Data;

        private Vector3 DragOffset;
        private Vector3 InitialPosition;
        
        public TulipController(TulipTemplate template, Transform parent, TulipData data) : base(template, parent)
        {
            Data = data;
            Model.ScreenPos = parent.transform.position;
            Model.Color = data.Color;
            UiDriver.RegisterForHold(View, OnHoldStarted, OnHoldEnded, OnDrag, 0f);
            UpdateViewAtEndOfFrame();
        }
        
        private void OnHoldStarted()
        {
            InitialPosition = View.transform.position;
            DragOffset = InitialPosition - UiDriver.PointerPosition;
        }

        private void OnDrag()
        {
            Model.ScreenPos = UiDriver.PointerPosition + DragOffset;
            UpdateViewAtEndOfFrame();
        }

        private void OnHoldEnded()
        {
            void ReturnToOrigin()
            {
                Model.ScreenPos = InitialPosition;
                UpdateViewAtEndOfFrame();
            }

            bool ShopCheck()
            {
                if (Data.OwnedByPlayer) return true;
                return Purchase();
            }
            
            if (IsOverBucket(out Bucket consumer) && ControllerDb.GetControllerFromView(consumer.interactable, out IUIController contr))
            {
                if (contr is PlotController plot) // we're dropping on a plot
                {
                    if (Data.Stage != TulipData.TulipStage.Bulb)
                    {
                        throw new Exception("How did you do this?!");
                    }
                    if (plot.IsPlanted || !ShopCheck()) // you cant grow there, or you cant purchase
                    {
                        ReturnToOrigin();
                        return;
                    }
                    
                    plot.PlantTulip(Data);
                    Consumed?.Invoke(this, plot);
                }
                else if (contr is SimpleBucketController bucket) // we're dropping on a simple bucket
                {
                    // Give the data to the bucket, just like with plot
                }
                else if (contr is BulbInventoryController bulbInventory) // we're dropping on the inventory
                {
                    if (!bulbInventory.Server.HasEmpty() || !ShopCheck()) // you cant purchase
                    {
                        ReturnToOrigin();
                        return;
                    }
                    bulbInventory.AddItem(Data);
                    Consumed?.Invoke(this, bulbInventory);
                }
                else if (contr is TulipInventoryController tulipInventory) // we're dropping on the inventory
                {
                    if (!tulipInventory.Server.HasEmpty() || !ShopCheck()) // you cant purchase
                    {
                        ReturnToOrigin();
                        return;
                    }
                    tulipInventory.AddItem(Data);
                    Consumed?.Invoke(this, tulipInventory);
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
            pointerData.position = View.transform.position;
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

        private bool Purchase()
        {
            Data.Owner = TulipData.TulipOwner.Player;
            return true;
        }
    }
}