using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Megaman.Actors;

//right now all movement code is for teleporting, will change it to allow sliding later

namespace Megaman
{
    public class Object
    {
        public Vector2 position;
        public string color;
        public int[] panelHeight;
        public int stageWidth;

        public Animation activeSprite;

        public Stage stage;

        protected Animation effectSprite;
        
        protected List<Vector2> enemyPosition;
        protected List<Vector2> friendPosition;

        public Vector2 drawOffset;  //Offset from center for drawing
        public Vector2 location;

        public Object()
        {
            
            // hard coding here :/
            panelHeight = new int[2];
            panelHeight[0] = 23;
            panelHeight[1] = 24;

            stageWidth = 40;

            activeSprite = new Animation(); //This is the actual sprite we draw
            effectSprite = null; //Effects such as explosions etc.

            drawOffset = new Vector2(0, 0);
        }

        public virtual void Initialize(ContentManager content,  Vector2 position, Stage stage)
        {
            this.position = position;

            this.stage = stage;
        }

        public virtual void Update(GameTime gameTime)
        {
            activeSprite.Update(gameTime);

            enemyPosition = checkEnemy();
            friendPosition = checkFriend();
        }


        //eventually we want to move some of the movement code here,
        //so it can be used by projectiles, viruses, etc.

        public virtual void Draw(SpriteBatch spriteBatch, float resolution)
        {
            location = getLocation(position);

            //Draw with shifted position for motion
            activeSprite.Draw(spriteBatch, location + drawOffset, resolution);
        }

        //gets the draw location from position
        public Vector2 getLocation(Vector2 position)
        {
            int heightSum = 72;
            for (int i = 0; i < position.Y; i++)
                heightSum += panelHeight[i];

            //offsets due to the position of the stage
            Vector2 offSet = new Vector2(3, 16);
            return new Vector2(offSet.X + position.X * stageWidth, offSet.Y + heightSum);
        }

        //Determine if an enemy occupies the grid at position
        public bool checkEnemyLocation(Vector2 position)
        {
            int i = (int)position.X;
            int j = (int)position.Y;
            if (stage.actorArray[i, j] != null && stage.actorArray[i, j].color != color
                        && !(stage.actorArray[i, j] is Obstacle)) return true;
            else return false;
        }

        public List<Vector2> checkEnemy()
        {
            List<Vector2> enemies = new List<Vector2>();
            for (int i = 0; i < stage.actorArray.GetLength(0); i++)
                for (int j = 0; j < stage.actorArray.GetLength(1); j++)
                {
                    if (checkEnemyLocation(new Vector2(i, j)))
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

        //Find targets to apply damage to
        public void doDamage(Vector2 position, int damage, List<string> damageType, List<string> effects, Animation effectSprite)
        {
            this.effectSprite = effectSprite;
            List<Vector2> targetList = new List<Vector2>();

            if (effects == null) effects = new List<string>();

            if (effects.Contains("wide"))
            {
                for (int i = (int)position.Y - 1; i <= (int)position.Y + 1; i++)
                {
                    Vector2 targetLocation = new Vector2((int)position.X, i);
                    if (!(i < 0 | i > 2))
                    {
                        targetList.Add(targetLocation);
                        if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(targetLocation));
                    }
                }
            }
            else if (effects.Contains("V"))
            {
                int x = (int)position.X;
                if (color == "red") x += 1;
                else x -= 1;

                if (!(x < 0 | x > 5))
                {
                    for (int i = (int)position.Y - 1; i <= (int)position.Y + 1; i += 2)
                    {
                        if (!(i < 0 | i > 2))
                        {
                            Vector2 targetLocation = new Vector2(x, i);
                            targetList.Add(targetLocation);
                            if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(targetLocation));
                        }
                    }
                }
                targetList.Add(position);
                if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(position));
            }
            else if (effects.Contains("long"))
            {
                int x = (int)position.X;
                if (color == "red") x += 1;
                else x -= 1;

                if (!(x < 0 | x > 5))
                {
                    targetList.Add(new Vector2(x, (int)position.Y));
                    if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(new Vector2(x, (int)position.Y)));
                }
                targetList.Add(position);

                if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(position));
            }
            else if (effects.Contains("spread"))
            {
                for (int i = (int)position.X - 1; i <= (int)position.X + 1; i++)
                {
                    for (int j = (int)position.Y - 1; j <= (int)position.Y + 1; j++)
                    {
                        Vector2 targetLocation = new Vector2(i, j);
                        if (!(i < 0 | i > 5 | j < 0 | j > 2))
                        {
                            targetList.Add(targetLocation);
                            if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(targetLocation));
                        }
                    }
                }
            }
            else if (effects.Contains("cross"))
            {
                for (int i = (int)position.X - 1; i <= (int)position.X + 1; i += 2)
                {
                    for (int j = (int)position.Y - 1; j <= (int)position.Y + 1; j += 2)
                    {
                        Vector2 targetLocation = new Vector2(i, j);
                        if (!(i < 0 | i > 5 | j < 0 | j > 2))
                        {
                            targetList.Add(targetLocation);
                            if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(targetLocation));
                        }
                    }
                }
                targetList.Add(position);
                if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(position));
            }
            else
            {
                targetList.Add(position);
                if (effectSprite != null) stage.addEffect(effectSprite.Clone(), getLocation(position));
            }

            foreach (Vector2 foo in targetList)
            {
                applyDamage(foo, damage, damageType, effects);
            }
        }

        //Acutally apply the damage
        public void applyDamage(Vector2 position, int damage, List<string> damageType, List<string> effects)
        {
            //Applies damage
            int damReturn = damage;
            Actor target = stage.getActor(position);
            string panel = stage.getPanelType(position);

            //Checks body type
            if (target != null)
            {
                foreach (string d in damageType)
                {
                    damReturn += (int)damageMod(target, d) * damage;
                }
            }

            //Gets damage modifier and effects from damage type
            foreach (string d in damageType)
            {
                damReturn += (int)stage.damageMod(position, d) * damage;
                Game.damageTypes[d].onHit(stage, position, panel);
            }

            if (panel == "Holy") damReturn = damReturn / 2;

            if (target != null) if ((target.Guard) & !(effects.Contains("Break"))) damReturn = 0;

            //no friendly fire, but panel can do damage
            if (target != null)
            {
                if (target.color != color || effects.Contains("Panel"))
                {
                    target.HP -= damReturn;
                    if (target.HP < 1) target.Delete();
                }
            }
        }

        public float damageMod(Actor target, string damageType)
        {
            float mod = 0;

            if (Game.damageTypes[damageType].damageMod == null) return mod;

            foreach (KeyValuePair<String, float> entry in Game.damageTypes[damageType].damageMod)
                if (target.Body[entry.Key]) mod += entry.Value;

            return mod;
        }
    }
}