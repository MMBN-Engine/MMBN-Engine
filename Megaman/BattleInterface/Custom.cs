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
using Microsoft.Xna.Framework.Audio;
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

        internal bool playedMaxSound;

        SpriteFont font, fontBase, fontRed, fontGreen, fontGray, fontYellow;

        private Texture2D screen, hpDisplay, bar, color;
        public Texture2D NullElem, FireElem, elecElem, AquaElem, woodElem; 
        private Animation full, cursor, cursorOK, cursorAdd;

        private SoundEffect chipCancel, chipChoose, chipConfirm, chipSelect;
        private SoundEffect custBarFull, customScreenOpen;
        private SoundEffect redHPSound;
        private SoundEffectInstance redHPInstance;

        private Navi navi;

        internal Chip[,] chipArray;

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

            NullElem = content.Load<Texture2D>("sprites/custom/nullElem");
            AquaElem = content.Load<Texture2D>("sprites/custom/aquaElem");
            elecElem = content.Load<Texture2D>("sprites/custom/elecElem");
            FireElem = content.Load<Texture2D>("sprites/custom/fireElem");
            woodElem = content.Load<Texture2D>("sprites/custom/woodElem");

            loadFonts(content);
            
            chipCancel = content.Load<SoundEffect>("soundFX/custom/chipCancel");
            chipChoose = content.Load<SoundEffect>("soundFX/custom/chipChoose");
            chipConfirm = content.Load<SoundEffect>("soundFX/custom/chipConfirm");
            chipSelect = content.Load<SoundEffect>("soundFX/custom/chipSelect");

            custBarFull = content.Load<SoundEffect>("soundFX/custom/custBarFull");
            customScreenOpen = content.Load<SoundEffect>("soundFX/custom/customScreenOpen");

            redHPSound = content.Load<SoundEffect>("soundFX/custom/redHP");
            redHPInstance = redHPSound.CreateInstance();
            redHPInstance.IsLooped = true;
            
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

        public void loadFonts(ContentManager content)
        {
            fontBase = content.Load<SpriteFont>("navi-hp");
            fontRed = content.Load<SpriteFont>("navi-hp-red");
            fontGreen = content.Load<SpriteFont>("navi-hp-green");
            fontGray = content.Load<SpriteFont>("font-gray");
            fontYellow = content.Load<SpriteFont>("font-yellow");
        }

        public void Update(GameTime gameTime)
        {
            if (navi.HP <= navi.MaxHP / 4) redHPInstance.Play();

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

            for (int i = navi.customFolder.Count() - 1; i >= 0; i--)
                if (navi.customFolder[i].selected) navi.customFolder.RemoveAt(i);
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
            cursorPosition = new Vector2();
            playedMaxSound = false;

            customScreenOpen.Play();

            //Draw new chips and remove unused ones
            navi.chips = new List<Chip>();
            resetChipArray();
        }

        //Eventually will want to rewrite this so you can't select empty chips
        public void moveCursor(Vector2 move)
        {
            cursorPosition = cursorPosition.Mod(move, new Vector2(6, 2));
            chipSelect.Play();
            //cursorPosition += move;
        }
        
        public void Draw(SpriteBatch spriteBatch, float resolution)
        {
            spriteBatch.Draw(screen, position * resolution,
                scale: new Vector2(1, 1) * resolution, color: Color.White);
            if (open == true)
            {

                Chip chip = chipArray[(int)cursorPosition.X, (int)cursorPosition.Y];

                //Draw chip
                if (chip != null)
                {
                    spriteBatch.Draw(chip.image, new Vector2(8, 24) * resolution, 
                        scale: new Vector2(1,1)* resolution, color: Color.White);

                    Texture2D element = (Texture2D)typeof(Custom).GetField(chip.element + "Elem").GetValue(this);
                    spriteBatch.Draw(element, new Vector2(25, 81) * resolution, 
                        scale: new Vector2(1, 1) * resolution, color: Color.White);

                    spriteBatch.DrawString(fontYellow, chip.code, new Vector2(10, 83) * resolution,
                            scale: resolution, color: Color.White, rotation: 0, origin: new Vector2(), effects: SpriteEffects.None,
                            layerDepth: 0);

                    if (chip.attack != 0)
                    {
                        int length = (int)fontGray.MeasureString(chip.attack.ToString()).X;
                        spriteBatch.DrawString(fontGray, chip.attack.ToString(),
                            new Vector2(71 - length, 83) * resolution,
                            scale: resolution, color: Color.White, rotation: 0, origin: new Vector2(), effects: SpriteEffects.None,
                            layerDepth: 0);
                    }
                }

                //Draw chip icons and codes
                for (int i = 0; i < Math.Min(navi.Custom, navi.customFolder.Count()); i++)
                {
                    Color chipColor;

                    if (i < 5)
                    {
                        spriteBatch.DrawString(fontYellow, chipArray[i, 0].code, new Vector2(14 + 16 * i, 121) * resolution,
                                scale: resolution / 2, color: Color.White, rotation: 0, origin: new Vector2(), effects: SpriteEffects.None,
                                layerDepth: 0);

                        if (canSelect(chipArray[i, 0])) chipColor = Color.White;
                        else chipColor = Color.Gray;

                        if (!chipArray[i, 0].selected)
                        {                            
                            spriteBatch.Draw(chipArray[i, 0].icon,  new Vector2(9 + 16 * i, 105) * resolution,
                                scale: new Vector2(1,1)* resolution, color: chipColor);
                        }
                    }
                    else
                    {
                        spriteBatch.DrawString(fontYellow, chipArray[i - 5, 1].code, new Vector2(14 + 16 * (i - 5), 145) * resolution,
                                scale: resolution / 2, color: Color.White, rotation: 0, origin: new Vector2(), effects: SpriteEffects.None,
                                layerDepth: 0);

                        if (canSelect(chipArray[i, 0])) chipColor = Color.White;
                        else chipColor = Color.Gray;

                        if (!chipArray[i - 5, 1].selected)
                        {
                            spriteBatch.Draw(chipArray[i - 5, 1].icon, new Vector2(9 + 16 * (i - 5), 129) * resolution,
                                scale: new Vector2(1,1) * resolution, color: chipColor);
                        }
                    }
                }

                //Draw selected chips
                for (int i = 0; i < navi.chips.Count(); i++)
                    spriteBatch.Draw(navi.chips[i].icon, new Vector2(97, 25 + 16 * i) * resolution, 
                        scale: new Vector2(1, 1) * resolution, color: Color.White);

                //Draw cursor
                Vector2 cursorDraw = new Vector2(9 + 16 * cursorPosition.X, 105 + 24 * cursorPosition.Y);
                if (cursorPosition == new Vector2(5, 0))
                    cursorOK.Draw(spriteBatch, new Vector2(9 + 16 * cursorPosition.X, 105 + 24 * cursorPosition.Y), resolution);
                else if (cursorPosition == new Vector2(5, 1))
                    cursorAdd.Draw(spriteBatch, new Vector2(9 + 16 * cursorPosition.X, 105 + 24 * cursorPosition.Y), resolution);
                else cursor.Draw(spriteBatch, new Vector2(9 + 16 * cursorPosition.X, 105 + 24 * cursorPosition.Y), resolution);
            }
        }

        //This needs to be separated from the custom window
        //Particle effects are in from of this, but behind custom window
        public void drawBars(SpriteBatch spriteBatch, float resolution)
        {
            //This updates before the custom bar, so we need this stament to prevent "jumping"
            int X = (int)position.X + screen.Width;
            if (X > screen.Width)
                X = screen.Width;

            spriteBatch.Draw(hpDisplay, new Vector2(X, position.Y) * resolution, 
                scale: new Vector2(1,1)*resolution, color: Color.White);

            //Draw the actual HP
            if (navi.HP <= navi.MaxHP / 4) font = fontRed;
            else font = fontBase;

            Vector2 hpOffset = new Vector2(40, 3);
            int justify = (int)font.MeasureString(navi.HP.ToString()).X;
            spriteBatch.DrawString(font, navi.HP.ToString(), (new Vector2(X - justify, position.Y) + hpOffset) * resolution,
                scale: resolution, color: Color.White, rotation: 0, origin: new Vector2(), effects: SpriteEffects.None,
                layerDepth: 0);

            //Only draw if custom window is closed
            if (closed == true)
            {
                spriteBatch.Draw(bar, new Vector2(X + hpDisplay.Width, position.Y) * resolution, 
                    color: Color.White, scale: new Vector2(1, 1) * resolution);
                
                //Draw the increasing custom bar
                spriteBatch.Draw(color, new Rectangle((int)((X + hpDisplay.Width + barPosition.X) * resolution),
                    (int)(barPosition.Y * resolution), (int)(custom * resolution), (int) (color.Height*resolution)),
                    Color.White);

                if (custom == customMax)
                {
                    full.Draw(spriteBatch, new Vector2(X + hpDisplay.Width, position.Y), resolution);
                    if (!playedMaxSound)
                    {
                        custBarFull.Play();
                        playedMaxSound = true;
                    }
                }
            }
        }

        public void Select()
        {
            Chip chip = chipArray[(int)cursorPosition.X, (int)cursorPosition.Y];

            if (chip != null)
            {
                if (cursorPosition.X < 5 && !chip.selected && canSelect(chip))
                {
                    navi.chips.Add(chip);
                    chip.selected = true;
                    chipChoose.Play();
                }
            }
            if (cursorPosition == new Vector2(5, 0))
            {
                Close();
                chipConfirm.Play();
            }
        }

        public void unSelect()
        {
            int count = navi.chips.Count();
            if (count > 0)
            {
                navi.chips[count - 1].selected = false;
                navi.chips.RemoveAt(count - 1);
                chipCancel.Play();
            }
        }

        //Check to see if we can use the chip
        //@ is equal to *, the wildcard code
        public bool canSelect(Chip chip)
        {
            int num = navi.chips.Count();

            if (num == 0) return true;

            List<string> codeList = new List<string>();
            List<string> nameList = new List<string>();

            foreach (Chip c in navi.chips)
            {
                codeList.Add(c.code);
                nameList.Add(c.name);
            }

            List<string> code = codeList.Where(o => o != "@").ToList();
            if (code.Count() == 0) return true; //All codes are *

            bool test = codeList.Any(o => o != code[0] || o != "@");

            if (!nameList.Any(o => o != nameList[0]) && chip.name == nameList[0]) return true;
            if (!code.Any(o => o != code[0]) && (chip.code == code[0] || chip.code == "@")) return true;
            else return false;
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
                else chipArray[i - 5, 1] = navi.customFolder[i];
            }
        }
    }
}
