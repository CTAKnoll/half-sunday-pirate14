using System;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core
{
    public class FeverMode : MonoBehaviour, IService
    {
        public SmartNumber Awareness;
        public SmartNumber FeverLevel;

        [SerializeField] private Image ProgressBarImage;
        
        private int FillPropertyID = Shader.PropertyToID("_Fill");
        Material mat;
        private Timeline timeline;

        public float DailyMaxNaturalIncrease = 0.005f;
        public float DailyMaxNaturalDecrease = -0.003f;
        
        public void Awake()
        {
            ServiceLocator.RegisterAsService(this);
            timeline = ServiceLocator.GetService<Timeline>();

            Awareness = new SmartNumber(0f);
            Awareness.AddTrigger(1f, Rollover);

            FeverLevel = new SmartNumber(0f);
            
            timeline.AddRecurring(this, Tick, TimeSpan.FromDays(1));
            var img =  ProgressBarImage.GetComponent<Image>();
            mat = Instantiate(ProgressBarImage.GetComponent<Image>().material);
            mat.CopyPropertiesFromMaterial(ProgressBarImage.GetComponent<Image>().material);
            img.material = mat;
        }

        public void Rollover()
        {
            while (Awareness.Value >= 1)
            {
                FeverLevel += 1f;
                Awareness.SetQuietly(Awareness.Value - 1);
            }
        }

        public void Tick()
        {
            Awareness += FloatExtensions.RandomBetween(DailyMaxNaturalDecrease, DailyMaxNaturalIncrease);
            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            ProgressBarImage.GetComponent<Image>().material.SetFloat(FillPropertyID, Awareness.Value);
        }
    }
}