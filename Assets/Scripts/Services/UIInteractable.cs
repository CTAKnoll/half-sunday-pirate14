using UnityEngine;

[DisallowMultipleComponent]
public abstract class UIInteractable : MonoBehaviour
{
    public bool Active = true;
    public string TooltipText;
    
    public enum UIInteractionPriority
    {
        VeryLow,
        Low,
        Default,
        High,
        VeryHigh
    }

    public UIInteractionPriority Priority = UIInteractionPriority.Default;
}
