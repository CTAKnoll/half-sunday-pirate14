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

    [Header("SFX")]
    public AudioEvent sfx_sellTulip;
    public AudioEvent sfx_toGarden;
    public AudioEvent sfx_toCompetition;

    public float GardenFeverModifier = 0.02f;

    private Economy Economy;
    private FeverMode FeverMode;
    private AudioService Audio;
    // Start is called before the first frame update
    void Start()
    {
        _ = new TulipInteractionController(SellTulipButton, SellTulip);
        _ = new TulipInteractionController(SendToGardenButton, SendTulipToGarden);
        _ = new TulipInteractionController(SendToCompetitionButton, SendTulipToCompetition);

        ServiceLocator.TryGetService(out Audio);
        Economy = ServiceLocator.LazyLoad<Economy>();
        ServiceLocator.TryGetService(out FeverMode);
    }

    private bool SellTulip(TulipData tulipData)
    {
        Audio.PlayOneShot(sfx_sellTulip);
        return Economy.SellTulip(tulipData);
    }

    private bool SendTulipToGarden(TulipData tulipData)
    {
        Audio.PlayOneShot(sfx_toGarden);
        return Economy.SendTulipToGarden(tulipData, GardenFeverModifier);
    }
    
    private bool SendTulipToCompetition(TulipData tulipData)
    {
        Audio.PlayOneShot(sfx_toCompetition);
        throw new NotImplementedException();
    }
}
