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
    private AlertText AlertText;

    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.TryGetService(out UiDriver);
        ServiceLocator.TryGetService(out AlertText);
        Economy = ServiceLocator.LazyLoad<Economy>();
        UiDriver.RegisterForTap(this, TryPurchaseDukedom);

        UiDriver.RegisterForFocus(this, CreateTooltip, DestroyTooltip);
    }

    private void TryPurchaseDukedom()
    {
        if(!Economy.BuyDukeTitle())
            AlertText.Alert("You come to bargain for a Dukedom without funds?!", 5f);
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
