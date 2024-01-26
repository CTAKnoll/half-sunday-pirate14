using System;
using Services;
using Stonks;
using UnityEngine;

public class BuyFuture : UIInteractable
{
    public int NumYears;
    private UIDriver UiDriver;
    private Economy Economy;
    private AudioService Audio;

    [Header("SFX")]
    public AudioEvent sfx_futurePurchased;
    public AudioEvent sfx_futurePayout;

    private TooltipServer TooltipServer;

    // Start is called before the first frame update
    void Start()
    {
        Economy = ServiceLocator.LazyLoad<Economy>();
        ServiceLocator.TryGetService(out UiDriver);
        ServiceLocator.TryGetService(out Audio);

        
        UiDriver.RegisterForTap(this, PurchaseFuture);
        UiDriver.RegisterForFocus(this, CreateTooltip, DestroyTooltip);
    }

    private void PurchaseFuture()
    {
        DateTime DecemberOfFuture = new DateTime(Timeline.Now.Year + NumYears, 12, 1);
        Audio.PlayOneShot(sfx_futurePurchased);
        Economy.BuyFuture(DecemberOfFuture, () => Audio.PlayOneShot(sfx_futurePayout));
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
