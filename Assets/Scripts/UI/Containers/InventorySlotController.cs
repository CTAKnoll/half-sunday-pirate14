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

        public void UpdateSlot(Inventory.InventoryStack invStack)
        {
            Stacks = AllowStacking ? invStack.Count : Math.Max(invStack.Count, 1);
            Model.Stacks = Stacks;
            Model.AllowStacking = AllowStacking;
            Tulip?.Close();
            if (invStack.Varietal != null)
            {
                Tulip = AddChild(new TulipData(invStack.Varietal, GeneratedStage, TulipData.TulipOwner.Player).Serve(View.transform));
                Tulip.Consumed += ConsumeFunction;
            }
            UpdateViewAtEndOfFrame();
        }
    }
}