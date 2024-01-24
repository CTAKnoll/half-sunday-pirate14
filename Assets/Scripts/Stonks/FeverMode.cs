using System;
using System.Collections;
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

        public float ReturnToValue = 0.2f;
        public float ReturnNumSeconds = 1.5f;

        [SerializeField] private Image ProgressBarImage;
        
        private int FillPropertyID = Shader.PropertyToID("_Fill");
        Material mat;
        private Timeline timeline;
        private IncidentsManager incManager;    

        public float DailyMaxNaturalIncrease = 0.005f;
        public float DailyMaxNaturalDecrease = -0.003f;
        
        public void Awake()
        {
            ServiceLocator.RegisterAsService(this);
            Awareness = new SmartNumber(0f);
            Awareness.AddTrigger(1f, () => StartCoroutine(Rollover()));
            FeverLevel = new SmartNumber(0f);
        }

        protected void Start()
        {
            timeline = ServiceLocator.LazyLoad<Timeline>();
            ServiceLocator.TryGetService(out incManager);

            timeline.AddRecurring(this, Tick, TimeSpan.FromDays(1));
            var img =  ProgressBarImage.GetComponent<Image>();
            mat = Instantiate(ProgressBarImage.GetComponent<Image>().material);
            mat.CopyPropertiesFromMaterial(ProgressBarImage.GetComponent<Image>().material);
            img.material = mat;

            incManager.Dialogue.AddFunction("fever_meter", () => { return Awareness.Value; });
        }

        private IEnumerator Rollover()
        {
            while (Awareness.Value > ReturnToValue)
            {
                Awareness.SetQuietly(Awareness.Value - 1/((1-ReturnToValue)*30*ReturnNumSeconds));
                UpdateProgressBar();
                yield return new WaitForSeconds(0.03f);
            }
            FeverLevel.Value += 1;
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