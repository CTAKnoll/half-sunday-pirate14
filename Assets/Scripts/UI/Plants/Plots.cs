using System.Collections.Generic;
using Plants;
using UnityEngine;

namespace UI.Plants
{
    public class Plots : MonoBehaviour
    {
        public List<PlotView> PlotsList = new();
        public List<TulipView> TulipsTEST = new();

        public void Start()
        {
            foreach (var plot in PlotsList)
            {
                _ = new PlotController(plot);
            }
            
            foreach (var tulip in TulipsTEST)
            {
                _ = new TulipController(tulip, new TulipData(Color.red, TulipData.TulipKind.SolidColor));
            }
        }
    }
}