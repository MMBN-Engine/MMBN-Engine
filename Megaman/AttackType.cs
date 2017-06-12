using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomExtensions.CustomExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Megaman.Actors;
using Megaman.Projectiles;


//This will store all of the attacks in the game, so they only need to be initialized once
namespace Megaman
{
    public class AttackType
    {
        public ParamsAction onAction;
        public string name;

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

        public Animation waveSprite;
        public SoundEffect waveSound;

        public Animation recoverSprite;
        public SoundEffect recoverSound;

        public Animation spreaderSprite;
        public SoundEffect spreaderSound;

        public Animation bubblerSprite;
        public SoundEffect bubblerSound;

        public Animation heaterSprite;
        public SoundEffect heaterSound;

        public AttackType()
        {
            waveSprite = new Animation();
            recoverSprite = new Animation();
            spreaderSprite = new Animation();
            heaterSprite = new Animation();
            bubblerSprite = new Animation();

            onAction = (ParamsAction) delegate (object[] args) { };
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

        public void Action(params object[] args)
        {
            object[] args2 = new object[args.Count() + 1];
            args2[0] = this;
            args.CopyTo(args2, 1);

            onAction(args2);
        }

        public void projectileInitialization(Actor actor)
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