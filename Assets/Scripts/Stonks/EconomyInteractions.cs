using System;
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
    
    // Start is called before the first frame update
    void Start()
    {
        _ = new TulipInteractionController(SellTulipButton, SellTulip);
        _ = new TulipInteractionController(SendToGardenButton, SendTulipToGarden);
        _ = new TulipInteractionController(SendToCompetitionButton, SendTulipToCompetition);
    }

    private bool SellTulip(TulipData tulipData)
    {
        return ServiceLocator.GetService<Economy>().SellTulip(tulipData);
    }

    private bool SendTulipToGarden(TulipData tulipData)
    {
        throw new NotImplementedException();
    }
    
    private bool SendTulipToCompetition(TulipData tulipData)
    {
        throw new NotImplementedException();
    }
}
