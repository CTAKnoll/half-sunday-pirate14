using System;
using UnityEngine;

namespace Utils
{
    public static class FloatExtensions
    {
        public static float RoundToDecimalPlaces(this float num, int places)
        {
            double multiplier = Math.Pow(10, places);
            return (int)(num * multiplier) / (float) multiplier;
        }
    }
}