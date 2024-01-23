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

        public Texture2D ChromaCopy(Texture2D canvas, Texture2D reference, Color[] keys, Color[] values)
        {
            if (keys.Length != values.Length)
                throw new ArgumentException("There needs to be a chroma key for every value and vice versa!");
            
            if (canvas.width != reference.width || canvas.height != reference.height)
                throw new ArgumentException($"The reference and canvas are not the same size! can:{canvas.width}x{canvas.height} vs ref:{reference.width}X{reference.height}");
            
            if(reference == null)
                return canvas;
            
            for (int i = 0; i < canvas.width; i++)
            {
                for (int j = 0; j < canvas.height; j++)
                { 
                    Color pixel = reference.GetPixel(i, j);

                    var index = Array.IndexOf(keys, pixel);
                    if (index != -1)
                    {
                        canvas.SetPixel(i, j, values[index]);
                    }
                    else canvas.SetPixel(i, j, pixel);
                }
            }

            canvas.Apply();
            return canvas;
        }
    }
}