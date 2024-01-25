using System.Collections.Generic;
using Plants;
using Stonks;
using UI.Model.Templates;
using UnityEngine;

namespace UI.Stonks
{
    public class TulipEconomyLineController : UIController<TulipEconomyLineView, TulipEconomyLineModel>
    {
        public float GraphLineZ = -5;
        private Vector3 upperLeftBound;
        private Vector3 upperRightBound;
        private Vector3 lowerLeftBound;

        private TulipData.TulipVarietal Varietal;
            
        public TulipEconomyLineController(TulipEconomyLineTemplate template, RectTransform parent, TulipData.TulipVarietal varietal) : base(template, parent)
        {
            Model.RecentPrices = new();
            if (varietal.UnityColor.r + varietal.UnityColor.g + varietal.UnityColor.b >= 2.9)
            {
                Model.LineColor = new(1 - varietal.UnityColor.r, 1 - varietal.UnityColor.g, 1 - varietal.UnityColor.b);
            }
            else Model.LineColor = varietal.UnityColor;
            

            Varietal = varietal;
            
            var bounds = new Vector3[4];
            parent.GetWorldCorners(bounds);
            lowerLeftBound = bounds[0];
            upperLeftBound = bounds[1];
            upperRightBound = bounds[2];
        }

        public void BuildLine(List<TulipEconomy.PriceSnapshot> snapshots, float min, float max, bool visible)
        {
            View.gameObject.SetActive(visible);
            if (!visible)
                return;
            
            var xDistance = (upperRightBound - upperLeftBound).magnitude / snapshots.Count;
            var minY = lowerLeftBound.y;
            var maxY = upperLeftBound.y;
            var currX = upperLeftBound.x;

            if (Economy.Focused == null)
                Model.LineWidth = 1;
            else Model.LineWidth = Economy.Focused.Equals(Varietal) ? 3 : 1;
            
            List<Vector3> points = new();
            foreach (var snapshot in snapshots)
            {
                var yValue = Mathf.Lerp(minY, maxY, Mathf.InverseLerp(min, max, snapshot.Price));
                points.Add(new Vector3(currX, yValue, GraphLineZ));
                currX += xDistance;
            }
            
            Model.RecentPrices = points;
            UpdateViewAtEndOfFrame();
        }
        
     
    }
}