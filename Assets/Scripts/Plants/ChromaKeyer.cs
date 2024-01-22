using System;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Plants
{
    public class ChromaKeyer : IService
    {
        public Sprite ChromaKey(Sprite image, Color[] keys, Color[] values)
        {
            if (keys.Length != values.Length)
                throw new ArgumentException("There needs to be a chroma key for every value and vice versa!");
            
            if(image == null)
                return null;
            
            Texture2D texture = image.texture;
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                { 
                    Color pixel = texture.GetPixel(i, j);

                    var index = Array.IndexOf(keys, pixel);
                    if (index != -1)
                    {
                        texture.SetPixel(i, j, values[index]);
                    }
                }
            }
            
            return image;
        }
    }
}