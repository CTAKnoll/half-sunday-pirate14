using System.Collections;
using System.Collections.Generic;
using Services;
using Stonks;
using UnityEngine;

public class BuyDukeTitle : UIInteractable
{
    private UIDriver UiDriver;
    private Economy Economy;
    private TooltipServer TooltipServer;

    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.TryGetService(out UiDriver);
        Economy = ServiceLocator.LazyLoad<Economy>();
        UiDriver.RegisterForTap(this, TryPurchaseDukedom);

        UiDriver.RegisterForFocus(this, CreateTooltip, DestroyTooltip);
    }

    private void TryPurchaseDukedom()
    {
        Economy.BuyDukeTitle();
    }
    private void CreateTooltip()
    {
        if (TooltipServer == null)
            ServiceLocator.TryGetService(out TooltipServer);

        TooltipServer.SpawnTooltip(this.TooltipText);
    }

    private void DestroyTooltip()
    {
        TooltipServer.DisposeTooltip();
    }

}
