using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using static Megaman.AttackType;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//right now all movement code is for teleporting, will change it to allow sliding later

namespace Megaman.Projectiles
{
    class Shot : Projectile
    {
        public Vector2 speed;
        public int damage;
        public bool didAttack;  //Tracks whether or not we attacted on a given panel
        public List<string> damageType;
        public List<String> effects;

        public Shot(Actor actor) : base(actor)           
        {
            speed = new Vector2(1, 0) * (float)actor.info.speed * 46;
            damage = actor.info.damage;
            damageType = actor.info.damageType;
            effects = actor.info.effects;
        }

        public override void Update(GameTime gameTime)
        {
            drawOffset += speed * (int)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            if (drawOffset.X >= stageWidth)
            {
                position.X += 1;
                drawOffset.X -= stageWidth;
                didAttack = false;

                if (offStage()) isActive = false;
            }

            if (isActive && !didAttack)
            {
                if (checkEnemyLocation(position))
                {
                    doDamage(position, damage, damageType, effects, effectSprite);
                    didAttack = true;
                }
            }

            base.Update(gameTime);                               
        }
    }
}