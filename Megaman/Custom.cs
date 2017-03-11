using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Megaman.Chips;
using Megaman.Actors.Navis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CustomExtensions;

namespace Megaman
{
    class Custom
    {
        private int speed;
        private int speed_mod; 
        private Vector2 position;

        private Vector2 cursorPosition;

        public bool open;
        public bool closed;
        
        public float custom;
        public float customMax;
        public float customRate;
        private Vector2 barPosition;

        int HP, maxHP;
        SpriteFont font, fontBase, fontRed, fontGreen, fontGray;

        private Texture2D screen, hpDisplay, bar, color;
        public Texture2D nullElem, heatElem, elecElem, aquaElem, woodElem; 
        private Animation full, cursor, cursorOK, cursorAdd;

        private Navi navi;

        internal Chip[,] chipArray;

        public int chipSelected;

        public Custom() 
        {
            open = true;
            closed = false;

            position.X = 0;
            position.Y = 0;
            speed = 0;
            speed_mod = 15;

            custom = 0f;
            customRate = 8;
            customMax = 128f;

            barPosition.X = 12;
            barPosition.Y = 9;

            chipSelected = 0;  //The chip we are going to draw

            full = new Animation();
            cursor = new Animation();
            cursorOK = new Animation();
            cursorAdd = new Animation();
         }

        public void Initialize(ContentManager content,  Navi navi)
        {
            screen = content.Load<Texture2D>("sprites/custom/custom-screen");
            hpDisplay = content.Load<Texture2D>("sprites/custom/hp-screen");
            bar = content.Load<Texture2D>("sprites/custom/custom-bar");
            color = content.Load<Texture2D>("sprites/custom/bar-color");

            nullElem = content.Load<Texture2D>("sprites/custom/nullElem");
            aquaElem = content.Load<Texture2D>("sprites/custom/aquaElem");
            elecElem = content.Load<Texture2D>("sprites/custom/elecElem");
            heatElem = content.Load<Texture2D>("sprites/custom/heatElem");
            woodElem = content.Load<Texture2D>("sprites/custom/woodElem");

            fontBase = content.Load<SpriteFont>("navi-hp");
            fontRed = content.Load<SpriteFont>("navi-hp-red");
            fontGreen = content.Load<SpriteFont>("navi-hp-green");
            fontGray = content.Load<SpriteFont>("font-gray");
            
            full.Initialize(content.Load<Texture2D>("sprites/custom/custom-full"),
                new Vector2(0, 0), 147, 180, true);
            cursor.Initialize(content.Load<Texture2D>("sprites/custom/cursor"),
                new Vector2(2, 2), 18, 40, true);
            cursorOK.Initialize(content.Load<Texture2D>("sprites/custom/cursorOK"),
                new Vector2(2, -8), 26, 40, true);
            cursorAdd.Initialize(content.Load<Texture2D>("sprites/custom/cursorAdd"),
                new Vector2(2, -7), 25, 40, true);

            this.navi = navi;

            resetChipArray();
        }

        public void Update(GameTime gameTime)
        {
            position.X += speed;

            if (position.X < -screen.Width)
            {
                position.X = -screen.Width;
                closed = true;
                speed = 0;
            }

            if (position.X > 0)
            {
                position.X = 0;
                open = true;
                speed = 0;
            }

            if (custom <= customMax && closed==true)
                custom += ((float)gameTime.ElapsedGameTime.TotalSeconds/customRate)*customMax;
            if (custom > customMax)
                custom = customMax;

            if (custom == customMax)
                full.Update(gameTime);

            if (open)
            {
                cursor.Update(gameTime);
                cursorOK.Update(gameTime);
                cursorAdd.Update(gameTime);
            }
        }

        public void Close()
        {
            speed = -speed_mod;
            open = false;
        }

        public void Open()
        {
            speed = speed_mod;
            closed = false;
            custom = 0;
            full.Reset();
            cursor.Reset();
            cursorAdd.Reset();
            cursorOK.Reset();
            chipSelected = 0;

            resetChipArray();
        }

        //Eventually will want to rewrite this so you can't select empty chips
        public void moveCursor(Vector2 move)
        {
            cursorPosition = cursorPosition.Mod(move, new Vector2(6, 2));
            //cursorPosition += move;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(screen, position, Color.White);
            if (open == true)
            {

                Chip chip = chipArray[(int)cursorPosition.X, (int)cursorPosition.Y];

                if (chip != null)
                {
                    spriteBatch.Draw(chip.image, new Vector2(8, 24), Color.White);

                    Texture2D element = (Texture2D)typeof(Custom).GetField("nullElem").GetValue(this);
                    spriteBatch.Draw(element, new Vector2(25, 81), Color.White);

                    if (chip.attack != 0)
                    {
                        int length = (int)fontGray.MeasureString(chip.attack.ToString()).X;
                        spriteBatch.DrawString(fontGray, chip.attack.ToString(),
                            new Vector2(71 - length, 83), Color.White);
                    }
                }

                for (int i = 0; i < navi.Custom; i++)
                {
                    if (i < 5) spriteBatch.Draw(navi.customFolder[i].icon, new Vector2(9 + 16 * i, 105), Color.White);
                    else spriteBatch.Draw(navi.customFolder[i].icon, new Vector2(9 + 16 * (i - 5), 129), Color.White);
                }

                Vector2 cursorDraw = new Vector2(9 + 16 * cursorPosition.X, 105 + 24 * cursorPosition.Y);
                if (cursorPosition == new Vector2(5, 0))
                    cursorOK.Draw(spriteBatch, new Vector2(9 + 16 * cursorPosition.X, 105 + 24 * cursorPosition.Y));
                else if (cursorPosition == new Vector2(5, 1))
                    cursorAdd.Draw(spriteBatch, new Vector2(9 + 16 * cursorPosition.X, 105 + 24 * cursorPosition.Y));
                else cursor.Draw(spriteBatch, new Vector2(9 + 16 * cursorPosition.X, 105 + 24 * cursorPosition.Y));
            }
        }

        //This needs to be separated from the custom window
        //Particle effects are in from of this, but behind custom window
        public void drawBars(SpriteBatch spriteBatch)
        {
            //This updates before the custom bar, so we need this stament to prevent "jumping"
            int X = (int)position.X + screen.Width;
            if (X > screen.Width)
                X = screen.Width;

            spriteBatch.Draw(hpDisplay, new Vector2(X, position.Y), Color.White);
            HP = navi.HP;

            //Draw the actual HP
            if (HP <= maxHP / 4) font = fontRed;
            else font = fontBase;

            Vector2 hpOffset = new Vector2(40, 3);
            int justify = (int)font.MeasureString(HP.ToString()).X;
            spriteBatch.DrawString(font, HP.ToString(), new Vector2(X - justify, position.Y) + hpOffset, Color.White);

            //Only draw if custom window is closed
            if (closed == true)
            {
                spriteBatch.Draw(bar, new Vector2(X + hpDisplay.Width, position.Y), Color.White);
                spriteBatch.Draw(color, new Rectangle((int)(X + hpDisplay.Width + barPosition.X),
                    (int)barPosition.Y, (int)custom, color.Height), Color.White);
                if (custom == customMax)
                    full.Draw(spriteBatch, new Vector2(X + hpDisplay.Width, position.Y));
            }
        }

        public void Select()
        {
            if (cursorPosition == new Vector2(5, 0)) Close();
        }

        //Draws the top chips off the deck
        internal void resetChipArray()
        {
            //we make this a 6 x 2 array to account for the okay and add buttons
            //the last column will never be assigned
            chipArray = new Chip[6, 2];
            for (int i = 0; i < Math.Min(navi.Custom, navi.customFolder.Count()); i++)
            {
                if (i < 5) chipArray[i, 0] = navi.customFolder[i];
                else chipArray[i - 5, 0] = navi.customFolder[i];
            }
        }
    }
}
