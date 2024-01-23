using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Plants
{
    [Serializable]
    public class TulipView : UIView<TulipModel>
    {
        public Image TulipIcon;
        public AudioEvent sfx_pick_up;
        public AudioEvent sfx_plant_failed;
        
        public override void UpdateViewWithModel(TulipModel model)
        {
            transform.position = model.ScreenPos;
            TulipIcon.sprite = model.IconSprite;
        }

        public string ToString(TulipModel model)
        {
            return $"Pos:{transform.position} LocalPos:{transform.localPosition} ModelPos:{model.ScreenPos}";
        }
    }
}