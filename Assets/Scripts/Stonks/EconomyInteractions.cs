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

    public float GardenFeverModifier = 0.05f;

    private Economy Economy;
    private FeverMode FeverMode;
    private Competitions Competitions;
    private IncidentsManager IncidentsManager;

    // Start is called before the first frame update
    void Start()
    {
        _ = new TulipInteractionController(SellTulipButton, SellTulip);
        _ = new TulipInteractionController(SendToGardenButton, SendTulipToGarden);
        _ = new TulipInteractionController(SendToCompetitionButton, SendTulipToCompetition);

        Economy = ServiceLocator.LazyLoad<Economy>();
        Competitions = ServiceLocator.LazyLoad<Competitions>();
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
        ServiceLocator.TryGetService(out IncidentsManager);
        Competitions.RunCompetition(tulipData.Varietal, true);
        IncidentsManager.Dialogue.StartDialogue("TulipCompetition");
        return true;
    }
}
