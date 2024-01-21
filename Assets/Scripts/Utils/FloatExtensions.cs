using System;
using UnityEngine;
using Random = System.Random;

namespace Utils
{
    public static class FloatExtensions
    {
        public static float RoundToDecimalPlaces(this float num, int places)
        {
            double multiplier = Math.Pow(10, places);
            return (int)(num * multiplier) / (float) multiplier;
        }

        public static float RandomBetween(float min, float max)
        {
            Random random = new Random();
            var value = random.Next(1000000);
            return Mathf.Lerp(min, max, value / 1000000f);
        }
    }
}