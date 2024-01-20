using TMPro;

namespace UI.Stonks
{
    public class StonkGraphView : UIView<StonkGraphModel>
    {
        public TextMeshProUGUI MinValue;
        public TextMeshProUGUI MaxValue;
        
        public override void UpdateViewWithModel(StonkGraphModel model)
        {
            MinValue.text = model.MinValue.ToString();
            MaxValue.text = model.MaxValue.ToString();
        }
    }
}