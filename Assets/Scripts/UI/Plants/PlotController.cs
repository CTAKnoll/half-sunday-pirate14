using Plants;
using Services;
using Unity.VisualScripting;

namespace UI.Plants
{
    public class PlotController : UIController<PlotView, PlotModel>
    {
        private TulipData Tulip;
        public bool IsPlanted => Tulip != null;

        private AudioService _audio;
        
        public PlotController(PlotView view) : base(view)
        {
            UiDriver.RegisterForTap(View, OnClick);
            Model.DebugText = "UNPLANTED";
            Ticker.AddTickable(ModifyDisplay, 0.1f);
            _audio = ServiceLocator.GetService<AudioService>();
            UpdateViewAtEndOfFrame();
        }

        public void OnClick()
        {
            if (IsPlanted && Tulip.CanHarvest)
            {
                Tulip.Harvest();
                Tulip = null;
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

            _audio.PlayOneShot(View.sfx_planted);
        }
    }
}