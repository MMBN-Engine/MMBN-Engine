using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Megaman.Actors.Navis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman
{
    class Custom
    {
        private int speed;
        private int speed_mod; 
        private Vector2 position;
        public bool open;
        public bool closed;
        
        public float custom;
        public float customMax;
        public float customRate;
        private Vector2 barPosition;

        int HP, maxHP;
        SpriteFont font, fontBase, fontRed, fontGreen, fontGray;

        private Texture2D screen, hpDisplay, bar, color;
        private Animation full;

        private Navi navi;

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

            chipSelected = 0;

            full = new Animation();
         }

        public void Initialize(ContentManager content,  Navi navi)
        {
            screen = content.Load<Texture2D>("sprites/custom/custom-screen");
            hpDisplay = content.Load<Texture2D>("sprites/custom/hp-screen");
            bar = content.Load<Texture2D>("sprites/custom/custom-bar");
            color = content.Load<Texture2D>("sprites/custom/bar-color");
            
            fontBase = content.Load<SpriteFont>("navi-hp");
            fontRed = content.Load<SpriteFont>("navi-hp-red");
            fontGreen = content.Load<SpriteFont>("navi-hp-green");
            fontGray = content.Load<SpriteFont>("font-gray");
            
            full.Initialize(content.Load<Texture2D>("sprites/custom/custom-full"),
                new Vector2(0, 0), 147, 180, true);

            this.navi = navi;
        }

        public void Update(GameTime gameTime)
        {            
            position.X += speed;
            
            if (custom <= customMax && closed==true)
                custom += ((float)gameTime.ElapsedGameTime.TotalSeconds/customRate)*customMax;
            if (custom > customMax)
                custom = customMax;

            if (custom == customMax)
                full.Update(gameTime);

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
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
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

            spriteBatch.Draw(screen, position, Color.White);
            if (open == true)
            {
                
                int Attack = navi.customFolder[chipSelected].attack;
                spriteBatch.Draw(navi.customFolder[chipSelected].image, new Vector2(8, 24), Color.White);
                
                if (!(navi.customFolder[chipSelected].attack == 0))
                {
                    int length = (int)fontGray.MeasureString(Attack.ToString()).X;
                    spriteBatch.DrawString(fontGray, Attack.ToString(), 
                        new Vector2(71 - length, 83), Color.White);
                }

                for (int i = 0; i < navi.Custom; i++)
                {
                    if (i < 5) spriteBatch.Draw(navi.customFolder[i].icon, new Vector2(9 + 16 * i, 105), Color.White);
                    else spriteBatch.Draw(navi.customFolder[i].icon, new Vector2(9 + 16 * (i - 5), 129), Color.White);
                }
            }
        }

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
    }
}
