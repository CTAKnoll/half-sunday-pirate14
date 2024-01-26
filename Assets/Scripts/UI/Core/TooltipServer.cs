using System.Collections;
using System.Collections.Generic;
using Services;
using UI.Core;
using UnityEngine;

public class TooltipServer : MonoBehaviour, IService
{
    public Vector3 Offset;
    
    private TooltipController Tooltip;
    private TemplateServer TemplateServer;

    protected void Awake()
    {
        ServiceLocator.RegisterAsService(this);
    }
    
    protected void Start()
    {
        ServiceLocator.TryGetService(out TemplateServer);
    }
    
    public void SpawnTooltip(string text)
    {
        Tooltip = new TooltipController(TemplateServer.Tooltip, transform, text);
    }
    
    public void DisposeTooltip()
    {
        if (Tooltip == null)
            return;
        Tooltip.Close();
        if (Tooltip.interactable.gameObject != null)
            Destroy(Tooltip.interactable);
        Tooltip = null;
    }
}
