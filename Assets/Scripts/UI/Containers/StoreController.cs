using Plants;
using Services;
using UI.Core;
using UI.Model;
using UI.Plants;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Containers
{
    public class StoreController : UIController<StoreView, StoreModel>, Container<TulipData, TulipController>
    {
        public ContainerServer<TulipData, TulipController> Server { get; }

        private TemplateServer TemplateServer;
        private TooltipController Tooltip;

        public StoreController(StoreView view, StoreModel model) : base(view, model)
        {
            Server = new Store(View.StoreSlots);
            Ticker.AddTickable(RefreshStore, 10f);
            UiDriver.RegisterForFocus(View, CreateTooltip, DestroyTooltip);
            RefreshStore();
        }

        private void CreateTooltip()
        {
            if (TemplateServer == null)
                ServiceLocator.TryGetService(out TemplateServer);

            Tooltip = AddChild(new TooltipController(TemplateServer.Tooltip, View.transform, View));
        }

        private void DestroyTooltip()
        {
            Tooltip.Close();
            Tooltip = null;
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