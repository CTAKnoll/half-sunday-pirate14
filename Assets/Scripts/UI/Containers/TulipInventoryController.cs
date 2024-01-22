using Plants;
using Services;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class TulipInventoryController : UIController<InventoryView, InventoryModel>, Container<TulipData.TulipVarietal, TulipController>, IService
    {
        public ContainerServer<TulipData.TulipVarietal, TulipController> Server { get; }
        
        private AudioService _audio;
        public TulipInventoryController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            Server = new Inventory(View.SlotControllers);
            _audio = ServiceLocator.GetService<AudioService>();
        }

        public void AddItem(TulipData data)
        {
            Server.AddItem(data.Varietal, OnStoreItemConsumed);
            _audio.PlayOneShot(View.sfx_place_item);
        }

        public bool HasItem(TulipData.TulipVarietal varietal)
        {
            return Server.HasItem(varietal);
        }

        private void OnStoreItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip.Data.Varietal);
        }
    }
}