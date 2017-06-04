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
        public Animation square1, square2, square3, square4;
        public Animation corner1, corner2;
        public Animation walkse1, walksw1, walkne1, walknw1;
        public Animation walkup1, walkdown1;

        int tileWidth;
        int tileHeight;
        string[,] mapArray;  //For procedural generation of maps

        public Tileset(string tileset, Vector2 origin, int spriteWidth, int tileWidth, int tileHeight, ContentManager content)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;

            square1 = new Animation();
            square2 = new Animation();
            square3 = new Animation();
            square4 = new Animation();

            corner1 = new Animation();
            corner2 = new Animation();

            walkse1 = new Animation();
            walksw1 = new Animation();
            walkne1 = new Animation();
            walknw1 = new Animation();

            walkup1 = new Animation();
            walkdown1 = new Animation();

            String path = "maps/tilesets/" + tileset;

            square1.Initialize(content.Load<Texture2D>(path + "/square1"), new Vector2(0, 0), spriteWidth, 15, false);
            square2.Initialize(content.Load<Texture2D>(path + "/square2"), new Vector2(0, 0), spriteWidth, 15, false);
            square3.Initialize(content.Load<Texture2D>(path + "/square3"), new Vector2(0, 0), spriteWidth, 15, false);
            square4.Initialize(content.Load<Texture2D>(path + "/square4"), new Vector2(0, 0), spriteWidth, 15, false);

            corner1.Initialize(content.Load<Texture2D>(path + "/corner1"), new Vector2(0, 0), spriteWidth, 15, false);
            corner2.Initialize(content.Load<Texture2D>(path + "/corner2"), new Vector2(0, 0), spriteWidth, 15, false);

            walkse1.Initialize(content.Load<Texture2D>(path + "/walkse1"), new Vector2(0, 0), spriteWidth, 15, false);
            walksw1.Initialize(content.Load<Texture2D>(path + "/walksw1"), new Vector2(0, 0), spriteWidth, 15, false);
            walkne1.Initialize(content.Load<Texture2D>(path + "/walkne1"), new Vector2(0, 0), spriteWidth, 15, false);
            walknw1.Initialize(content.Load<Texture2D>(path + "/walknw1"), new Vector2(0, 0), spriteWidth, 15, false);

            walkup1.Initialize(content.Load<Texture2D>(path + "/walkup1"), new Vector2(0, 0), spriteWidth, 15, false);
            walkdown1.Initialize(content.Load<Texture2D>(path + "/walkdown1"), new Vector2(0, 0), spriteWidth, 15, false);
        }
    }
}