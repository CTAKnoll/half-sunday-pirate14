using System;
using Plants;
using Services;
using UI.Core;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class StoreController : UIController<StoreView, StoreModel>, Container<TulipData, TulipController>
    {
        public ContainerServer<TulipData, TulipController> Server { get; }

        private TemplateServer TemplateServer;
        private TooltipController Tooltip;

        public enum MerchantOrigin
        {
            Venice,
            Ottoman,
            Morocco
        }

        public MerchantOrigin ShopOrigin;

        public StoreController(StoreView view, StoreModel model) : base(view, model)
        {
            Server = new Store(View.StoreSlots);
            Timeline.AddRecurring(this, RefreshStore, TimeSpan.FromDays(100));
            Timeline.AddRecurring(this, ChangeMerchant, TimeSpan.FromDays(365));
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

        public void ChangeMerchant()
        {
            //Server.Clear
            //Change Merchant Type
            RefreshStore();
        }

        private void RefreshStore()
        {
            while (Server.HasEmpty())
            {
                TulipData data = GenerateTulipOfMerchantType();
                Server.AddItem(data, OnStoreItemConsumed);
            }
        }

        private TulipData GenerateTulipOfMerchantType()
        {
            return new TulipData(TulipData.TulipColor.Random, TulipData.TulipKind.Plain);
        }

        private void OnStoreItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip.Data);
        }
    }
}