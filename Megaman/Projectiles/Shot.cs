using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string damageType;
        public List<String> effects;

        public Shot(Animation sprite, String color, Vector2 position, Stage stage, Vector2 speed, 
            int damage, String damageType, List<string> effects) : base(sprite, color, position, stage)
        {
            activeSprite = sprite;
            stage.addProjectile(this);
            base.Initialize(null, position, stage);
            isActive = true;

            this.speed = speed;
            this.damage = damage;
            this.damageType = damageType;
            this.effects = effects;
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
                    doDamage(position, damage, damageType, effects);
                    didAttack = true;
                }
            }

            base.Update(gameTime);                               
        }
    }
}