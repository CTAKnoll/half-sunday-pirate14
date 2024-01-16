using Plants;
using Services;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class TulipInventoryController : UIController<InventoryView, InventoryModel>, Container<TulipData, TulipController>, IService
    {
        public ContainerServer<TulipData, TulipController> Server { get; }

        private Inventory.FilterFunction PickedInventoryFunction = data => data.OwnedByPlayer && !data.UseBulbIcon;

        public TulipInventoryController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            Server = new Inventory(PickedInventoryFunction, View.InventorySlots);
        }

        public void AddItem(TulipData data)
        {
            var newTulip = Server.AddItem(data);
            newTulip.Consumed += OnStoreItemConsumed;
        }

        private void OnStoreItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip);
        }
    }
}