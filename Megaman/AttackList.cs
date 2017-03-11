using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Megaman.Actors;
using Megaman.Projectiles;


//This will store all of the attacks in the game, so they only need to be initialized once
namespace Megaman
{
    class AttackList
    {
        public struct attackSpecs
        {
            public int damage;
            public string damageType;
            public double speed;
            public Animation sprite;
            public List<string> effects;
            public Vector2 position;
            public attackSpecs(int damage, string damageType, double speed, Animation sprite)
            {
                this.damage = damage;
                this.damageType = damageType;
                this.speed = speed;
                this.sprite = sprite;
                this.position = new Vector2();
                this.effects = new List<string>();
            }
            public attackSpecs( int damage, string damageType, float speed, Animation sprite, 
                List<string> effects)
            {
                this.damage = damage;
                this.damageType = damageType;
                this.speed = speed;
                this.sprite = sprite;
                this.effects = effects;
                this.position = new Vector2();
            }
        }

        internal Animation waveSprite;
        internal Animation recoverSprite;

        public AttackList()
        {
            waveSprite = new Animation();
            recoverSprite = new Animation();
        }

        public void Initialize(ContentManager content)
        {
            waveSprite.Initialize(content.Load<Texture2D>("sprites/effects/wave"), new Vector2(4, 48),
                46, 125, false);
            recoverSprite.Initialize(content.Load<Texture2D>("sprites/effects/recover"), new Vector2(4, 60),
                43, 20, false);
        }

        public void MegaBuster(Actor actor, int damage)
        {
            actor.info.damage = damage;
            actor.info.damageType = "Null";
            actor.info.effects = new List<string>();

            actor.Shoot(actor.busterSprite, actor.Gun);
        }

        // a speed of 1 will cover one tile per second
        public void Wave(Actor actor, int damage, double speed)
        {
            Animation temp = waveSprite.Clone();
            temp.frameTime = 1000 / (temp.frameCount * speed);


            actor.info.speed = speed;
            actor.info.sprite = temp;
            actor.info.damage = damage;
            actor.info.damageType = "null";
            actor.info.effects = new List<string>();

            projectileInitialization(actor);

            actor.Hammer(null, actor.createWave);
        }

        public void Recover(Actor actor, int recov)
        {
            actor.Heal(actor, recov);
            actor.stage.addEffect(recoverSprite.Clone(), actor.location);
        }

        internal void projectileInitialization(Actor actor)
        {
            if (actor.color == "blue")
            {
                actor.info.speed = -actor.info.speed;
                actor.info.position = actor.position - new Vector2(1, 0);
                actor.info.sprite.Flip();
            }
            else actor.info.position = actor.position + new Vector2(1, 0);
        }
    }
}