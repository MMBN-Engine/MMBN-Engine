using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman.Actors.Viruses
{
    class Virus : Actor
    {
        internal Animation genericMove;
        SpriteFont HpDisplay;

        public Virus()
        {
            color = "blue";

            genericMove = new Animation();
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            genericMove.Initialize(content.Load<Texture2D>("sprites/move"),new Vector2(-2, 42), 30, 15, false);
            deathSprite.Initialize(content.Load<Texture2D>("sprites/effects/explosion"), new Vector2(7, 34), 50, 30, false);

            HpDisplay = content.Load<SpriteFont>("virus-hp");  
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            AI(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DrawHP(spriteBatch);
        }

        public void DrawHP(SpriteBatch spriteBatch)
        {
            //Probably only works for mettaurs, will need to change
            int length = (int)HpDisplay.MeasureString(HP.ToString()).X;
            spriteBatch.DrawString(HpDisplay, HP.ToString(),
                location + new Vector2(26 - length, -3), Color.White);
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
        }
    }
}
