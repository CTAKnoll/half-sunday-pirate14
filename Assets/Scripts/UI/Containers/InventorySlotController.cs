using System;
using Plants;
using UI.Model;
using UI.Plants;
using UnityEngine;

namespace UI.Containers
{
    public class InventorySlotController : UIController<InventorySlotView, InventorySlotModel>
    {
        public bool IsEmpty => Tulip == null;
        public TulipController Tulip;
        private bool AllowStacking;
        public int Stacks;

        public Action<TulipController, IUIController> ConsumeFunction;
        private TulipData.TulipStage GeneratedStage;

        public InventorySlotController(InventorySlotView view, TulipData.TulipStage stage, bool allowStacking) : base(view)
        {
            AllowStacking = allowStacking;
            GeneratedStage = stage;
        }

        public void UpdateSlot(TulipData.TulipVarietal tulipType, int number)
        {
            Debug.Log(number);
            Stacks = AllowStacking ? number : Math.Max(number, 1);
            Model.Stacks = Stacks;
            Model.AllowStacking = AllowStacking;
            Tulip?.Close();
            if (tulipType != null)
            {
                Tulip = AddChild(new TulipData(tulipType, GeneratedStage, TulipData.TulipOwner.Player).Serve(View.transform));
                Tulip.Consumed += ConsumeFunction;
            }
            UpdateViewAtEndOfFrame();
        }
    }
}