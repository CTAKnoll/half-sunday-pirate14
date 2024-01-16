using Plants;
using Services;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class BulbInventoryController : UIController<InventoryView, InventoryModel>, Container<TulipData, TulipController>, IService
    {
        public ContainerServer<TulipData, TulipController> Server { get; }

        private Inventory.FilterFunction BulbInventoryFunction = data => data.OwnedByPlayer && data.UseBulbIcon;

        public BulbInventoryController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            Server = new Inventory(BulbInventoryFunction, View.InventorySlots);
        }

        public void AddItem(TulipData data)
        {
            var newTulip = Server.AddItem(data);
            newTulip.Consumed += OnInventoryItemConsumed;
        }

        private void OnInventoryItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip);
        }
    }
}