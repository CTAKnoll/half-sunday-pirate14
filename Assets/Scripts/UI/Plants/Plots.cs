using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plants;
using Services;
using UnityEngine;
using Utils;
using Debug = UnityEngine.Debug;

namespace UI.Plants
{
    public class Plots : MonoBehaviour, IService
    {
        public List<PlotView> PlotsList = new();
        private List<PlotController> PlotControllers = new();
        private WaitForSeconds WaitABit = new(0.1f);

        public float NoWeedsModifier = 1;
        public float MinTimeWithWeeds = 20;
        public float MaxTimeWithWeeds = 40;
        
        private bool AnyWeeded => PlotControllers.Any(plot => plot.IsWeeded);
        
        private System.Random Random;

        private float WithWeedsNextSpawn;
        private float TimeSinceLastSpawn;

        public void Start()
        {
            ServiceLocator.RegisterAsService(this);
            foreach (var plot in PlotsList)
            {
                PlotControllers.Add(new PlotController(plot));
            }
            Random = new System.Random();
            StartCoroutine(WaitForNextWeed());
        }

        private float GetNextTime()
        {
            return Random.Next((int)(MinTimeWithWeeds * 1000), (int)(MaxTimeWithWeeds * 1000)) / 1000f;
        }

        private IEnumerator WaitForNextWeed()
        {
            WithWeedsNextSpawn = GetNextTime();
            TimeSinceLastSpawn = 0; 
            while (TimeSinceLastSpawn < WithWeedsNextSpawn * (AnyWeeded ? 1 : NoWeedsModifier))
            {
                yield return WaitABit;
                TimeSinceLastSpawn += 0.1f;
            }
            TryPlaceNewWeed();
        }

        private void TryPlaceNewWeed()  
        {
            PlotController plot = PlotControllers[Random.Next(PlotControllers.Count)];
            while (plot.IsWeeded)
            {
                plot = PlotControllers[Random.Next(PlotControllers.Count)];
            }
            plot.PlantWeed(new WeedData());
            //StartCoroutine(WaitForNextWeed());
        }
    }
}