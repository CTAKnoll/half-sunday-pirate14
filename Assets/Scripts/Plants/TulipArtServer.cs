using System;
using System.Collections.Generic;
using Services;
using UnityEngine;
using static Plants.TulipData;

namespace Plants
{
    public class TulipArtServer : MonoBehaviour, IService
    {
        [Serializable]
        public struct TulipArtBlob
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

            public Color Key1;
            public Color Key2;
            public Color Key3;
            public Color Key4;

            public Sprite Serve(TulipStage stage)
            {
                switch (stage)
                {
                    case TulipStage.Bulb : return Bulb;
                    case TulipStage.Planted : return Planted;
                    case TulipStage.Sprout : return Sprout;
                    case TulipStage.Shoot : return Shoot;
                    case TulipStage.Bud : return Bud;
                    case TulipStage.Bloom : return Bloom;
                    case TulipStage.FullBloom : return Fullbloom;
                    case TulipStage.OverBloom : return Overbloom;
                    case TulipStage.Choking : return Choking;
                    case TulipStage.Dead : return Dead;
                    case TulipStage.Picked : return Picked;
                }
                return null;
            }

            public Color[] GetKeyValues()
            {
                return new Color[4] { Key1, Key2, Key3, Key4 };
            }
            
        }

        public TulipArtBlob Plain;
        public TulipArtBlob Fancy;
        public TulipArtBlob Spotted;
        public TulipArtBlob Striped;

        public Dictionary<TulipVarietal, TulipArtBlob> Cache;

        public Color ChromaKey1;
        public Color ChromaKey2;
        public Color ChromaKey3;
        public Color ChromaKey4;


        protected void Awake()
        {
            Cache = new();
            ServiceLocator.RegisterAsService(this);
            
        }

        public Sprite GetBaseSprite(TulipVarietal varietal, TulipStage stage)
        {
            if (Cache.ContainsKey(varietal))
                return Cache[varietal].Serve(stage);
            
            TulipArtBlob blob = Plain;
            switch (varietal.Kind)
            {
                case TulipKind.Fancy: blob = Fancy;
                    break;
                case TulipKind.Spotted: blob = Spotted;
                    break;
                case TulipKind.Striped: blob = Striped;
                    break;
            }

            return ServiceLocator.GetService<ChromaKeyer>().ChromaKey(blob.Serve(stage), GetKeyValues(), blob.GetKeyValues());
        }
        
        public Color[] GetKeyValues()
        {
            return new Color[4] { ChromaKey1, ChromaKey2, ChromaKey3, ChromaKey4 };
        }
    }
}