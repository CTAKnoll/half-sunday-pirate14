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
        
        private AudioService _audio;
        public BulbInventoryController(InventoryView view) : base(view)
        {
            ServiceLocator.RegisterAsService(this);
            
            Server = new Inventory(View.SlotControllers);
            _audio = ServiceLocator.GetService<AudioService>();
            var inc = ServiceLocator.GetService<IncidentsManager>();
            InitYarnFunctions(inc.Dialogue);
        }



        public bool AddItem(TulipData data)
        {
            bool completed = Server.AddItem(data.Varietal, OnInventoryItemConsumed, out _);
            if(completed)
                _audio.PlayOneShot(View.sfx_place_item);
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