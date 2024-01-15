using System;
using TMPro;

namespace UI.Plants
{
    [Serializable]
    public class PlotView : UIView<PlotModel>, Bucket
    {
        public TextMeshProUGUI DebugTextbox;
        public override void UpdateViewWithModel(PlotModel model)
        {
            DebugTextbox.text = model.DebugText;
        }
    }
}