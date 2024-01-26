using System.Collections;
using System.Collections.Generic;
using Services;
using Stonks;
using UnityEngine;

public class BuyDukeTitle : UIInteractable
{
    private UIDriver UiDriver;
    private Economy Economy;
    
    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.TryGetService(out UiDriver);
        Economy = ServiceLocator.LazyLoad<Economy>();
        UiDriver.RegisterForTap(this, TryPurchaseDukedom);
    }

    private void TryPurchaseDukedom()
    {
        Economy.BuyDukeTitle();
    }
    
}
