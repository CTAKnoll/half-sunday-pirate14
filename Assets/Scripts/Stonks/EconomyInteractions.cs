using System;
using Core;
using Plants;
using Services;
using UI.Plants;
using Stonks;
using UnityEngine;

public class EconomyInteractions : MonoBehaviour
{
    public SimpleBucketView SellTulipButton;
    public SimpleBucketView SendToGardenButton;
    public SimpleBucketView SendToCompetitionButton;

    public float GardenFeverModifier = 0.02f;

    private Economy Economy;
    private FeverMode FeverMode;
    
    // Start is called before the first frame update
    void Start()
    {
        _ = new TulipInteractionController(SellTulipButton, SellTulip);
        _ = new TulipInteractionController(SendToGardenButton, SendTulipToGarden);
        _ = new TulipInteractionController(SendToCompetitionButton, SendTulipToCompetition);

        Economy = ServiceLocator.LazyLoad<Economy>();
        ServiceLocator.TryGetService(out FeverMode);
    }

    private bool SellTulip(TulipData tulipData)
    {
        return Economy.SellTulip(tulipData);
    }

    private bool SendTulipToGarden(TulipData tulipData)
    {
        return Economy.SendTulipToGarden(tulipData, GardenFeverModifier);
    }
    
    private bool SendTulipToCompetition(TulipData tulipData)
    {
        throw new NotImplementedException();
    }
}
