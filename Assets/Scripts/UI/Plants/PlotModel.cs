using UnityEngine;

namespace UI.Plants
{
    public struct PlotModel : IUIModel
    {
        public string DebugText;
        public bool TulipShowing;
        public bool WeedShowing;
        public Sprite TulipImage;
        public Sprite WeedImage;

        public bool PickedIconVisible;
        public Sprite PickedIconImage;
        public Vector3 PickedIconPos;

        public bool WeedAlertUp;
        public Sprite WeedAlertUpImage;
        public bool WeedAlertDown;
        public Sprite WeedAlertDownImage;
        public bool WeedAlertLeft;
        public Sprite WeedAlertLeftImage;
        public bool WeedAlertRight;
        public Sprite WeedAlertRightImage;
    }
}