using Plants;

namespace UI.Plants
{
    public class PlotController : UIController<PlotView, PlotModel>
    {
        private TulipData Tulip;
        public bool IsPlanted => Tulip != null;
        
        public PlotController(PlotView view) : base(view)
        {
            Model.DebugText = "UNPLANTED";
            UpdateViewAtEndOfFrame();
        }

        public void ModifyDisplay()
        {
            Model.DebugText = Tulip?.ToString() ?? "UNPLANTED";
            UpdateViewAtEndOfFrame();
        }

        public void PlantTulip(TulipData tulip)
        {
            Tulip = tulip;
            Ticker.AddTickable(ModifyDisplay, 0.1f, true);
            tulip.Plant();
        }
    }
}