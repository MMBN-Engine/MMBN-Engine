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
        public static List<Color> getPalette(this Texture2D texture)
        {
            Color[] colorMap = new Color[texture.Width * texture.Height];
            texture.GetData(colorMap);
            
            //Sets up list of colors in sprite
            List<Color> palette = new List<Color>();
            for (int i = 0; i < colorMap.Length; i++)
                if (!palette.Contains(colorMap[i]))
                    palette.Add(colorMap[i]);

            return palette;
        }

        public static Texture2D changeColor(this Texture2D map, List<Color> palette, List<Color> palette2)
        {
            if (map == null) return null;

            Texture2D output = new Texture2D(map.GraphicsDevice, map.Width, map.Height);

            Color[] colorMap = new Color[map.Width * map.Height];
            map.GetData(colorMap);

            for (int i = 0; i < colorMap.Length; i++)
            {
                int index;
                index = palette.FindIndex(color => color == colorMap[i]);
                if(index > -1) colorMap[i] = palette2[index];
            }

            output.SetData(colorMap);
            return output;
        }

        public static void saveTexture(this Texture2D texture, string filename)
        {
            List<Color> palette = texture.getPalette();

            Texture2D output = new Texture2D(texture.GraphicsDevice, 1, palette.Count());
            output.SetData(palette.ToArray());

            Stream stream = File.OpenWrite(filename + ".png");
            output.SaveAsPng(stream, output.Width, output.Height);
        }

        public static void saveImage(this Texture2D texture, string filename)
        {
            Stream stream = File.OpenWrite(filename + ".png");
            texture.SaveAsPng(stream, texture.Width, texture.Height);
        }

        public static void mergePalette(this Texture2D texture, List<Color> palette2, string filename)
        {
            List<Color> palette = texture.getPalette();
            palette = palette.Union(palette2).ToList();

            Texture2D output = new Texture2D(texture.GraphicsDevice, 1, palette.Count());
            output.SetData(palette.ToArray());

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

        public static Vector2 Mod(this Vector2 vector2, Vector2 add, Vector2 mod)
        {
            float x = (vector2.X + add.X) % mod.X;
            float y = (vector2.Y + add.Y) % mod.Y;

            if (x < 0) x += mod.X;
            if (y < 0) y += mod.Y;

            return new Vector2(x, y);
        }
    }
}
