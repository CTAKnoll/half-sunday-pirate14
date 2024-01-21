using TMPro;

namespace UI.Core
{
    public class TooltipView : UIView<TooltipModel>
    {
        public TextMeshProUGUI TextBoxText;
        public override void UpdateViewWithModel(TooltipModel model)
        {
            transform.position = model.ScreenPos;
            TextBoxText.text = model.TooltipText;
        }
    }
}