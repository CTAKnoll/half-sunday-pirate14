using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Plants
{
    [Serializable]
    public class TulipView : UIView<TulipModel>
    {
        public Image TulipIcon;
        
        public override void UpdateViewWithModel(TulipModel model)
        {
            transform.position = model.ScreenPos;
            TulipIcon.color = model.Color;
        }

        public string ToString(TulipModel model)
        {
            return $"Pos:{transform.position} LocalPos:{transform.localPosition} ModelPos:{model.ScreenPos}";
        }
    }
}