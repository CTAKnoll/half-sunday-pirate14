using DG.Tweening;
using DG.Tweening.Plugins.Options;
using Plants;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Plants
{
    [Serializable]
    public class PlotView : UIView<PlotModel>, Bucket
    {
        [Header("SFX")]
        public AudioEvent sfx_planted;
        public AudioEvent sfx_harvested;
        public AudioEvent sfx_damageWeed;
        public AudioEvent sfx_killWeed;

        [Space]

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

        public void TweenToInventory(PlotModel Model, Vector3 endPosition, TweenCallback onComplete, float duration = 0.25f)
        {
            UpdateViewWithModel(Model);
            Transform tweenTarget = PickedTulipImage.gameObject.transform;

            Vector3 Getter() => tweenTarget.position;
            void Setter(Vector3 value) => tweenTarget.position = value;

            DOTween.To(
                Getter,
                Setter,
                endPosition,
                duration
                ).OnComplete(onComplete);
        }
    }
}