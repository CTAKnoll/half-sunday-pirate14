using UnityEngine;

[DisallowMultipleComponent]
public abstract class UIInteractable : MonoBehaviour
{
    public bool Active = true;
    public string TooltipText;

    [Header("Interaction SFX")]
    public AudioEvent sfx_onClick;
    public AudioEvent sfx_onRelease;

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
