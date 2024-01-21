using System;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core
{
    public class ProgressBar : MonoBehaviour
    {
        public SmartNumber Progress;
        public SmartNumber Storage;

        [SerializeField] private Image ProgressBarImage;
        
        private int FillPropertyID = Shader.PropertyToID("_Fill");
        private Timeline timeline;
        
        public void Start()
        {
            timeline = ServiceLocator.GetService<Timeline>();

            Progress = new SmartNumber(0f);
            Progress.AddTrigger(1f, Rollover);

            Storage = new SmartNumber(0f);
            
            timeline.AddRecurring(this, Tick, TimeSpan.FromDays(1));
        }

        public void Rollover()
        {
            Storage += 1f;
            Progress.SetQuietly(0f);
        }

        public void Tick()
        {
            Progress += 0.01f;
            Debug.Log(Progress.Value);
            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            ProgressBarImage.GetComponent<Image>().material.SetFloat(FillPropertyID, Progress.Value);
        }
    }
}