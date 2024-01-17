using System;
using TMPro;

namespace UI.Plants
{
    [Serializable]
    public class PlotView : UIView<PlotModel>, Bucket
    {
        public TextMeshProUGUI DebugTextbox;
        public AudioEvent sfx_planted;
        public override void UpdateViewWithModel(PlotModel model)
        {
            DebugTextbox.text = model.DebugText;
        }
    }
}