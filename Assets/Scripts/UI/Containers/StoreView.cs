using System.Collections.Generic;
using System.Linq;
using Plants;
using UnityEngine.UI;

namespace UI.Containers
{
    public class StoreView : UIView<StoreModel>
    {
        public List<InventorySlotView> StoreSlots;
        public List<InventorySlotController> SlotControllers = new();
        public Image StoreShip;

        public void Awake()
        {
            SlotControllers = StoreSlots.Select(view => new InventorySlotController(view, TulipData.TulipStage.Bulb, false)).ToList();
        }
        
        public override void UpdateViewWithModel(StoreModel model)
        {
            StoreShip.sprite = model.ShopShip;
        }
    }
}