using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using static Megaman.AttackList;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//right now all movement code is for teleporting, will change it to allow sliding later

namespace Megaman.Projectiles
{
    class Projectile : Object
    {
        public bool isActive;
  
        public Projectile(Actor actor)
        {
            activeSprite = actor.info.sprite;
            actor.stage.addProjectile(this);
            color = actor.color;
            base.Initialize(null, actor.info.position, actor.stage);
            isActive = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);                               
        }

        public bool offStage()
        {
            if (position.X > 5 | position.X < 0 | position.Y > 2 | position.Y < 0) return true;
            else return false;
        }
    }
}