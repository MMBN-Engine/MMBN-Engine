﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman
{
    public class Animation
    {
        public Texture2D map;
        int elapsedTime;
        public double frameTime;
        public int frameCount;
        public int currentFrame;
        Rectangle frame = new Rectangle();
        private int frameWidth;
        private int frameHeight;
        public bool active;
        public bool looping;

        public bool flip;
        
        //for which direction it will play, so far it is only for non looping but i can
        //extend it if i need to
        public bool forward;
        public Vector2 origin;

        public void Initialize(Texture2D map, Vector2 origin, int frameWidth, int frameTime,
            bool looping)
        {
            this.frameWidth = frameWidth;
            this.frameTime = frameTime;
            this.frameCount = map.Width/frameWidth;

            this.looping = looping;
            this.map = map;

            this.origin = origin;

            elapsedTime = 0;
            currentFrame = 0;

            active = true;
            forward = true;
            frameHeight = map.Height;

            //Let's us draw before the Animation is first updated.
            frame = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
        }
        
        public void Update(GameTime gameTime)
        {
            if (active == false)
                return;

            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                if (frameTime != 0)
                    if (forward)
                        currentFrame++;
                    else currentFrame--;

                if (currentFrame == frameCount && forward)
                {
                    if (!looping)
                    {
                        active = false;
                        currentFrame--;
                    }
                    else currentFrame = 0;
                }

                if (currentFrame == 0 && !forward)
                    active = false;
                elapsedTime = 0;
            }
        }
       
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float resolution)
        {
            frame = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            if (flip)
                spriteBatch.Draw(texture: map, position: (position - origin) * resolution, sourceRectangle: frame,
                    color: Color.White, scale: new Vector2(1, 1) * resolution,
                    effects: SpriteEffects.FlipHorizontally);
            else
                spriteBatch.Draw(texture: map, position: (position - origin) * resolution, sourceRectangle: frame,
                    color: Color.White, scale: new Vector2(1, 1) * resolution);
        }

        public void Reset()
        {
            active = true;
            elapsedTime = 0;
            currentFrame = 0;
        }

        public Animation Clone()
        {
            return (Animation)this.MemberwiseClone();
        }

        public void Flip()
        {
            flip = true;
            origin.X = - 40 + (frameWidth - origin.X);
        }
    }
}