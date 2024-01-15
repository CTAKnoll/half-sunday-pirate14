namespace UI.Plants
{
    public class TulipView : UIView<TulipModel>
    {
        public override void UpdateViewWithModel(TulipModel model)
        {
            transform.position = model.ScreenPos;
        }
    }
}