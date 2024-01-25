using System;
using System.Collections;
using Core;
using DefaultNamespace;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Fadeout : MonoBehaviour, IService
{
    public Image Overlay;
    public Image DutchFlag;
    public TextMeshProUGUI DutchText;
    public AnimationCurve FeverFlashAlpha;
    public float FlashNumSec = 2.5f;
    public event Action EndTheWorld;
    public int DaysUntilAlphaStart;
    public float AlphaIncreasePerDay;

    [Header("SFX")]
    public AudioEvent sfx_dutchAware;

    private AudioService Audio;
    private Timeline Timeline;
    private FeverMode FeverMode;
    
    void Start()
    {
        ServiceLocator.RegisterAsService(this);
        Timeline = ServiceLocator.LazyLoad<Timeline>();
        ChangeAlpha(Overlay, 0);
        ChangeAlpha(DutchFlag, 0);
        ChangeAlpha(DutchText, 0);

        ServiceLocator.TryGetService(out FeverMode);
        FeverMode.FeverLevel.OnChanged += OnFeverLevelChanged;
        ServiceLocator.LazyLoad<Timeline>().MarketCrashed += StartFadeOut;
        ServiceLocator.TryGetService(out Audio);
    }
    
    private static void ChangeAlpha(Image g, float newAlpha)
    {
        var color = g.color;
        color.a = newAlpha;
        g.color = color;
    }
    
    private static void ChangeAlpha(TextMeshProUGUI g, float newAlpha)
    {
        var color = g.color;
        color.a = newAlpha;
        g.color = color;
    }

    private void OnFeverLevelChanged(SmartNumber prevLevel, SmartNumber currLevel)
    {
        Audio.PlayOneShot(sfx_dutchAware);
        StartCoroutine(DutchFlash());
    }

    private IEnumerator DutchFlash()
    {
        float progress = 0;
        while (progress <= 1)
        {
            progress += 1 / (FlashNumSec * 30);
            ChangeAlpha(DutchFlag, FeverFlashAlpha.Evaluate(progress));
            ChangeAlpha(DutchText, FeverFlashAlpha.Evaluate(progress));
            yield return new WaitForSeconds(0.03f);
        }
        ChangeAlpha(DutchFlag, 0);
        ChangeAlpha(DutchText, 0);
    }

    private void StartFadeOut()
    {
        Timeline.AddTimelineEvent(this, 
            () => Timeline.AddRecurring(this, IncreaseAlpha, TimeSpan.FromDays(1)), 
            Timeline.FromNow(TimeSpan.FromDays(DaysUntilAlphaStart)));
    }

    private void IncreaseAlpha()
    {
        if (Overlay.color.a >= .999f)
        {
            Timeline.RemoveAllEvents(this);
            Timeline.StopTheWorld();
            
            EndTheWorld?.Invoke();
            ServiceLocator.LazyLoad<GameStateManager>().PanToState(GameStateManager.GameState.Credits);
            ChangeAlpha(Overlay, 0);
        }
        else
        {
            ChangeAlpha(Overlay, Overlay.color.a + AlphaIncreasePerDay);
        }
            
    }
}
