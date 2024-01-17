using System.Collections.Generic;
using UnityEngine;

namespace UI.Plants
{
    public class Plots : MonoBehaviour
    {
        public List<PlotView> PlotsList = new();

        public void Start()
        {
            foreach (var plot in PlotsList)
            {
                _ = new PlotController(plot);
            }
        }
    }
}