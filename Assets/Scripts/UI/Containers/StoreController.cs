using Plants;
using UI.Model;
using UI.Plants;
using UnityEngine;

namespace UI.Containers
{
    public class StoreController : UIController<StoreView, StoreModel>, Container<TulipData, TulipController>
    {
        public ContainerServer<TulipData, TulipController> Server { get; }
        
        public StoreController(StoreView view) : base(view)
        {
            Server = new Store(View.StoreSlots);
            Ticker.AddTickable(RefreshStore, 10f);
            RefreshStore();
        }

        public StoreController(StoreView view, StoreModel model) : base(view, model)
        {
            Server = new Store(View.StoreSlots);
            Ticker.AddTickable(RefreshStore, 10f);
            RefreshStore();
        }

        private void RefreshStore()
        {
            while (Server.HasEmpty())
            {
                Server.AddItem(new TulipData(TulipData.TulipColor.Random, TulipData.TulipKind.SolidColor), OnStoreItemConsumed);
            }
        }

        private void OnStoreItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip.Data);
        }
    }
}