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

        public Animation busterSprite;
        public Animation gunSprite;
        public bool isShooting;
        internal Vector2 armLocation;

        public Animation swordSprite;
        public bool isSlashing;
        internal Vector2 swordLocation;

        public attackMethod attackHandle;

        internal int damage;
        internal string damageType;
        internal List<string> effects;
        internal List<Animation> explosionSprites;

        public Navi(int HP)
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
            explosionSprites = new List<Animation>();
            swordSprite = new Animation();
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            charge.Initialize(content.Load<Texture2D>("sprites/navi/charge"), new Vector2(10, 55), 64, 25, true);

            //fix this sprite late, seems to be a little bit jerky, not a priority now
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
                    attackHandle(damage, damageType, effects, explosionSprites);
                }
            }

            if (isSlashing)
            {
                //swordSprite.Update(gameTime)
                if (attackSprites[1].currentFrame == 4) attackHandle(damage, damageType, effects, explosionSprites);
                if (!attackSprites[1].active) isSlashing = false;                                                    
            }

            if (charged) chargeFull.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (isShooting) gunSprite.Draw(spriteBatch, location + armLocation);

            if (isCharging) charge.Draw(spriteBatch, location);
            if (charged) chargeFull.Draw(spriteBatch, location);
        }

        public void Shoot(Animation animation, int damage, string damageType, List<string> effects, attackMethod attackHandle,
            List<Animation> sprites)
        {
            if (!canAttack()) return;
            gunSprite = animation;
            busterSprite.active = true;
            doAttack(0);
            isShooting = true;

            this.attackHandle = attackHandle;
            this.damage = damage;
            this.damageType = damageType;
            this.effects = effects;
            explosionSprites = sprites;
        }

        public void Slash(Animation animation, int damage, string damageType, List<string> effects, attackMethod attackHandle,
            List<Animation> sprites)
        {
            if (!canAttack()) return;
            //swordSprite.active = true;
            //swordSprite = animation;
            doAttack(1);
            isSlashing = true;

            this.attackHandle = attackHandle;
            this.damage = damage;
            this.damageType = damageType;
            this.effects = effects;
            explosionSprites = sprites;
        }

        public void Buster()
        {
            Shoot(busterSprite, Attack, "null", new List<string>(), Gun, null);
        }
    }
}