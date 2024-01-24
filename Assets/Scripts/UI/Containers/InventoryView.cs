using System.Collections.Generic;
using System.Linq;
using Plants;
using UI.Plants;
using UnityEngine;

namespace UI.Containers
{
    public class InventoryView : UIView<InventoryModel>, Bucket
    {
        public List<InventorySlotView> InventorySlots;
        public List<InventorySlotController> SlotControllers;
        public AudioEvent sfx_place_item;
        
        public bool AllowStacking;
        public TulipData.TulipStage Stage;

        public void Start()
        {
            SlotControllers = InventorySlots.Select(view => new InventorySlotController(view, Stage, AllowStacking)).ToList();
        }
        
        public override void UpdateViewWithModel(InventoryModel model)
        {
            
        }
    }
}