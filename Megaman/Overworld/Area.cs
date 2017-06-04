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

        public Vector2 drawLocation;
        public Vector2 currentTile;

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

            drawLocation = new Vector2(120, 80); //Puts us in the center of the first tile
            currentTile = new Vector2();

            tileWidth = 32;
            tileHeight = 16;
        }

        public void loadTileset(Tileset tileset)
        {
            square1 = tileset.square1;
            square2 = tileset.square2;
            square3 = tileset.square3;
            square4 = tileset.square4;

            corner1 = tileset.corner1;
            corner2 = tileset.corner2;

            walkse1 = tileset.walkse1;
            walksw1 = tileset.walksw1;
            walkne1 = tileset.walkne1;
            walknw1 = tileset.walknw1;

            walkup1 = tileset.walkup1;
            walkdown1 = tileset.walkdown1;
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

        //Looks at tiles and seeds if we can move where we want to
        public void areaMove(Vector2 move)
        {
            Vector2 step = new Vector2(tileWidth / tileHeight * move.X, move.Y);
            step.Normalize();
            step = 2 * step;
            step = drawLocation - step;

            bool didMove = false;

            // Check if we will move outside bounding box
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2 nextTile = new Vector2(i, j);
                    if (onTile(currentTile + nextTile, step) && !didMove)
                    {
                        drawLocation = step;
                        didMove = true;
                        currentTile += nextTile;
                    }
                }
            }

            if (onTile(currentTile, step))  
            {
                drawLocation = step;
            }
            else if (onTile(currentTile + new Vector2(1,0), step))
            {
                drawLocation = step;
                currentTile.X += 1;
            }
            else if (onTile(currentTile - new Vector2(1, 0), step))
            {
                drawLocation = step;
                currentTile.X -= 1;
            }
            else if (onTile(currentTile + new Vector2(0, 1), step))
            {
                drawLocation = step;
                currentTile.Y += 1;
            }
            else if (onTile(currentTile - new Vector2(0, 1), step))
            {
                drawLocation = step;
                currentTile.Y -= 1;
            }
        }

        //Checks to see if there is a tile at (i,j)
        public bool tileExists(int i, int j)
        {
            if (i < 0 || j < 0 || i >= mapArray.GetLength(0) || j >= mapArray.GetLength(1))
                return false;
            string tileType = mapArray[i, j];
            if (tileType == "") return false;
            else return true;
        }

        //Checks to see if we are on a tile
        public bool onTile(Vector2 tile, Vector2 position)
        {
            Vector2 delta = position + tileLocation((int)tile.X + 1, (int)tile.Y) 
                - new Vector2(120, 80);
            bool isTile = tileExists((int)tile.X, (int)tile.Y);
            bool inTile = (Math.Abs(delta.X) / tileWidth + Math.Abs(delta.Y) / tileHeight <= 1);
            if (isTile && inTile) return true;
            else return false;
        }

        //Finds the position to draw the tile in
        public Vector2 tileLocation(int i, int j)
        {
            Vector2 position = new Vector2();
            position.X = (i - j - 1) * tileWidth;
            position.Y = (i + j - 1) * tileHeight;

            return position;
        }

        public void loadMap(string fileName)
        {
            string[] lines = File.ReadAllLines("Content/areas/maps/" + fileName);
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