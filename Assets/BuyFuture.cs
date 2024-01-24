using System;
using Services;
using Stonks;

public class BuyFuture : UIInteractable
{
    public int NumYears;
    private UIDriver UiDriver;
    private Economy Economy;

    // Start is called before the first frame update
    void Start()
    {
        Economy = ServiceLocator.GetService<Economy>();
        ServiceLocator.TryGetService(out UiDriver);
        
        UiDriver.RegisterForTap(this, PurchaseFuture);
    }

    private void PurchaseFuture()
    {
        DateTime DecemberOfFuture = new DateTime(Timeline.Now.Year + NumYears, 12, 1);
        Economy.BuyFuture(DecemberOfFuture);
    }
    
}
