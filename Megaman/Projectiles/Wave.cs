using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using static Megaman.AttackType;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

//right now all movement code is for teleporting, will change it to allow sliding later

namespace Megaman.Projectiles
{
    class Wave : Projectile
    {
        public Vector2 speed;
        public int damage;
        public bool didAttack;  //Tracks whether or not we attacted on a given panel
        public List<string> damageType;
        public List<String> effects;
        public float waveTimer;
        public float waveTime;  //Time to advance the wave
        public SoundEffect sound;

        public Wave(Actor actor) : base(actor)           
        {
            speed = new Vector2(1, 0) * (float)actor.info.speed *46;
            damage = actor.info.damage;
            damageType = actor.info.damageType;
            effects = actor.info.effects;

            sound = actor.info.sound;
            sound?.Play();
        }

        public override void Update(GameTime gameTime)
        {
            if (waveTime > 0)
            {
                waveTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if ((waveTimer > waveTime) && isActive)
                {
                    isActive = false;
                    Clone();
                }
            }

            if (!activeSprite.active && isActive)
            {
                isActive = false;
                //activeSprite.Reset();
                Clone();
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

        public void Clone()
        {
            if ((speed.X > 0 && position.X < 5) || (speed.X < 0 && position.X > 0))
            {
                Wave newWave = (Wave)this.MemberwiseClone();
                newWave.Reset();
                newWave.position += new Vector2(1, 0) * Math.Sign(speed.X);
                stage.addProjectile(newWave);
                sound?.Play();
            }
        }

        public void Reset()
        {
            activeSprite = activeSprite.Clone();
            activeSprite.Reset();
            waveTimer = 0;
            isActive = true;
            didAttack = false;
        }
    }
}