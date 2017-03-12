using System;
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

        public Virus(AttackList attackTypes) : base(attackTypes)
        {
            color = "blue";

            genericMove = new Animation();
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            genericMove.Initialize(content.Load<Texture2D>("sprites/move"),new Vector2(-2, 42), 30, 15, false);
            deathSprite.Initialize(content.Load<Texture2D>("sprites/effects/explosion"), new Vector2(7, 34), 50, 30, false);
            deathSound = content.Load<SoundEffect>("soundFX/battle/explodeShort");

            HpDisplay = content.Load<SpriteFont>("virus-hp");  
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
            stage.actorArray[(int)position.X, (int)position.Y] = null;
            deathSound.Play();
        }

        public override void doAttack(int attackNum)
        {
            base.doAttack(attackNum);
            didAttack = true;
        }

        public void paletteSwap()
        {
            staticSprite.map = staticSprite.map.changeColor(palette1, palette2);
            guardSprite.map = guardSprite.map.changeColor(palette1, palette2);
            foreach (Animation foo in attackSprites)
            {
                foo.map = foo.map.changeColor(palette1, palette2);
            }
        }

    }
}
