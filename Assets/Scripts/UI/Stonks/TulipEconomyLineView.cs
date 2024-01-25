using UnityEngine;

namespace UI.Stonks
{
    public class TulipEconomyLineView : UIView<TulipEconomyLineModel>
    {
        public LineRenderer Renderer;
        public override void UpdateViewWithModel(TulipEconomyLineModel model)
        {
            Renderer.positionCount = model.RecentPrices.Count;
            Renderer.startWidth = model.LineWidth;
            Renderer.endWidth = model.LineWidth;
            Renderer.startColor = model.LineColor;
            Renderer.endColor = model.LineColor;
            Renderer.SetPositions(model.RecentPrices.ToArray());
        }
    }
}