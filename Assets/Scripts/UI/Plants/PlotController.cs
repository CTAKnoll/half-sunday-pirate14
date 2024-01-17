using Plants;
using Services;
using Unity.VisualScripting;

namespace UI.Plants
{
    public class PlotController : UIController<PlotView, PlotModel>
    {
        private TulipData Tulip;
        private WeedData Weed;
        public bool IsPlanted => Tulip != null;
        public bool IsWeeded => Weed != null;

        private AudioService _audio;
        
        public PlotController(PlotView view) : base(view)
        {
            UiDriver.RegisterForTap(View, OnClick);
            Model.DebugText = "UNPLANTED\nUNWEEDED";
            Ticker.AddTickable(ModifyDisplay, 0.1f);
            _audio = ServiceLocator.GetService<AudioService>();
            UpdateViewAtEndOfFrame();
        }

        public void OnClick()
        {
            if (IsWeeded)
            {
                Weed.Damage();
            }
            else if (IsPlanted && Tulip.CanHarvest && !IsWeeded)
            {
                Tulip.Harvest();
                Tulip = null;
            }
        }

        public void ModifyDisplay()
        {
            Model.DebugText = $"{Tulip?.ToString() ?? "UNPLANTED"}\n{Weed?.ToString() ?? "UNWEEDED"}";
            UpdateViewAtEndOfFrame();
        }

        public void PlantTulip(TulipData tulip)
        {
            Tulip = tulip;
            Tulip.OnDeath += () => Tulip = null;
            tulip.Plant();

            _audio.PlayOneShot(View.sfx_planted);
        }

        public void PlantWeed(WeedData weed)
        {
            Weed = weed;
            Weed.OnDeath += () => Weed = null;
            weed.Plant();
        }
    }
}