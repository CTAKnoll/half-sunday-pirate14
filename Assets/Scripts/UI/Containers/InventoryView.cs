using System.Collections.Generic;
using UI.Plants;

namespace UI.Containers
{
    public class InventoryView : UIView<InventoryModel>, Bucket
    {
        public List<UIInteractable> InventorySlots;
        public AudioEvent sfx_place_item;
        public override void UpdateViewWithModel(InventoryModel model)
        {
            
        }
    }
}