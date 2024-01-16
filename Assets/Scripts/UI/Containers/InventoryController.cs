using Plants;
using UI.Plants;

namespace UI.Containers
{
    public class InventoryController : UIController<InventoryView, InventoryModel>, Container<TulipData, TulipController>
    {
        public ContainerServer<TulipData, TulipController> Server { get; }

        private Inventory.FilterFunction BulbInventoryFunction = data => data.OwnedByPlayer && data.UseBulbIcon;
        private Inventory.FilterFunction PickedInventoryFunction = data => data.OwnedByPlayer && !data.UseBulbIcon;

        public InventoryController(InventoryView view, bool isBulbInventory) : base(view)
        {
            Server = new Inventory(isBulbInventory ? BulbInventoryFunction : PickedInventoryFunction, View.InventorySlots);
        }

        private void RefreshInventory()
        {
            
        }

        private void ServeInventory()
        {
            
        }
    }
}