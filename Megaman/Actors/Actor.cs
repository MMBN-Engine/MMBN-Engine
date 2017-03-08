using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Megaman.AttackList;
using Megaman.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//right now all movement code is for teleporting, will change it to allow sliding later

namespace Megaman.Actors
{
    class Actor : Object
    {
        public int HP, MaxHP;
        public Animation staticSprite, moveSprite, guardSprite;
        public Animation busterSprite;

        public List<Animation> attackSprites;
        // 0 - shoot
        // 1 - sword
        // 2 - bomb
        // 3 - hammer

        public List<Color> palette1, palette2;

        public Animation gunSprite;
        public bool isShooting;

        public int Attack; //We need this here so the Attacklist class understands the megabuster

        public Animation deathSprite;  //Sprite to play when we die

        public bool AquaBody, ElecBody, FireBody, WoodBody;

        protected int attackNum;
        public bool isAttacking;
        public bool isSlashing;

        internal AttackList attackTypes;

        protected bool isGuarding;    //Check if we are in the process of doing the guard animation
        public bool Guard;         //True - guard effect is active
        internal int guardTime;    //Maximum time before guard resests
        internal int guardElapsed; //Time since timer was set 

        protected bool moveStart, moveFin, isSliding;
        protected Vector2 move;

        public delegate void attackMethod(attackSpecs info);
        public attackMethod attackHandle;
        public attackSpecs info;                //structure containing attack information

        //Atributes
        public bool FlotShoe, AirShoe;

        public bool isDead;
  
        public Actor(AttackList attacks)
        {
            attackTypes = attacks;

            staticSprite = new Animation();
            moveSprite = new Animation();
            attackSprites = new List<Animation>();
            guardSprite = new Animation();
            deathSprite = new Megaman.Animation();

            attackSprites.Add(new Animation());
            attackSprites.Add(new Animation());
            attackSprites.Add(new Animation());
            attackSprites.Add(new Animation());

            info = new attackSpecs();
        }

        public override void Initialize(ContentManager content,  Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            stage.actorArray[(int)position.X, (int)position.Y] = this;
            activeSprite = staticSprite;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
                               
            if (moveStart)
            {
                if (!moveSprite.active)
                {
                    moveStart = false;
                    moveFin = true;
                    moveSprite.currentFrame = moveSprite.frameCount - 1;
                    moveSprite.active = true;
                    moveSprite.forward = false;
                    doMove();
                }
            }

            if (moveFin)
            {
                if (!moveSprite.active)
                {
                    moveFin = false;
                    activeSprite = staticSprite;
                    
                    //Slide on ice panels, need to do it here to make sure he finishes his move
                    if (canMove(move) && !FlotShoe && !AquaBody &&
                        stage.PanelType[(int)(position.X + move.X),
                        (int)(position.Y + move.Y)].Equals("Ice"))
                    {
                        isSliding = true;
                        Move(move);
                    }
                    else isSliding = false;
                }
            }

            if (isAttacking)
            {
                if (!attackSprites[attackNum].active)
                {
                    isAttacking = false;
                    activeSprite = staticSprite;
                }
            }

            //Sets guard when sprite animation finishes
            if (isGuarding)
            {
                if (!guardSprite.active)
                {
                    isGuarding = false;
                    Guard = true;
                }
            }

            //Advance the guard timer, break guard if over
            if (Guard)
            {
                guardElapsed += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if ((guardElapsed > guardTime) && (guardTime > 0))
                {
                    breakGuard();
                }
            }
        }
        

        //as of now, this should only be used to move one space
        //i'll add a teleport bool later to avoid ice weirdness
        public virtual void Move(Vector2 move)
        {
            if (canMove(move))
            {
                //don't allow new movement if movement is being forced.
                if (!isSliding) this.move = move;
                
                moveStart = true;
                moveSprite.Reset();
                moveSprite.forward = true;
                activeSprite = moveSprite;
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public bool canMove(Vector2 move)
        {
            //checks to see if we're already moving
            if (moveStart | moveFin) return false;

            //checks to see if we're attacking
            if (isAttacking) return false;

            //checks to see if we are guarding
            if (isGuarding | Guard) return false;
           
            //checks to see if the tile exists and is controlled by the right dude and no one is there
            if (position.X + move.X > -1 && position.X + move.X < stage.PanelType.GetLength(0) &&
                position.Y + move.Y > -1 && position.Y + move.Y < stage.PanelType.GetLength(1)
                && color.Equals(stage.Area[(int)(position.X + move.X), (int)(position.Y + move.Y)])
                && stage.actorArray[(int)(position.X + move.X), (int)(position.Y + move.Y)] == null)
            {
                //can't step on broken panels without airshoes
                if (stage.PanelType[(int)(position.X + move.X), (int)(position.Y + move.Y)].Equals("Broken")
                    | stage.PanelType[(int)(position.X + move.X), (int)(position.Y + move.Y)].Equals("Hole"))
                    
                    if (AirShoe) return true;
                    else return false;

                return true;
            }
            else return false;
        }

        //does the actual movement
        private void doMove()
        {
            //checks to see if we float
            if (!FlotShoe)
            {
                //we crack broken panels
                if (stage.PanelType[(int)position.X, (int)position.Y].Equals("Cracked") && !FlotShoe)
                    stage.PanelType[(int)position.X, (int)position.Y] = "Broken";
            }

            // Tells the stage we moved
            stage.actorArray[(int)position.X, (int)position.Y] = null;
            position += move;
            stage.actorArray[(int)position.X, (int)position.Y] = this;
        }

        public bool canAttack()
        {
            if (moveStart | moveFin | isSliding | isAttacking) return false;
            else return true;
        }

        public void doAttack(int attackNum)
        {
            if (canAttack())
            {
                attackSprites[attackNum].Reset();
                activeSprite = attackSprites[attackNum];
                isAttacking = true;
                this.attackNum = attackNum;
            }
        }
        
        public void Heal(Actor actor, int value)
        {
            actor.HP += value;
            if (actor.HP > actor.MaxHP) actor.HP = actor.MaxHP;
        }

        public void Gun(attackSpecs info)
        {
            int y = (int)position.Y;

            if (color == "red")
            {
                for (int i = (int) position.X + 1; i < 6; i++)
                {
                    if (isBlue(i, y))
                    {
                        doDamage(new Vector2(i, y), info.damage, info.damageType, info.effects);
                        break;
                    }
                }
            }

            if (color == "blue")
            {
                for (int i = (int) position.X - 1; i > -1; i--)
                {
                    if (isRed(i, y))
                    {
                        doDamage(new Vector2(i, y), info.damage, info.damageType, info.effects);
                        break;
                    }
                }
            }
        }

        public void Sword(attackSpecs info)
        {
            int x = (int) position.X;
            int y = (int) position.Y;
            
            if (color == "red")
            {
                if (x < 5 && isBlue(x + 1, y))
                {
                    doDamage(new Vector2(x + 1, y), info.damage, info.damageType, info.effects);
                }
            }

            if (color == "blue")
            {
                if (x > -1  && isRed(x - 1, y))
                {
                    doDamage(new Vector2(x - 1, y), info.damage, info.damageType, info.effects);
                }
            }
        }

        public void Slash(Animation animation, int damage, string damageType, List<string> effects, attackMethod attackHandle,
            List<Animation> sprites)
        {
            if (!canAttack()) return;
            //swordSprite.active = true;
            //swordSprite = animation;
            doAttack(1);
            isSlashing = true;

            this.attackHandle = attackHandle;
            this.info.damage = damage;
            this.info.damageType = damageType;
            this.info.effects = effects;
        }

        public void createShot(attackSpecs info)
        {
            new Shot(this);
        }

        public void createWave(attackSpecs info)
        {
            new Wave(this);
        }

        //Initialize the guard animation and sets time to zero
        public void setGuard()
        {
            guardSprite.Reset();
            activeSprite = guardSprite;
            isGuarding = true;
            guardElapsed = 0;
        }

        public void breakGuard()
        {
            activeSprite = staticSprite;
            Guard = false;
        }

        public void Shoot(Animation animation, attackMethod attackHandle)
        {
            if (!canAttack()) return;
            gunSprite = animation;
            gunSprite.Reset();
            doAttack(0);
            isShooting = true;

            this.attackHandle = attackHandle;
        }

        public virtual void Delete()
        {
        }
    }
}