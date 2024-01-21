using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Plants;
using UnityEngine;

public class IncidentWidgetView : UIView<IncidentWidgetModel>
{
    [SerializeField]
    TMP_Text debugDisplayText;
    public override void UpdateViewWithModel(IncidentWidgetModel model)
    {
        debugDisplayText.text = model.yarnNodeName;
    }
}
