using Plants;
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
            RefreshStore();
        }

        private void RefreshStore()
        {
            while (Server.HasEmpty())
            {
                Server.AddItem(new TulipData(TulipData.TulipColor.Random, TulipData.TulipKind.SolidColor));
            }
        }
    }
}