using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using Unity.Mathematics;
using UnityEngine;

public class ResolutionButton : UIInteractable
{
    public int2 Resolution;
    public int ResolutionMultiplier;

    public static int RESOLUTION_MULT = 1;
    private UIDriver UiDriver;

    void Start()
    {
        ServiceLocator.TryGetService(out UiDriver);
        int maxSupportedWidth = Screen.resolutions.Max(res => res.width);
        int maxSupportedHeight = Screen.resolutions.Max(res => res.height);
        
        // if the resolution would be breaking, don't show it
        if (maxSupportedWidth < Resolution.x || maxSupportedHeight < Resolution.y)
        {
            gameObject.SetActive(false);
            return;
        }
        
        UiDriver.RegisterForTap(this, () =>
        {
            Screen.SetResolution(Resolution.x, Resolution.y, false);
            RESOLUTION_MULT = ResolutionMultiplier;
        });
    }
    
    
}
