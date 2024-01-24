using Plants;
using Services;
using Stonks;
using System.Collections.Generic;
using System.Linq;
using UI.Dialogue;
using UI.Model;
using UI.Plants;
using Yarn.Unity;
using static UI.Containers.Inventory;

namespace UI.Containers
{
    public class TulipInventoryController : UIController<InventoryView, InventoryModel>, Container<TulipData.TulipVarietal, TulipController>, IService, IYarnFunctionProvider
    {
        public Inventory Inventory { get; }
        public ContainerServer<TulipData.TulipVarietal, TulipController> Server => Inventory;

        private AudioService _audio;
        public TulipInventoryController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            Inventory = new Inventory(View.SlotControllers);
            _audio = ServiceLocator.GetService<AudioService>();
            var inc = ServiceLocator.GetService<IncidentsManager>();
            InitYarnFunctions(inc.Dialogue);
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

        public void InitYarnFunctions(DialogueRunner dialogueRunner)
        {
            dialogueRunner.AddFunction("get_num_tulips", () => { return Server.Count; });
        }

        [YarnFunction("total_owned_tulips_market_price")]
        public static float GetInventoryValue()
        {
            ServiceLocator.TryGetService(out TulipInventoryController inventoryController);
            ServiceLocator.TryGetService(out Economy economy);

            List<TulipEconomy> tulipEconData = new();
            float sum = 0;
            for(int i = 0; i < inventoryController.Inventory.Elements.Length; i++)
            {
                InventoryStack invStack = inventoryController.Inventory.Elements[i];

                economy.TulipEconomyData.TryGetValue(invStack.Varietal, out TulipEconomy tulipEcon);
                sum += invStack.Count * tulipEcon.Price;
            }


            return sum;
        }
    }
}