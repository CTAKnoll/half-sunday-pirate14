using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Plants
{
    public class PlotController : UIController<PlotView, PlotModel>
    {
        public PlotController(PlotView view) : base(view)
        {
           Debug.Log("TEST");
        }
    }
}