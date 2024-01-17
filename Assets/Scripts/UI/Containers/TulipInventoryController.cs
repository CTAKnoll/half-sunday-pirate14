using Plants;
using Services;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class TulipInventoryController : UIController<InventoryView, InventoryModel>, Container<TulipData, TulipController>, IService
    {
        public ContainerServer<TulipData, TulipController> Server { get; }

        public Inventory.FilterFunction PickedInventoryFunction = data => data.OwnedByPlayer && !data.UseBulbIcon;

        private AudioService _audio;
        public TulipInventoryController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            Server = new Inventory(PickedInventoryFunction, View.InventorySlots);
            _audio = ServiceLocator.GetService<AudioService>();
        }

        public void AddItem(TulipData data)
        {
            var newTulip = Server.AddItem(data);
            newTulip.Consumed += OnStoreItemConsumed;
            _audio.PlayOneShot(View.sfx_place_item);
        }

        private void OnStoreItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip);
        }
    }
}