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

    // Start is called before the first frame update
    void Start()
    {
        Economy = ServiceLocator.LazyLoad<Economy>();
        ServiceLocator.TryGetService(out UiDriver);
        ServiceLocator.TryGetService(out Audio);

        
        UiDriver.RegisterForTap(this, PurchaseFuture);
    }

    private void PurchaseFuture()
    {
        DateTime DecemberOfFuture = new DateTime(Timeline.Now.Year + NumYears, 12, 1);
        Audio.PlayOneShot(sfx_futurePurchased);
        Economy.BuyFuture(DecemberOfFuture, () => Audio.PlayOneShot(sfx_futurePayout));
    }

}
