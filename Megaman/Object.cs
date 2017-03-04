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
    class Object
    {
        public Vector2 position;
        public string color;
        public int[] panelHeight;

        public Animation activeSprite;

        public Stage stage;
        
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

            activeSprite = new Animation(); //This is the actual sprite we draw

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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            int stageWidth = 40;
            int heightSum = 72;
            for (int i = 0; i < position.Y; i++)
                heightSum += panelHeight[i];
            Vector2 test = new Vector2(1, 2);

            //offsets due to the position of the stage
            Vector2 offSet = new Vector2(3, 16);

            //finds the location to draw it in
            location = new Vector2(offSet.X + position.X * stageWidth, offSet.Y + heightSum);
            //shif with the draw offset
            location = Vector2.Add(location, drawOffset);

            activeSprite.Draw(spriteBatch, location);
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
            if (target.HP < 10) stage.actorArray[(int)position.X, (int)position.Y] = null;
        }
    }
}