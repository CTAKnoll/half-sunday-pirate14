using System;
using Plants;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class InventorySlotController : UIController<InventorySlotView, InventorySlotModel>
    {
        public bool IsEmpty => Tulip == null;
        public TulipController Tulip;
        public bool AllowStacking;
        private TulipData.TulipOwner Owner;
        public int Stacks;

        public Action<TulipController, IUIController> ConsumeFunction;
        private TulipData.TulipStage GeneratedStage;

        public InventorySlotController(InventorySlotView view, TulipData.TulipStage stage, bool allowStacking, bool ownedByPlayer) : base(view)
        {
            AllowStacking = allowStacking;
            GeneratedStage = stage;
            Owner = ownedByPlayer ? TulipData.TulipOwner.Player : TulipData.TulipOwner.Shop;
        }

        public void UpdateSlot(Inventory.InventoryStack invStack)
        {
            Stacks = AllowStacking ? invStack.Count : Math.Max(invStack.Count, 1);
            Model.Stacks = Stacks;
            Model.AllowStacking = AllowStacking;
            Tulip?.Close();
            if (invStack.Varietal != null)
            {
                Tulip = AddChild(new TulipData(invStack.Varietal, GeneratedStage, Owner).Serve(View.transform));
                Tulip.Consumed += ConsumeFunction;
            }
            UpdateViewAtEndOfFrame();
        }
    }
}