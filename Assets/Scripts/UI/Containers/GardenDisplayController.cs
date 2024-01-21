using Plants;
using Services;
using Stonks;
using UI.Model;
using UI.Plants;
using UnityEngine;

namespace UI.Containers
{
    public class GardenDisplayController : UIController<InventoryView, InventoryModel>, Container<TulipData.TulipVarietal, TulipController>, IService
    {
        public ContainerServer<TulipData.TulipVarietal, TulipController> Server { get; }
        
        private AudioService _audio;
        public GardenDisplayController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            Server = new Inventory(View.SlotControllers, 20);
            _audio = ServiceLocator.GetService<AudioService>();
            ServiceLocator.GetService<Economy>().SentToGarden += AddItem;
        }

        public void AddItem(TulipData.TulipVarietal data)
        {
            var tulip = Server.AddItem(data, null);
            tulip.interactable.Active = false; // these tulips cannot be played with
            _audio.PlayOneShot(View.sfx_place_item);
        }

        private void OnInventoryItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip.Data.Varietal);
        }
    }
}