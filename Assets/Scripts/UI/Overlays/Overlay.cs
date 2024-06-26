using System;
using System.Collections;
using Core;
using DefaultNamespace;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Overlay : MonoBehaviour, IService
{
    public Image Fadeout;
    public Image Pause;
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

    void Awake()
    {
        ServiceLocator.RegisterAsService(this);
        Timeline = ServiceLocator.LazyLoad<Timeline>();
        ChangeAlpha(Fadeout, 0);
        ChangeAlpha(Pause, 0);
        ChangeAlpha(DutchFlag, 0);
        ChangeAlpha(DutchText, 0);
    }

    void Start()
    {
        ServiceLocator.TryGetService(out FeverMode);
        ServiceLocator.TryGetService(out Audio);
        FeverMode.FeverLevel.OnChanged += OnFeverLevelChanged;

        Timeline timeline = ServiceLocator.LazyLoad<Timeline>();
        timeline.MarketCrashed += StartFadeOut;
        timeline.GamePaused += PauseOverlay;
        timeline.GameUnpaused += UnpauseOverlay;
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
            () => Timeline.AddRecurring(this, FadeoutOverTime, TimeSpan.FromDays(1)), 
            Timeline.FromNow(TimeSpan.FromDays(DaysUntilAlphaStart)));
    }

    private void FadeoutOverTime()
    {
        if (Fadeout.color.a >= .999f)
        {
            Timeline.RemoveAllEvents(this);
            Timeline.StopTheWorld();
            
            EndTheWorld?.Invoke();
            ServiceLocator.LazyLoad<GameStateManager>().PanToState(GameStateManager.GameState.Credits);
            ChangeAlpha(Fadeout, 0);
        }
        else
        {
            ChangeAlpha(Fadeout, Fadeout.color.a + AlphaIncreasePerDay);
        }
    }

    private void PauseOverlay()
    {
        ChangeAlpha(Pause, 0.5f);
    }

    private void UnpauseOverlay(Timeline.GameSpeed _)
    {
        ChangeAlpha(Pause, 0);
    }
}
