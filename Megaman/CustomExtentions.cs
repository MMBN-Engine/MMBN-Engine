using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
namespace CustomExtensions
{
    public static class CustomExtentions
    {
        public static List<Color> getPallete(this Texture2D texture)
        {
            Color[] colorMap = new Color[texture.Width * texture.Height];
            texture.GetData(colorMap);
            
            //Sets up list of colors in sprite
            List<Color> pallete = new List<Color>();
            for (int i = 0; i < colorMap.Length; i++)
                if (!pallete.Contains(colorMap[i]))
                    pallete.Add(colorMap[i]);

            return pallete;
        }

        public static Texture2D changeColor(this Texture2D map, List<Color> pallete, List<Color> pallete2)
        {
            Texture2D output = new Texture2D(map.GraphicsDevice, map.Width, map.Height);

            Color[] colorMap = new Color[map.Width * map.Height];
            map.GetData(colorMap);

            for (int i = 0; i < colorMap.Length; i++)
            {
                int index;
                index = pallete.FindIndex(color => color == colorMap[i]);
                if(index > -1) colorMap[i] = pallete2[index];
            }

            output.SetData(colorMap);
            return output;
        }

        public static void saveTexture(this Texture2D texture, string filename)
        {
            List<Color> pallete = texture.getPallete();

            Texture2D output = new Texture2D(texture.GraphicsDevice, 1, pallete.Count());
            output.SetData(pallete.ToArray());

            Stream stream = File.OpenWrite(filename + ".png");
            output.SaveAsPng(stream, output.Width, output.Height);
        }

        static readonly Random Random = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
