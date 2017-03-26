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
    public partial class Area
    {
        Animation square1, square2, square3, square4;
        Animation corner1, corner2;
        Animation walkse1, walksw1, walkne1, walknw1;
        Animation walkup1, walkdown1;

        int tileWidth;
        int tileHeight;
        string[,] mapArray;  //For procedural generation of maps

        Vector2 drawLocation;

        public Area()
        {
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

            drawLocation = new Vector2();

            tileWidth = 64;
            tileHeight = 16;
        }

        public void loadTileset(string tileset, ContentManager content)
        {
            String path = "maps/tilesets/" + tileset;
            square1.Initialize(content.Load<Texture2D>(path + "/square1"), new Vector2(0, 0), tileWidth, 15, false);
            square2.Initialize(content.Load<Texture2D>(path + "/square2"), new Vector2(0, 0), tileWidth, 15, false);
            square3.Initialize(content.Load<Texture2D>(path + "/square3"), new Vector2(0, 0), tileWidth, 15, false);
            square4.Initialize(content.Load<Texture2D>(path + "/square4"), new Vector2(0, 0), tileWidth, 15, false);

            corner1.Initialize(content.Load<Texture2D>(path + "/corner1"), new Vector2(0, 0), tileWidth, 15, false);
            corner2.Initialize(content.Load<Texture2D>(path + "/corner2"), new Vector2(0, 0), tileWidth, 15, false);

            walkse1.Initialize(content.Load<Texture2D>(path + "/walkse1"), new Vector2(0, 0), tileWidth, 15, false);
            walksw1.Initialize(content.Load<Texture2D>(path + "/walksw1"), new Vector2(0, 0), tileWidth, 15, false);
            walkne1.Initialize(content.Load<Texture2D>(path + "/walkne1"), new Vector2(0, 0), tileWidth, 15, false);
            walknw1.Initialize(content.Load<Texture2D>(path + "/walknw1"), new Vector2(0, 0), tileWidth, 15, false);

            walkup1.Initialize(content.Load<Texture2D>(path + "/walkup1"), new Vector2(0, 0), tileWidth, 15, false);
            walkdown1.Initialize(content.Load<Texture2D>(path + "/walkdown1"), new Vector2(0, 0), tileWidth, 15, false);
        }

        //Inefficient, eventually will want to change so it only draws what we need
        public void Draw(SpriteBatch spriteBatch, float resolution)
        {
            for (int i = 0; i < mapArray.GetLength(0); i++)
            {
                for (int j = 0; j < mapArray.GetLength(1); j++)
                {
                    getTile(i, j)?.Draw(spriteBatch, drawLocation + tileLocation(i, j), resolution);
                }
            }
        }

        //Finds the tile to draw
        public Animation getTile(int i, int j)
        {
            string tileType = mapArray[i, j];

            if (tileType == "s1") return square1;
            if (tileType == "s2") return square2;
            if (tileType == "s3") return square3;
            if (tileType == "s4") return square4;

            if (tileType == "c1") return corner1;
            if (tileType == "c2") return corner2;

            if (tileType == "se1") return walkse1;
            if (tileType == "sw1") return walksw1;
            if (tileType == "ne1") return walkne1;
            if (tileType == "nw1") return walknw1;

            if (tileType == "u1") return walkup1;
            if (tileType == "d1") return walkdown1;

            else return null;
        }

        //Finds the position to draw the tile in
        public Vector2 tileLocation(int i, int j)
        {
            Vector2 position = new Vector2();
            position.X = (i - j) * tileWidth / 2;
            position.Y = (i + j) * tileHeight;

            return position;
        }

        public void loadMap(string fileName)
        {
            string[] lines = File.ReadAllLines("Content/maps/" + fileName);
            mapArray = new string[lines[0].Split(';').Count(), lines.Count()];

            for (int j = 0; j < mapArray.GetLength(1); j++)
            {
                string[] csv = lines[j].Split(';');
                for (int i = 0; i < mapArray.GetLength(0); i++)
                {
                    mapArray[i, j] = csv[i].Replace(" ","");
                }
            }
        }
    }
}