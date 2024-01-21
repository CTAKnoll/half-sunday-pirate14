using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Mission;
using UnityEngine;

public class IncidentWidgetController : UIController<IncidentWidgetView, IncidentWidgetModel>
{
    IncidentsManager inc;

    public event Action<IncidentWidgetController> completed;
    public event Action started;

    public IncidentWidgetController(IncidentWidgetView view, string yarnNode) : base(view)
    {
        Model.yarnNodeName = yarnNode;
        UiDriver.RegisterForTap(View, OnClick);

        inc = ServiceLocator.GetService<IncidentsManager>();
        View.UpdateViewWithModel(Model);
    }

    private void OnClick()
    {
        inc.Dialogue.StartDialogue(Model.yarnNodeName);
        started?.Invoke();
        inc.Dialogue.onDialogueComplete.AddListener(OnNodeComplete);
    }

    private void OnNodeComplete()
    {
        inc.Dialogue.onDialogueComplete.RemoveListener(OnNodeComplete);
        completed?.Invoke(this);

        Dispose();
    }

}
