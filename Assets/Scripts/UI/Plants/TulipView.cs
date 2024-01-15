using System;
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
    }
}