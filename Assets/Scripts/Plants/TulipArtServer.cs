using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;
using static Plants.TulipData;

namespace Plants
{
    public class TulipArtServer : MonoBehaviour, IService
    {
        public Sprite Planted;
        public Sprite Sprout;
        public Sprite Shoot;
        public Sprite Bud;
        public Sprite Bloom;
        public Sprite Fullbloom;
        public Sprite Overbloom;
        public Sprite Choking;
        public Sprite Dead;
        
        public Sprite Bulb;
        public Sprite Picked;

        private Dictionary<TulipStage, Sprite> BaseImageServer;

        protected void Awake()
        {
            ServiceLocator.RegisterAsService(this);
            BaseImageServer = new Dictionary<TulipStage, Sprite>
            {
                [TulipStage.Bulb] = Bulb,
                [TulipStage.Planted] = Planted,
                [TulipStage.Sprout] = Sprout,
                [TulipStage.Shoot] = Shoot,
                [TulipStage.Bud] = Bud,
                [TulipStage.Bloom] = Bloom,
                [TulipStage.FullBloom] = Fullbloom,
                [TulipStage.OverBloom] = Overbloom,
                [TulipStage.Choking] = Choking,
                [TulipStage.Dead] = Dead,
                [TulipStage.Picked] = Picked,
            };
        }

        public Sprite GetBaseSprite(TulipStage stage)
        {
            return BaseImageServer[stage];
        }
    }
}