using System.Text;
using Services;
using UnityEngine;

namespace Utils
{
    public class TextServer : MonoBehaviour, IService
    {
        public TextAsset Rumours;
        private string[] ParsedRumours;
        public TextAsset MarqueeText;
        private string ParsedMarquee;

        protected void Awake()
        {
            ServiceLocator.RegisterAsService(this);
        }

        protected void Start()
        {
            Parse();
        }

        public string DeliverRumour()
        {
            return ParsedRumours[(int)FloatExtensions.RandomBetween(0, ParsedRumours.Length)];
        }

        public string DeliverMarquee(int start, int window)
        {
            return ParsedMarquee.Substring(start, window);
        }

        private void Parse()
        {
            ParsedRumours = Rumours.text.Split('\n');
            var longSpace = new string(' ', 100);
            ParsedMarquee = MarqueeText.text.Replace("\n", longSpace);
        }
    }
}