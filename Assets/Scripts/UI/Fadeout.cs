using System;
using DefaultNamespace;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class Fadeout : MonoBehaviour, IService
{
    public Image Overlay;
    public event Action EndTheWorld;
    public int DaysUntilAlphaStart;
    public float AlphaIncreasePerDay;
    
    void Start()
    {
        ServiceLocator.RegisterAsService(this);
        ChangeAlpha(Overlay, 0);
        Overlay.gameObject.SetActive(false);
        ServiceLocator.GetService<Timeline>().MarketCrashed += StartFadeOut;
    }
    
    private static void ChangeAlpha(Image g, float newAlpha)
    {
        var color = g.color;
        color.a = newAlpha;
        g.color = color;
    }

    private void StartFadeOut()
    {
        Overlay.gameObject.SetActive(true);
        ServiceLocator.GetService<Timeline>().AddTimelineEvent(this, 
            () => ServiceLocator.GetService<Timeline>().AddRecurring(this, IncreaseAlpha, TimeSpan.FromDays(1)), 
            Timeline.FromNow(TimeSpan.FromDays(DaysUntilAlphaStart)));
    }

    private void IncreaseAlpha()
    {
        if (Overlay.color.a >= .999f)
        {
            ServiceLocator.GetService<Timeline>().RemoveAllEvents(this);
            EndTheWorld?.Invoke();
            ServiceLocator.GetService<GameStateManager>().PanToState(GameStateManager.GameState.Credits);
            Overlay.gameObject.SetActive(false);
        }
        else
        {
            ChangeAlpha(Overlay, Overlay.color.a + AlphaIncreasePerDay);
        }
            
    }
}
