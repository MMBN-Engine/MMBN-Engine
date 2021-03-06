﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using CustomExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Megaman.Actors.Viruses
{
    class Virus : Actor
    {
        internal Animation genericMove;
        public bool didAttack;  //Lets us know if we did attack already, useful for virus ai
        SpriteFont HpDisplay;

        public string family;

        public Virus() : base()
        {
            color = "blue";

            genericMove = new Animation();
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            gfxFolder = "gfx/virus/" + family + "/";

            base.Initialize(content, position, stage);

            genericMove.Initialize(content.Load<Texture2D>("sprites/move"),new Vector2(-2, 42), 30, 15, false);
            deathSprite.Initialize(content.Load<Texture2D>("sprites/effects/explosion"), new Vector2(7, 34), 50, 30, false);
            deathSound = content.Load<SoundEffect>("soundFX/battle/explodeShort");

            HpDisplay = content.Load<SpriteFont>("virus-hp");

            paletteSwap(family, name);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            AI(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, float resolution)
        {
            base.Draw(spriteBatch, resolution);
            DrawHP(spriteBatch, resolution);
        }

        public void DrawHP(SpriteBatch spriteBatch, float resolution)
        {
            //Probably only works for mettaurs, will need to change
            int length = (int)HpDisplay.MeasureString(HP.ToString()).X;
            spriteBatch.DrawString(HpDisplay, HP.ToString(),
                (location + new Vector2(26 - length, -3)) * resolution,
                scale: resolution, color: Color.White, rotation: 0, origin: new Vector2(), effects: SpriteEffects.None,
                layerDepth: 0);
        }

        public virtual void AiInitialize()
        {
        }

        public virtual void AI(GameTime gameTime)
        {
        }

        public override void Delete()
        {
            stage.addEffect(deathSprite, location);
            stage.actorArray.SetValue(position, (Actor) null);
            deathSound.Play();
        }

        public override void doAttack(string attackName)
        {
            base.doAttack(attackName);
            didAttack = true;
        }

    }
}
