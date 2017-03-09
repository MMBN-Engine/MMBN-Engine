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

        internal int damage;
        internal double speed;

        protected List<Vector2> metPosition;

        //1 if moving, 0 if shielded
        public bool MetMoveStatus;

        protected MettaurBase(AttackList attackTypes) : base(attackTypes)
        {
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            moveSprite = genericMove;
            staticSprite.Initialize(content.Load<Texture2D>("sprites/virus/mettaur/virus"),
                new Vector2(-7, 22), 22, 0, true);
            guardSprite.Initialize(content.Load<Texture2D>("sprites/virus/mettaur/guard"),
                new Vector2(-7, 26), 23, 100, false);

            attackSprites[3].Initialize(content.Load<Texture2D>("sprites/virus/mettaur/attack"),
                new Vector2(14, 44), 67, 100, false);

            palette1 = content.Load<Texture2D>("sprites/virus/mettaur/mettaur").getPalette();

            attackFrame = new List<int> { 0, 0, 0, 6 };

            setSpeed();
            
            //This needs to be last, textures have to be initialized first so they are assigned correctly
            base.Initialize(content, position, stage);
        }

        public override void AiInitialize()
        {
            base.AiInitialize();

            metPosition = checkMet();

            //Sets the ai status for the first mettaur to move, rest to guard
            MettaurBase Met = (MettaurBase) stage.actorArray[(int) metPosition[0].X, (int) metPosition[0].Y];
            Met.MetMoveStatus = true;
            if (!(this == Met)) setGuard();
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
                if (didAttack && !isAttacking)
                {
                    activateNext();

                    MetMoveStatus = false;
                    setGuard();
                }

                elapsedTime += (int)gameTime.ElapsedGameTime.Milliseconds;

                if (elapsedTime > timer)
                {
                    for (int i = 0; i < enemyPosition.Count; i++)
                    {
                        if (enemyPosition[i].Y == position.Y)
                        {
                            //Sets the next guy to move mode

                            attackTypes.Wave(this, damage, speed);

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

        public void setSpeed()
        {
            timer = 1500/(int) speed;
            guardSprite.frameTime = guardSprite.frameTime / speed;
            attackSprites[3].frameTime = attackSprites[3].frameTime / speed;
        }

        //Sets the next mettaur to active
        public void activateNext()
        {
            int MetNum = 0;
            for (int j = 0; j < metPosition.Count; j++)
            {
                if (stage.actorArray[(int)metPosition[j].X, (int)metPosition[j].Y] == this)
                {
                    MetNum = j;
                    break;
                }
            }

            int index = (MetNum + 1) % metPosition.Count;
            MettaurBase Met = (MettaurBase)stage.actorArray[(int)metPosition[index].X, (int)metPosition[index].Y];
            Met.breakGuard();
            Met.didAttack = false;
            Met.elapsedTime = 0;
            Met.MetMoveStatus = true;
        }

        public override void Delete()
        {
            base.Delete();
            activateNext();
        }
    }

    class Mettaur : MettaurBase
    {
        public Mettaur(AttackList attackTypes) : base(attackTypes)
        {
            HP = 40;
            MaxHP = 40;
            damage = 10;
            noGuarding = true;
            speed = 1;
        }
    }

    class Mettaur2 : MettaurBase
    {        
        public Mettaur2(AttackList attackTypes) : base(attackTypes)
        {
            HP = 80;
            MaxHP = 80;
            timer = 1000;
            damage = 40;
            speed = 1.3;
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);
            
            palette2 = content.Load<Texture2D>("sprites/virus/mettaur/mettaur2").getPalette();
            paletteSwap();
        }
    }

    class Mettaur3 : MettaurBase
    {
        public Mettaur3(AttackList attackTypes) : base(attackTypes)
        {
            HP = 120;
            MaxHP = 120;
            timer = 500;
            damage = 80;
            speed = 1.6;
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            palette2 = content.Load<Texture2D>("sprites/virus/mettaur/mettaur3").getPalette();
            paletteSwap();
        }
    }

    class MettaurΩ : MettaurBase
    {
        public MettaurΩ(AttackList attackTypes) : base(attackTypes)
        {
            HP = 160;
            MaxHP = 120;
            timer = 250;
            damage = 150;
            speed = 2.5;
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            palette2 = content.Load<Texture2D>("sprites/virus/mettaur/mettaurΩ").getPalette();
            paletteSwap();
        }
    }
}
