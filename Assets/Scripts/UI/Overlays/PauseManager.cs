using Services;
using UnityEngine;

public class PauseManager : UIInteractable
{
    private UIDriver UiDriver;
    
    // this exists purely because I didnt want to deal with timing issues with having UiDriver registrations in Timeline
    private void Start()
    {
        ServiceLocator.TryGetService(out UiDriver);
        Timeline timeline = ServiceLocator.LazyLoad<Timeline>();
        UiDriver.RegisterForBack(this, () =>
        {
            Debug.Log("hoi");
            timeline.TogglePause();
        });
    }
}
