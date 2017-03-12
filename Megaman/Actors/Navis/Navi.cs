using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Megaman.Chips;

namespace Megaman.Actors.Navis
{
    class Navi : Actor
    {
        Animation charge;
        Animation chargeFull;
        public Chip[] chipFolder;
        public List<Chip> customFolder;

        public int Custom;

        public int Attack, Rapid, Charge;

        public bool isCharging, charged;
        public int chargeTime, chargeElapsed;

        internal Vector2 armLocation;

        public Animation swordSprite;
        internal Vector2 swordLocation;

        public Navi(AttackList attackTypes, int HP) : base(attackTypes)
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

            chipFolder = new Chip[30];
            customFolder = new List<Chip>();

            charge = new Animation();
            chargeFull = new Animation();

            busterSprite = new Animation();
            gunSprite = new Animation();
            swordSprite = new Animation();

            attackFrame = new List<int>() { 0, 4, 0, 0 };
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            charge.Initialize(content.Load<Texture2D>("sprites/navi/charge"), new Vector2(10, 55), 64, 25, true);

            //fix this sprite later, seems to be a little bit jerky, not a priority now
            chargeFull.Initialize(content.Load<Texture2D>("sprites/navi/charge-full"), new Vector2(10, 55), 64, 25, true);
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

        public virtual void styleChange(String Element, String Style) { }
        public virtual void chargedAttack() { }
    }
}