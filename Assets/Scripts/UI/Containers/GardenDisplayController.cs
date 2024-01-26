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
            Server = new Inventory(View.SlotControllers);
            ServiceLocator.TryGetService(out _audio);
            ServiceLocator.LazyLoad<Economy>().SentToGarden += (varietal) => AddItem(varietal);
        }

        public bool AddItem(TulipData.TulipVarietal data)
        {
            bool success = Server.AddItem(data, null, out TulipController tulip);
            if (success)
            {
                tulip.interactable.Active = false; // these tulips cannot be played with
                _audio.PlayOneShot(View.sfx_place_item);
            }

            return success;
        }
    }
}