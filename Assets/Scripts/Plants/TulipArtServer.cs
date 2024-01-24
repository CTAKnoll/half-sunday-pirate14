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
        }
        
        [Serializable]
        public struct ColorBlob
        {
             public Color Key1;
             public Color Key2;
             public Color Key3;
             public Color Key4;
             
             public Color[] GetKeyValues()
             {
                 return new Color[4] { Key1, Key2, Key3, Key4 };
             }
        }
        

        public TulipArtBlob Plain;
        public TulipArtBlob Fancy;
        public TulipArtBlob Spotted;
        public TulipArtBlob Striped;

        public ColorBlob DefaultRed;
        public ColorBlob White;
        public ColorBlob Blue;
        public ColorBlob Pink;
        public ColorBlob Yellow;
        public ColorBlob Purple;
        public ColorBlob PurpleWhite;
        public ColorBlob Sunshine;
        public ColorBlob Tangerine;
        public ColorBlob Coral;
        

        public Dictionary<(TulipVarietal, TulipStage), Sprite> Cache;


        protected void Awake()
        {
            Cache = new();
            ServiceLocator.RegisterAsService(this);
            
        }

        public ColorBlob GetTulipColorData(TulipVarietal varietal)
        {
            ColorBlob color = DefaultRed;
            switch (varietal.Color)
            {
                case TulipColor.White: color = White;
                    break;
                case TulipColor.Blue: color = Blue;
                    break;
                case TulipColor.Yellow: color = Yellow;
                    break;
                case TulipColor.Pink: color = Pink;
                    break;
                case TulipColor.Purple: color = Purple;
                    break;
                case TulipColor.PurpleWhite: color = PurpleWhite;
                    break;
                case TulipColor.Sunshine: color = Sunshine;
                    break;
                case TulipColor.Tangerine: color = Tangerine;
                    break;
                case TulipColor.Coral: color = Coral;
                    break;
            }

            return color;
        }

        public TulipArtBlob GetTulipArtData(TulipVarietal varietal)
        {
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

            return blob;
        }

        public Sprite GetBaseSprite(TulipVarietal varietal, TulipStage stage)
        {
            if (Cache.ContainsKey((varietal, stage)))
                return Cache[(varietal, stage)];
            
            ColorBlob color = GetTulipColorData(varietal);
            TulipArtBlob blob = GetTulipArtData(varietal);

            Sprite image = blob.Serve(stage);
            if (image == null)
            {
                Debug.LogWarning("TulipArtServer :: Returned a null sprite! I hope the plain fallback works...");
                image = Plain.Serve(stage);
            }
            
            Texture2D newImage = ServiceLocator.LazyLoad<ChromaKeyer>().ChromaCopy(new Texture2D(image.texture.width, image.texture.height), 
                image.texture, DefaultRed.GetKeyValues(), color.GetKeyValues());
            Sprite newSprite = Sprite.Create(newImage, image.rect, image.pivot);
            Cache.Add((varietal, stage), newSprite);
            return newSprite;
        }
        
    }
}