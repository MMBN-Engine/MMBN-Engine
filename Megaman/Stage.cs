using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman
{
    class Stage
    {
        public string[,] PanelType;
        public string[,] Area;
        public int spriteWidth;
        public int[] spriteHeight;
         
        private int width;
        private int height;

        public Actor[,] actorArray;

        public Texture2D texture;

        public Stage() 
        {
            width = 6;
            height = 3;

            spriteWidth = 40;
            spriteHeight = new int[height];
            spriteHeight[0] = 24;
            spriteHeight[1] = 23;
            spriteHeight[2] = 33;

     
            PanelType = new string[width,height];
            Area = new string[width, height];
            actorArray = new Actor[width, height];

            for (int i = 0;i < width;++i)
                for(int j = 0; j <height;j++)
                    PanelType[i,j] = "null";
            
            for (int i = 0;i < width;++i)
                for(int j = 0; j <height;j++)
                    Area[i,j] = "blue";

            for (int i = 0; i < width/2; ++i)
                for (int j = 0; j < height; j++)
                    Area[i, j] = "red";
            
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; j++)
                    actorArray[i, j] = null;

        }

        public void Initialize(ContentManager content)
        {
            texture = content.Load<Texture2D>("sprites/tiles");
        }

        public int[, ,] stageDraw()
        {            //sets up numeric values for every panel types
            int[,,] numCode = new int[width, height, 2];
            
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; j++)
                    if (Area[i, j] == "red")
                        numCode[i, j, 0] = 0;
                    else
                        numCode[i, j, 0] = 1;

            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; j++)
                    if (PanelType[i, j] == "null")
                        numCode[i, j, 1] = 2;
                    else if (PanelType[i, j] == "Cracked")
                        numCode[i, j, 1] = 3;
                    else if (PanelType[i, j] == "Broken")
                        numCode[i, j, 1] = 4;
                    else if (PanelType[i, j] == "Grass")
                        numCode[i, j, 1] = 5;
                    else if (PanelType[i, j] == "Sand")
                        numCode[i, j, 1] = 6;
                    else if (PanelType[i, j] == "Metal")
                        numCode[i, j, 1] = 7;
                    else if (PanelType[i, j] == "Ice")
                        numCode[i, j, 1] = 8;
                    else if (PanelType[i, j] == "Swamp")
                        numCode[i, j, 1] = 9;
                    else if (PanelType[i, j] == "Lava")
                        numCode[i, j, 1] = 10;
                    else if (PanelType[i, j] == "Holy")
                        numCode[i, j, 1] = 11;
                    else if (PanelType[i, j] == "Hole")
                        numCode[i, j, 1] = 12;

            return numCode;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void setStage(string value)
        {
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; j++)
                    PanelType[i, j] = value;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int[, ,] tileDraw = stageDraw();
            int stagePositionX = 0;
            int stagePositionY = 72;
            int stageY;

            for (int k = 0; k < tileDraw.GetLength(2); k++)
                for (int i = 0; i < tileDraw.GetLength(0); i++)
                {
                    stageY = 0;
                    for (int j = 0; j < tileDraw.GetLength(1); j++)
                    {
                        Rectangle panel = new Rectangle(
                            tileDraw[i, j, k] * spriteWidth,
                            stageY,
                            spriteWidth, spriteHeight[j]);

                        spriteBatch.Draw(texture,
                            new Vector2(stagePositionX + i * spriteWidth, stagePositionY + stageY),
                            panel, Color.White);

                        stageY += spriteHeight[j];
                    }
                }
        }
    }
}
