using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//right now all movement code is for teleporting, will change it to allow sliding later

namespace Megaman.Actors
{
    class Actor
    {
        public int HP, MaxHP;
        public Vector2 position;
        public string color;
        public Animation staticSprite, moveSprite, guardSprite;
        public List<Animation> attackSprites;
        public List<Color> palette1, palette2;
        public int[] panelHeight;

        public bool AquaBody, ElecBody, FireBody, WoodBody;

        public Stage stage;

        protected int attackNum;
        public bool isAttacking;

        protected bool isGuarding;    //Check if we are in the process of doing the guard animation
        public bool Guard;         //True - guard effect is active
        internal int guardTime;    //Maximum time before guard resests
        internal int guardElapsed; //Time since timer was set 

        protected bool moveStart, moveFin, isSliding;
        protected Vector2 move;
        public Vector2 location;
        protected List<Vector2> enemyPosition;
        protected List<Vector2> friendPosition;

        public delegate void attackMethod(int dammage, string damageType, List<String> effects, List<Animation> sprites);

        //Atributes
        public bool FlotShoe, AirShoe;

        public bool dead;
  
        public Actor()
        {
            
            // hard coding here :/
            panelHeight = new int[2];
            panelHeight[0] = 23;
            panelHeight[1] = 24;

            staticSprite = new Animation();
            moveSprite = new Animation();
            attackSprites = new List<Animation>();
            guardSprite = new Animation();

        }

        public virtual void Initialize(ContentManager content,  Vector2 position, Stage stage)
        {
            this.position = position;

            this.stage = stage;

            stage.actorArray[(int)position.X, (int)position.Y] = this;
        }

        public virtual void Update(GameTime gameTime)
        {          
            staticSprite.Update(gameTime);
            
            if (moveStart)
            {
                moveSprite.Update(gameTime);

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
                moveSprite.Update(gameTime);

                if (!moveSprite.active)
                {
                    moveFin = false;
                    
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
                attackSprites[attackNum].Update(gameTime);
                if (!attackSprites[attackNum].active) isAttacking = false;
            }

            //Sets guard when sprite animation finishes
            if (isGuarding)
            {
                guardSprite.Update(gameTime);
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

            enemyPosition = checkEnemy();
            friendPosition = checkFriend();

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
                moveSprite.active = true;
                moveSprite.forward = true;
            }

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            int stageWidth = 40;
            int heightSum = 72;
            for (int i = 0; i < position.Y; i++)
                heightSum += panelHeight[i];

            
            //offsets due to the position of the stage
            Vector2 offSet = new Vector2(3,16);
            
            //finds the location to draw it in
            location = new Vector2(offSet.X + position.X*stageWidth, offSet.Y + heightSum);

            if (moveStart | moveFin) moveSprite.Draw(spriteBatch, location);
            else if (isAttacking) attackSprites[attackNum].Draw(spriteBatch, location);
            else if (isGuarding | Guard) guardSprite.Draw(spriteBatch, location);
            else staticSprite.Draw(spriteBatch, location);
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
                isAttacking = true;
                this.attackNum = attackNum;
            }
        }

        public List<Vector2> checkEnemy()
        {
            List<Vector2> enemies = new List<Vector2>();
            for (int i = 0; i < stage.actorArray.GetLength(0); i++)
                for (int j = 0; j < stage.actorArray.GetLength(1); j++)
                {
                    if (stage.actorArray[i,j] != null && stage.actorArray[i, j].color != color
                        && !(stage.actorArray[i,j] is Obstacle))
                        enemies.Add(stage.actorArray[i, j].position);
                }

            return enemies;
        }

        public List<Vector2> checkFriend()
        {
            List<Vector2> enemies = new List<Vector2>();
            for (int i = 0; i < stage.actorArray.GetLength(0); i++)
                for (int j = 0; j < stage.actorArray.GetLength(1); j++)
                {
                    if (stage.actorArray[i, j] != null && stage.actorArray[i, j].color == color
                        && !(stage.actorArray[i, j] is Obstacle))
                        enemies.Add(stage.actorArray[i, j].position);
                }

            return enemies;
        }

        public void Heal(Actor actor, int value)
        {
            actor.HP += value;
            if (actor.HP > actor.MaxHP) actor.HP = actor.MaxHP;
        }

        public void Gun(int damage, string damageType, List<String> effects, List<Animation> sprites)
        {
            int y = (int)position.Y;

            if (color == "red")
            {
                for (int i = (int) position.X + 1; i < 6; i++)
                {
                    if (isBlue(i, y))
                    {
                        doDamage(new Vector2(i, y), damage, damageType, effects);
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
                        doDamage(new Vector2(i, y), damage, damageType, effects);
                        break;
                    }
                }
            }
        }

        public void Sword(int damage, string damageType, List<String> effects, List<Animation> sprites)
        {
            int x = (int) position.X;
            int y = (int) position.Y;
            
            if (color == "red")
            {
                if (x < 5 && isBlue(x + 1, y))
                {
                    doDamage(new Vector2(x + 1, y), damage, damageType, effects);
                }
            }

            if (color == "blue")
            {
                if (x > -1  && isRed(x - 1, y))
                {
                    doDamage(new Vector2(x - 1, y), damage, damageType, effects);
                }
            }
        }

        //Initialize the guard animation and sets time to zero
        public void setGuard()
        {
            guardSprite.Reset();
            isGuarding = true;
            guardElapsed = 0;
        }

        public void breakGuard()
        {
            Guard = false;
        }

        public bool isRed(int x, int y)
        {
            if (!(stage.actorArray[x, y] == null) && (stage.actorArray[x, y].color == "red")) return true;
            else return false;
        }

        public bool isBlue(int x, int y)
        {
            if (!(stage.actorArray[x, y] == null) && (stage.actorArray[x, y].color == "blue")) return true;
            else return false;
        }

        public void doDamage(Vector2 position, int damage, string damageType, List<string> effects)
        {
            //Applies damage
            int damReturn = damage;
            string panel = stage.PanelType[(int)position.X, (int)position.Y];
            Actor target = stage.actorArray[(int)position.X, (int)position.Y];

            //Checks body type
            if (target.AquaBody && damageType == "Elec") damReturn += damage;
            if (target.ElecBody && damageType == "Wood") damReturn += damage;
            if (target.FireBody && damageType == "Aqua") damReturn += damage;
            if (target.WoodBody && damageType == "Fire") damReturn += damage;

            //Checks stage type
            if (panel == "Grass" && damageType == "Fire")
            {
                damReturn += damage;
                stage.PanelType[(int)position.X, (int)position.Y] = "null";
            }
            if (panel == "Lava" && damageType == "Aqua")
            {
                damReturn += damage;
                stage.PanelType[(int)position.X, (int)position.Y] = "null";
            }
            if ((panel == "Ice" | panel == "Metal") && damageType == "Elec") damReturn += damage;
            if (panel == "holy") damReturn = damReturn / 2;

            if ((target.Guard) &! (effects.Contains("Break"))) damReturn = 0;
            
            target.HP -= damReturn;
        }
    }
}