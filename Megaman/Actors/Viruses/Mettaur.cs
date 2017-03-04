using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman.Actors.Viruses
{
    class MettaurBase : Virus
    {
        int elapsedTime;
        internal int timer;

        protected List<Vector2> metPosition;

        //1 if moving, 0 if shielded
        public bool MetMoveStatus;
        public int MetNum;

        protected MettaurBase()
        {
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            moveSprite = genericMove;
            staticSprite.Initialize(content.Load<Texture2D>("sprites/virus/mettaur/virus"),
                new Vector2(-7, 22), 22, 0, true);
            //guardSprite.Initialize(content.Load<Texture2D>("sprites/virus/mettaur/guard"),
            //    new Vector2(-7, 26), 23, 1000, false);

            guardSprite.Initialize(content.Load<Texture2D>("sprites/virus/mettaur/attack"),
                new Vector2(14, 44), 67, 65, false);

            attackSprites.Add(new Animation());
            attackSprites[0].Initialize(content.Load<Texture2D>("sprites/virus/mettaur/attack"),
                new Vector2(14, 44), 67, 1000, false);

            palette1 = content.Load<Texture2D>("sprites/virus/mettaur/mettaur").getPalette();

            MetNum = 0;
            
            //This needs to be last, textures have to be initialized first so they are assigned correctly
            base.Initialize(content, position, stage);
        }

        public override void AiInitialize()
        {
            base.AiInitialize();

            metPosition = checkFriend();

            //Sets the ai status for the first mettaur to move, rest to guard
            MettaurBase Met = (MettaurBase) stage.actorArray[(int) metPosition[0].X, (int) metPosition[0].Y];
            Met.MetMoveStatus = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            metPosition = checkMet();
        }

        public override void AI(GameTime gameTime)
        {
            if (MetMoveStatus == true)
            {
                elapsedTime += (int)gameTime.ElapsedGameTime.Milliseconds;

                if (elapsedTime > timer)
                {
                    for (int i = 0; i < enemyPosition.Count; i++)
                    {
                        if (enemyPosition[i].Y == position.Y)
                        {
                            //Sets the next guy to move mode
                            int MetNum = 0;
                            for (int j = 0; j < metPosition.Count; j++)
                            {
                                if (stage.actorArray[(int)metPosition[j].X, (int)metPosition[j].Y] == this)
                                {
                                    MetNum = j;
                                    break;
                                }
                            }
                            
                            MetMoveStatus = false;
                            setGuard();

                            int index = (MetNum + 1) % metPosition.Count;
                            MettaurBase Met = (MettaurBase)stage.actorArray[(int)metPosition[index].X, (int)metPosition[index].Y];
                            Met.breakGuard();
                            Met.MetMoveStatus = true;
                            break;
                        }
                        if (enemyPosition[i].Y < position.Y)
                        {
                            Move(new Vector2(0, -1));
                            break;
                        }
                        if (enemyPosition[i].Y > position.Y)
                        {
                            Move(new Vector2(0, 1));
                            break;
                        }
                    }

                    elapsedTime = 0;
                }
            }        
        }

        public List<Vector2> checkMet()
        {
            List<Vector2> enemies = new List<Vector2>();
            for (int i = 0; i < stage.actorArray.GetLength(0); i++)
                for (int j = 0; j < stage.actorArray.GetLength(1); j++)
                {
                    if (stage.actorArray[i, j] != null && stage.actorArray[i, j].color == color
                        && !(stage.actorArray[i, j] is Obstacle)  && (stage.actorArray[i, j] is MettaurBase))
                        enemies.Add(stage.actorArray[i, j].position);
                }

            return enemies;
        }
    }

    class Mettaur : MettaurBase
    {
        public Mettaur()
        {
            HP = 40;
            MaxHP = 40;
            timer = 1500;
        }
    }

    class Mettaur2 : MettaurBase
    {        
        public Mettaur2()
        {
            HP = 80;
            MaxHP = 80;
            timer = 1000;
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);
            
            palette2 = content.Load<Texture2D>("sprites/virus/mettaur/mettaur2").getPalette();

            staticSprite.map = base.staticSprite.map.changeColor(palette1, palette2);
            guardSprite.map = base.guardSprite.map.changeColor(palette1, palette2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {            
            base.Draw(spriteBatch);
        }
    }

    class Mettaur3 : MettaurBase
    {
        public Mettaur3()
        {
            HP = 120;
            MaxHP = 120;
            timer = 500;
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            palette2 = content.Load<Texture2D>("sprites/virus/mettaur/mettaur3").getPalette();

            staticSprite.map = base.staticSprite.map.changeColor(palette1, palette2);
            guardSprite.map = base.guardSprite.map.changeColor(palette1, palette2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }

    class MettaurΩ : MettaurBase
    {
        public MettaurΩ()
        {
            HP = 160;
            MaxHP = 120;
            timer = 250;
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            palette2 = content.Load<Texture2D>("sprites/virus/mettaur/mettaurΩ").getPalette();

            staticSprite.map = base.staticSprite.map.changeColor(palette1, palette2);
            guardSprite.map = base.guardSprite.map.changeColor(palette1, palette2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
