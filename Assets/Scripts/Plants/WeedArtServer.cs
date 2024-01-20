using System.Collections.Generic;
using Services;
using UI.Plants;
using UnityEngine;
using static Plants.WeedData;

namespace Plants
{
    public class WeedArtServer : MonoBehaviour, IService
    {
        public Sprite Sprout;
        public Sprite Mature;
        public Sprite Spreading;

        public Sprite SpreadingUp;
        public Sprite SpreadingDown;
        public Sprite SpreadingLeft;
        public Sprite SpreadingRight;

        private Dictionary<WeedStage, Sprite> BaseImageServer;
        private Dictionary<PlotController.PlotSpreadDirection, Sprite> SpreadServer;

        protected void Start()
        {
            ServiceLocator.RegisterAsService(this);
            BaseImageServer = new Dictionary<WeedStage, Sprite>
            {
                [WeedStage.Sprout] = Sprout,
                [WeedStage.Mature] = Mature,
                [WeedStage.Spreading] = Spreading,
            };
            
            SpreadServer = new Dictionary<PlotController.PlotSpreadDirection, Sprite>
            {
                [PlotController.PlotSpreadDirection.Up] = SpreadingUp,
                [PlotController.PlotSpreadDirection.Down] = SpreadingDown,
                [PlotController.PlotSpreadDirection.Left] = SpreadingLeft,
                [PlotController.PlotSpreadDirection.Right] = SpreadingRight,
            };
        }

        public Sprite GetBaseSprite(WeedStage stage)
        {
            return BaseImageServer[stage];
        }
        
        public Sprite GetSpreadSprite(PlotController.PlotSpreadDirection direction)
        {
            return SpreadServer[direction];
        }
    }
}