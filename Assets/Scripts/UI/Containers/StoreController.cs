using System.IO.Pipes;
using Plants;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class StoreController : UIController<StoreView, StoreModel>, Container<TulipData, TulipController>
    {
        public ContainerServer<TulipData, TulipController> Server { get; }

        private Store.FilterFunction StoreInventoryFunction = data => !data.OwnedByPlayer && data.UseBulbIcon;

        public StoreController(StoreView view) : base(view)
        {
            Server = new Store(StoreInventoryFunction, View.StoreSlots);
            Ticker.AddTickable(RefreshStore, 10f, true);
            RefreshStore();
        }

        private void RefreshStore()
        {
            UnityEngine.Debug.Log("Refreshing!");
            while (Server.HasEmpty())
            {
                var newTulip = Server.AddItem(new TulipData(TulipData.TulipColor.Random, TulipData.TulipKind.SolidColor));
                newTulip.Consumed += OnStoreItemConsumed;
            }
        }

        private void OnStoreItemConsumed(TulipController tulip, IUIController consumer)
        {
            UnityEngine.Debug.Log(Server.Count);
            Server.RemoveItem(tulip);
            UnityEngine.Debug.Log(Server.Count);
        }

        private void Debug()
        {
            UnityEngine.Debug.Log(Server.Count);
        }
    }
}