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
            Parse();
        }

        public string DeliverRumour()
        {
            return ParsedRumours[(int)FloatExtensions.RandomBetween(0, ParsedRumours.Length)];
        }

        public string DeliverMarquee(int start, int window)
        {
            return ParsedMarquee.Substring(start % ParsedMarquee.Length, window % ParsedMarquee.Length);
        }

        private void Parse()
        {
            ParsedRumours = Rumours.text.Split('\n');
            Debug.Log(ParsedRumours == null);
            var longSpace = new string(' ', 100);
            ParsedMarquee = MarqueeText.text.Replace("\n", longSpace);
            ParsedMarquee = ParsedMarquee + " " + ParsedMarquee;
        }
    }
}