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

namespace Megaman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Area
    {
        Animation square1;
        int tileWidth;
        int tileHeight;
        string[,] mapArray;  //For procedural generation of maps

        Vector2 drawLocation;

        public Area()
        {
            square1 = new Animation();
            drawLocation = new Vector2();

            tileWidth = 64;
            tileHeight = 16;
        }

        public void loadTileset(string tileset, ContentManager content)
        {
            String path = "maps/tilesets/" + tileset;
            square1.Initialize(content.Load<Texture2D>(path + "/square1"), new Vector2(0, 0), tileWidth, 15, false);
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
                    mapArray[i, j] = csv[i];
                }
            }
        }
    }
}