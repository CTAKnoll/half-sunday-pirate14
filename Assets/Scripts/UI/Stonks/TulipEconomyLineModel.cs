using System.Collections.Generic;
using UnityEngine;

namespace UI.Stonks
{
    public struct TulipEconomyLineModel : IUIModel
    {
        public List<Vector3> RecentPrices;
        public float LineWidth;
        public Color LineColor;
    }
}