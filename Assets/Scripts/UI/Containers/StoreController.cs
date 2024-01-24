﻿using System;
using System.Collections.Generic;
using Plants;
using Services;
using UI.Core;
using UI.Model;
using UI.Plants;
using UnityEngine;
using Yarn.Compiler;
using Random = System.Random;

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

        public Dictionary<MerchantOrigin, TulipData.TulipKind> KindPreferences = new()
        {
            [MerchantOrigin.Ottoman] = TulipData.TulipKind.Spotted,
            [MerchantOrigin.Venice] = TulipData.TulipKind.Fancy,
            [MerchantOrigin.Morocco] = TulipData.TulipKind.Striped,
        };
        
        public Dictionary<MerchantOrigin, TulipData.TulipColor[]> ColorPreferences = new()
        {
            [MerchantOrigin.Ottoman] = new [] { TulipData.TulipColor.Yellow, TulipData.TulipColor.Pink, TulipData.TulipColor.Tangerine, TulipData.TulipColor.Red },
            [MerchantOrigin.Venice] = new [] { TulipData.TulipColor.Blue, TulipData.TulipColor.Coral, TulipData.TulipColor.White, TulipData.TulipColor.PurpleWhite },
            [MerchantOrigin.Morocco] = new [] { TulipData.TulipColor.Red, TulipData.TulipColor.Sunshine, TulipData.TulipColor.Purple, TulipData.TulipColor.PurpleWhite },
        };

        public MerchantOrigin ShopOrigin;
        private Dictionary<MerchantOrigin, Sprite> ShipSprites;

        public StoreController(StoreView view, StoreModel model) : base(view, model)
        {
            Server = new Store(View.StoreSlots);
            Timeline.AddRecurring(this, RefreshStore, TimeSpan.FromDays(120));
            Timeline.AddRecurring(this, ChangeMerchant, TimeSpan.FromDays(180));
            UiDriver.RegisterForFocus(View, CreateTooltip, DestroyTooltip);
        }

        public void InsertMerchantImages(Dictionary<MerchantOrigin, Sprite> sprites)
        {
            ShipSprites = sprites;
            ChangeMerchant();
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
            for (int i = 0; i < Server.MaxSize; i++)
            {
                Server.RemoveItem(i);
            }
            ShopOrigin = (MerchantOrigin) new Random().Next(Enum.GetNames(typeof(MerchantOrigin)).Length);
            Model.ShopShip = ShipSprites[ShopOrigin];
            RefreshStore();
            UpdateViewAtEndOfFrame();
        }

        private void RefreshStore()
        {
            while (Server.HasEmpty())
            {
                TulipData data = GenerateTulipOfMerchantType();
                Server.AddItem(data, OnStoreItemConsumed, out _);
            }
        }

        // 55% chance to be plain, 35% chance to be preferred special type, 5% chance each other special type
        // 70% chance to be one of 4 special colors, else random (20.5% chance to be a preferred color, 3% nonpreferred)
        private TulipData GenerateTulipOfMerchantType()
        {
            TulipData.TulipColor color;
            TulipData.TulipKind kind;
            
            double checkVal = new Random().NextDouble();
            if (checkVal > .45f)
                kind = TulipData.TulipKind.Plain;
            else if (checkVal > .1f)
                kind = KindPreferences[ShopOrigin];
            else if (checkVal > .05f)
                kind = KindPreferences[(MerchantOrigin) mod((int)ShopOrigin - 1, KindPreferences.Count)];
            else
                kind = KindPreferences[(MerchantOrigin) mod((int)ShopOrigin - 2, KindPreferences.Count)];
            
            checkVal = new Random().NextDouble();
            if (checkVal > .3f)
            {
                checkVal = new Random().NextDouble();
                color = ColorPreferences[ShopOrigin][(int)(ColorPreferences.Count * checkVal)];
            }
            else
                color = TulipData.TulipColor.Random;

            return new TulipData(color, kind);
        }

        private int mod(int x, int m)
        {
            return (x%m + m)%m;
        }

        private void OnStoreItemConsumed(TulipController tulip, IUIController consumer)
        {
            Server.RemoveItem(tulip.Data);
        }
    }
}