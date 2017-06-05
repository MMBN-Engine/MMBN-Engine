using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Megaman.Actors;
using Megaman.Projectiles;


//This will store all of the attacks in the game, so they only need to be initialized once
namespace Megaman
{
    public class AttackList
    {
        public struct attackSpecs
        {
            public int damage;
            public List<string> damageType;
            public double speed;
            public Animation sprite, effectSprite;
            public List<string> effects;
            public Vector2 position;
            public SoundEffect sound;
            public attackSpecs(int foo)
            {
                damage = 0;
                damageType = new List<string> { "Null" };
                speed = 0;
                sprite = new Animation();
                effects = new List<string>();
                position = new Vector2();
                sound = null;
                effectSprite = new Animation();
            }
            public void Reset()
            {
                damage = 0;
                damageType = new List<string> { "Null" };
                speed = 0;
                sprite = new Animation();
                effects = new List<string>();
                position = new Vector2();
                sound = null;
                effectSprite = null;
            }
        }

        internal Animation waveSprite;
        internal SoundEffect waveSound;

        internal Animation recoverSprite;
        internal SoundEffect recoverSound;

        internal Animation spreaderSprite;
        internal SoundEffect spreaderSound;

        internal Animation bubblerSprite;
        internal SoundEffect bubblerSound;

        internal Animation heaterSprite;
        internal SoundEffect heaterSound;

        public AttackList()
        {
            waveSprite = new Animation();
            recoverSprite = new Animation();
            spreaderSprite = new Animation();
            heaterSprite = new Animation();
            bubblerSprite = new Animation();
        }

        public void Initialize(ContentManager content)
        {
            waveSprite.Initialize(content.Load<Texture2D>("sprites/effects/wave"), new Vector2(4, 48),
                46, 125, false);
            waveSound = content.Load<SoundEffect>("soundFX/battle/wave");

            recoverSprite.Initialize(content.Load<Texture2D>("sprites/effects/recover"), new Vector2(4, 60),
                43, 20, false);
            recoverSound = content.Load<SoundEffect>("soundFX/battle/recover");

            spreaderSprite.Initialize(content.Load<Texture2D>("sprites/effects/spreader"), new Vector2(4, 30),
                42, 40, false);
            spreaderSound = content.Load<SoundEffect>("soundFX/battle/spreader");

            bubblerSprite.Initialize(content.Load<Texture2D>("sprites/effects/spreader"), new Vector2(4, 30),
                42, 40, false);
            bubblerSound = content.Load<SoundEffect>("soundFX/battle/bubbler");

            heaterSprite.Initialize(content.Load<Texture2D>("sprites/effects/explosion"), new Vector2(7, 34), 50, 30, false);
            heaterSound = content.Load<SoundEffect>("soundFX/battle/heater");
        }

        public void MegaBuster(Actor actor, int damage)
        {
            actor.info.Reset();
            actor.info.damage = damage;

            actor.Shoot(actor.busterSprite, actor.Gun);
        }

        public void Spreader(Actor actor, int damage, string effect)
        {
            actor.info.Reset();
            actor.info.damage = damage;
            actor.info.damageType = new List<string> { "Null" };
            actor.info.effects.Add(effect);
            actor.info.effectSprite = spreaderSprite.Clone();
            actor.info.sound = spreaderSound;

            actor.Shoot(actor.busterSprite, actor.Gun);
        }

        public void Heater(Actor actor, int damage, string effect)
        {
            actor.info.Reset();
            actor.info.damage = damage;
            actor.info.damageType = new List<string> { "Fire" };
            actor.info.effects.Add(effect);
            actor.info.effectSprite = heaterSprite.Clone();
            actor.info.sound = heaterSound;

            actor.Shoot(actor.busterSprite, actor.Gun);
        }

        public void Bubbler(Actor actor, int damage, string effect)
        {
            actor.info.Reset();
            actor.info.damage = damage;
            actor.info.damageType = new List<string> { "Aqua" };
            actor.info.effects.Add(effect);
            actor.info.effectSprite = bubblerSprite.Clone();
            actor.info.sound = bubblerSound;

            actor.Shoot(actor.busterSprite, actor.Gun);
        }

        // a speed of 1 will cover one tile per second
        public void Wave(Actor actor, int damage, double speed)
        {
            Animation temp = waveSprite.Clone();
            temp.frameTime = 1000 / (temp.frameCount * speed);

            actor.info.Reset();
            actor.info.speed = speed;
            actor.info.sprite = temp;
            actor.info.damage = damage;
            actor.info.sound = waveSound;

            projectileInitialization(actor);

            actor.Hammer(null, actor.createWave);
        }

        public void Recover(Actor actor, int recov)
        {
            recoverSound.Play();
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