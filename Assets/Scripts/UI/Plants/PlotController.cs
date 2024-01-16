using Plants;
using Unity.VisualScripting;

namespace UI.Plants
{
    public class PlotController : UIController<PlotView, PlotModel>
    {
        private TulipData Tulip;
        public bool IsPlanted => Tulip != null;
        
        public PlotController(PlotView view) : base(view)
        {
            UiDriver.RegisterForTap(View, OnClick);
            Model.DebugText = "UNPLANTED";
            Ticker.AddTickable(ModifyDisplay, 0.1f);
            UpdateViewAtEndOfFrame();
        }

        public void OnClick()
        {
            if (IsPlanted && Tulip.CanHarvest)
            {
                Tulip.Harvest();
            }
        }

        public void ModifyDisplay()
        {
            Model.DebugText = Tulip?.ToString() ?? "UNPLANTED";
            UpdateViewAtEndOfFrame();
        }

        public void PlantTulip(TulipData tulip)
        {
            Tulip = tulip;
            Tulip.OnDeath += () => Tulip = null;
            tulip.Plant();
        }
    }
}