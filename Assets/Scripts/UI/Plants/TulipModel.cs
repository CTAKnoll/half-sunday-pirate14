using UnityEngine;

namespace UI.Plants
{
    [System.Serializable]
    public struct TulipModel : IUIModel
    {
        public Vector3 ScreenPos;
        public Sprite IconSprite;
        public Color Color;
    }
}