using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Megaman.Chips;
using Megaman.Overworld;

namespace Megaman.Actors.Navis
{
    public class Navi : Actor
    {
        Animation charge;
        Animation chargeFull;
        public Chip[] chipFolder;
        public List<Chip> customFolder;

        Area area;

        public Animation standingSprite;
        // 0 - back
        // 1 - back right
        // 2 - right
        // 3 - front right
        // 4 - front
        public Vector2 facing; //Direction the navi is facing

        //Definitions for the loading the sprites
        internal Vector2 origin;
        internal int spriteWidth;
        internal string folder;

        public int Custom;

        public int Attack, Rapid, Charge;

        public bool isCharging, charged;
        public int chargeTime, chargeElapsed;

        internal Vector2 armLocation;

        public Animation swordSprite;
        internal Vector2 swordLocation;

        public Navi(int HP, Area area) : base()
        {
            color = "red";
            this.HP = HP;
            this.MaxHP = HP;

            chargeTime = 4000;
            chargeElapsed = 0;

            Attack = 1;
            Rapid = 1;
            Charge = 1;

            Custom = 5;

            this.area = area;

            chipFolder = new Chip[30];
            customFolder = new List<Chip>();

            charge = new Animation();
            chargeFull = new Animation();

            busterSprite = new Animation();
            gunSprite = new Animation();
            swordSprite = new Animation();

            standingSprite = new Animation();
            standingSprite.active = false;  //This is not an animation, we can just use the frame commands to grab frames easier

            facing = new Vector2(0, 1);

            attackFrame = new List<int>() { 0, 4, 0, 0 };
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            gfxFolder = "gfx/navi/" + name + "/";

            base.Initialize(content, position, stage);

            charge.Initialize(content.Load<Texture2D>("sprites/navi/charge"), new Vector2(10, 55), 64, 25, true);

            //fix this sprite later, seems to be a little bit jerky, not a priority now
            chargeFull.Initialize(content.Load<Texture2D>("sprites/navi/charge-full"), new Vector2(10, 55), 64, 25, true);

            loadBattleSprites(content);

            }

        private void loadBattleSprites(ContentManager content)
        {
            staticSprite.Initialize(content.Load<Texture2D>(folder + "navi"),
                origin, spriteWidth, 0, true);

            moveSprite.Initialize(content.Load<Texture2D>(folder + "move"),
                origin, spriteWidth, 30, false);

            attackSprites[0].Initialize(content.Load<Texture2D>(folder + "shoot"),
                origin, spriteWidth, 30, false);
            attackSprites[1].Initialize(content.Load<Texture2D>(folder + "sword"),
                origin, spriteWidth, 30, false);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            chargeElapsed += Charge * (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (!isCharging)
            {
                charge.Reset();
                chargeElapsed = 0;
            }

            if (!charged) chargeFull.Reset();

            if (isCharging)
            {
                charge.Update(gameTime);
                if (chargeElapsed >= chargeTime)
                {
                    charged = true;
                    isCharging = false;
                }
            }

            if (isShooting)
            {
                gunSprite.Update(gameTime);
                if (!gunSprite.active)
                {
                    info.sound?.Play();
                    isShooting = false;
                    attackHandle(info);
                    attackHandle = null;
                }
            }

            if (charged) chargeFull.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, float resolution)
        {
            base.Draw(spriteBatch, resolution);

            if (isShooting) gunSprite.Draw(spriteBatch, location + armLocation, resolution);

            if (isCharging) charge.Draw(spriteBatch, location, resolution);
            if (charged) chargeFull.Draw(spriteBatch, location, resolution);
        }

        public void Buster()
        {
            attackTypes.MegaBuster(this, Attack);
        }

        public virtual void styleChange(String Element, String Style)
        {
            foreach (KeyValuePair<string, DamageType> entry in Game.damageTypes)
            {
                if (!entry.Value.defaultDef) Body[entry.Key] = false;
            }

            Body[Element] = true;

            //staticSprite.map = staticSprite.map.changeColor(palette1, palette2);
            //moveSprite.map = moveSprite.map.changeColor(palette1, palette2);
            //busterSprite.map = busterSprite.map.changeColor(palette1, palette2);
            //foreach (Animation foo in attackSprites) foo.map = foo.map.changeColor(palette1, palette2);

            //standingSprite.map = standingSprite.map.changeColor(palette1, palette2);

            //palette1 = palette2;
        }

        public virtual void overWorldDraw(SpriteBatch spriteBatch, float resolution)
        {
            standingSprite.currentFrame = getStandingFrame();
            standingSprite.Draw(spriteBatch, new Vector2(120, 80), resolution);
        }

        public virtual void overWorldMove(Vector2 move)
        {
            facing = move;
            area.areaMove(move);
        }

        //Select appropriate frame for drawing
        public int getStandingFrame()
        {
            if (facing.X == 0)
            {
                if (facing.Y == -1) return 0;
                else return 4;
            }
            else
            {
                if (facing.X == -1) standingSprite.flip = true;
                if (facing.X == 1) standingSprite.flip = false;
                if (facing.Y == -1) return 1;
                if (facing.Y == 1) return 3;
                else return 2;
            }
        }

        public virtual void chargedAttack() { }
    }
}