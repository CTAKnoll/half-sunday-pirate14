using System;
using TMPro;
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

        public Image AboveWeedAlert;
        public Image BelowWeedAlert;
        public Image LeftWeedAlert;
        public Image RightWeedAlert;
        
        public TextMeshProUGUI DebugTextbox;
        
        public Image TulipImage;
        public Image WeedImage;
        public Image PickedTulipImage;
        
        public AudioEvent sfx_planted;

        public override void UpdateViewWithModel(PlotModel model)
        {
            DebugTextbox.text = model.DebugText;
            TulipImage.gameObject.SetActive(model.TulipShowing);
            WeedImage.gameObject.SetActive(model.WeedShowing);
            TulipImage.sprite = model.TulipImage;
            WeedImage.sprite = model.WeedImage;
                
            PickedTulipImage.gameObject.SetActive(model.PickedIconVisible);
            if (model.PickedIconVisible)
            {
                PickedTulipImage.sprite = model.PickedIconImage;
                PickedTulipImage.gameObject.transform.position = model.PickedIconPos;
            }
            
            AboveWeedAlert.gameObject.SetActive(model.WeedAlertUp && AboveLinkedPlot != null);
            BelowWeedAlert.gameObject.SetActive(model.WeedAlertDown && BelowLinkedPlot != null);
            LeftWeedAlert.gameObject.SetActive(model.WeedAlertLeft && LeftLinkedPlot != null);
            RightWeedAlert.gameObject.SetActive(model.WeedAlertRight && RightLinkedPlot != null);
        }
    }
}