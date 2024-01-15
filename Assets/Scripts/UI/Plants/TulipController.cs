using System.Collections.Generic;
using Plants;
using UI.Model;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Plants
{
    public class TulipController : UIController<TulipView, TulipModel>, UIPlantable, BucketConsumable
    {
        private TulipData Data;
        Plantable UIPlantable.PlantingData => Data;

        private Vector3 DragOffset;
        private Vector3 InitialPosition;

        public TulipController(TulipView view, TulipData data) : base(view)
        {
            Data = data;
            Model.ScreenPos = View.transform.position;
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
            if (IsOverBucket(out Bucket consumer) && ControllerDb.GetControllerFromView(consumer.interactable, out IUIController contr))
            {
                if (contr is PlotController plot)
                {
                    plot.PlantTulip(Data);
                }

                if (contr is SimpleBucketController bucket)
                {
                    // Give the data to the plot
                }
                Close();
            }
            else
            {
                Model.ScreenPos = InitialPosition;
                UpdateViewAtEndOfFrame();
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
    }
}