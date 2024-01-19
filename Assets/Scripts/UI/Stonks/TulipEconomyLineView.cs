using UnityEngine;

namespace UI.Stonks
{
    public class TulipEconomyLineView : UIView<TulipEconomyLineModel>
    {
        public LineRenderer Renderer;
        public override void UpdateViewWithModel(TulipEconomyLineModel model)
        {
            Renderer.SetPositions(model.RecentPrices.ToArray());
        }
    }
}