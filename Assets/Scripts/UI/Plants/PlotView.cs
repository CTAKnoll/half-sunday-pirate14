using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        
        public Image TulipImage;
        public Image WeedImage;
        
        public AudioEvent sfx_planted;

        public void Start()
        {
            Debug.Log(name + " " + transform.position);
        }
        
        public override void UpdateViewWithModel(PlotModel model)
        {
            DebugTextbox.text = model.DebugText;
            TulipImage.gameObject.SetActive(model.TulipShowing);
            WeedImage.gameObject.SetActive(model.WeedShowing);
            TulipImage.sprite = model.TulipImage;
            WeedImage.sprite = model.WeedImage;
        }
    }
}