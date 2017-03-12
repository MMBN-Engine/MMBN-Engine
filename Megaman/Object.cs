﻿using System;
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
    class Object
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
            effectSprite = new Animation(); //Effects such as explosions etc.

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
        public void doDamage(Vector2 position, int damage, string damageType, List<string> effects, Animation effectSprite)
        {
            this.effectSprite = effectSprite;
            List<Actor> targetList = new List<Actor>();

            if (effects.Contains("wide"))
            {
                for (int i = (int)position.Y - 1; i <= (int)position.Y + 1; i++)
                {
                    Vector2 targetLocation = new Vector2((int)position.X, i);
                    if (!(i < 0 | i > 2)) targetList.Add(getTarget(targetLocation));
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
                            targetList.Add(getTarget(targetLocation));
                        }
                    }
                }
                targetList.Add(getTarget(position));
            }
            else if (effects.Contains("long"))
            {
                int x = (int)position.X;
                if (color == "red") x += 1;
                else x -= 1;

                if (!(x < 0 | x > 5)) targetList.Add(getTarget(new Vector2(x, (int)position.Y)));
                targetList.Add(getTarget(position));
            }
            else if (effects.Contains("spread"))
            {
                for (int i = (int)position.X - 1; i <= (int)position.X + 1; i++)
                {
                    for (int j = (int)position.Y - 1; j <= (int)position.Y + 1; j++)
                    {
                        Vector2 targetLocation = new Vector2(i, j);
                        if (!(i < 0 | i > 5 | j < 0 | j > 2)) targetList.Add(getTarget(targetLocation));
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
                        if (!(i < 0 | i > 5 | j < 0 | j > 2)) targetList.Add(getTarget(targetLocation));
                    }
                }
            }
            else targetList.Add(getTarget(position));

            //No friendly fire
            foreach (Actor foo in targetList)
            {
                if (foo != null) if (foo.color != color) applyDamage(foo, damage, damageType, effects);
            }
        }

        //Get the target and apply sprites
        public Actor getTarget(Vector2 position)
        {
            if (effectSprite != null) stage.addEffect(effectSprite, getLocation(position));
            return stage.actorArray[(int)position.X, (int)position.Y];
        }

        //Acutally apply the damage
        public void applyDamage(Actor target, int damage, string damageType, List<string> effects)
        {
            //Applies damage
            int damReturn = damage;
            string panel = stage.PanelType[(int)target.position.X, (int)target.position.Y];

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

            if ((target.Guard) & !(effects.Contains("Break"))) damReturn = 0;

            target.HP -= damReturn;
            if (target.HP < 1) target.Delete();
        }
    }
}