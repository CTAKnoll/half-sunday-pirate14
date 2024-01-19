using System.Collections.Generic;
using Services;
using UnityEngine;
using static Plants.WeedData;

namespace Plants
{
    public class WeedArtServer : MonoBehaviour, IService
    {
        public Sprite Sprout;
        public Sprite Mature;
        public Sprite Spreading;

        private Dictionary<WeedStage, Sprite> BaseImageServer;

        protected void Start()
        {
            ServiceLocator.RegisterAsService(this);
            BaseImageServer = new Dictionary<WeedStage, Sprite>
            {
                [WeedStage.Sprout] = Sprout,
                [WeedStage.Mature] = Mature,
                [WeedStage.Spreading] = Spreading,
            };
        }

        public Sprite GetBaseSprite(WeedStage stage)
        {
            return BaseImageServer[stage];
        }
    }
}