using System;
using TMPro;
using UnityEngine;

namespace UI.Plants
{
    [Serializable]
    public class PlotView : UIView<PlotModel>, Bucket
    {
        public PlotView AboveLinkedPlot;
        public PlotView BelowLinkedPlot;
        public PlotView LeftLinkedPlot;
        public PlotView RightLinkedPlot;
        
        public TextMeshProUGUI DebugTextbox;
        public AudioEvent sfx_planted;

        public void Start()
        {
            Debug.Log(name + " " + transform.position);
        }
        
        public override void UpdateViewWithModel(PlotModel model)
        {
            DebugTextbox.text = model.DebugText;
        }
    }
}