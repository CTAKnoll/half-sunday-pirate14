using Plants;
using Services;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class BulbInventoryController : UIController<InventoryView, InventoryModel>, Container<TulipData, TulipController>, IService
    {
        public ContainerServer<TulipData, TulipController> Server { get; }

        public Inventory.FilterFunction BulbInventoryFunction = data => data.UseBulbIcon;

        private AudioService _audio;
        public BulbInventoryController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            Server = new Inventory(BulbInventoryFunction, View.InventorySlots);
            _audio = ServiceLocator.GetService<AudioService>();
        }

        public void AddItem(TulipData data)
        {
            var newTulip = Server.AddItem(data);
            newTulip.Consumed += OnInventoryItemConsumed;
            _audio.PlayOneShot(View.sfx_place_item);
        }

        private void OnInventoryItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip);
        }
    }
}