using Plants;
using Services;
using UI.Dialogue;
using UI.Model;
using UI.Plants;
using UnityEngine;
using Yarn.Unity;

namespace UI.Containers
{
    public class BulbInventoryController : UIController<InventoryView, InventoryModel>, Container<TulipData.TulipVarietal, TulipController>, IService, IYarnFunctionProvider
    {
        public ContainerServer<TulipData.TulipVarietal, TulipController> Server { get; }
        
        public BulbInventoryController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            
            Server = new Inventory(View.SlotControllers);
            var inc = ServiceLocator.LazyLoad<IncidentsManager>();
            InitYarnFunctions(inc.Dialogue);
        }



        public bool AddItem(TulipData data)
        {
            bool completed = Server.AddItem(data.Varietal, OnInventoryItemConsumed, out _);
            if (completed)
                Audio.PlayOneShot(View.sfx_place_item);
            else
                Audio.PlayOneShot(View.sfx_rejectItem);
            return completed;
        }

        private void OnInventoryItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip.Data.Varietal);
        }

        public void InitYarnFunctions(DialogueRunner dialogueRunner)
        {
            dialogueRunner.AddFunction("get_num_bulbs", () => { return Server.Count; });
        }
    }
}