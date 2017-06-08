using System;
using System.Collections.Generic;
using System.Linq;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Megaman.Actors.Navis;
using Megaman.Chips;
using Megaman.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CustomExtensions;
using System.IO;

namespace Megaman.Overworld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Tileset
    {
        public Dictionary<string, Animation> tiles;

        public Func<string[,], Vector2, string> mapParse;

        int tileWidth;
        int tileHeight;

        public Tileset(string tileset, Vector2 origin, int spriteWidth, int tileWidth, 
            int tileHeight, Func<string[,], Vector2, string> mapParse)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;

            this.mapParse = mapParse;

            tiles = new Dictionary<string, Animation>();

            String path = Game.modulePath + "gfx/tilesets/" + tileset + "/";

            List<String> fileArray = Directory.GetFiles(path).Select(Path.GetFileNameWithoutExtension).ToList();

            foreach (string t in fileArray)
            {
                Animation tile = new Animation();
                tile.Initialize(Scripting.loadImage(path + t + ".png"), origin, spriteWidth, 15, false);

                tiles.Add(t, tile);
            }
        }
    }
}