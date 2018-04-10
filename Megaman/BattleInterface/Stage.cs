using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Megaman.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman
{
    public class Stage
    {
        public Dictionary<string, PanelType> panelDef;
        public Dictionary<string, Texture2D> textures;

        public string[,] PanelType;
        public string[,] Area;
        public int spriteWidth;
        public int[] spriteHeight;
         
        private int width;
        private int height;

        public Actor[,] actorArray;

        public Texture2D texture;

        public struct effectsList
        {
            public List<Animation> effect;
            public List<Vector2> location;
            public effectsList(List<Animation> effect1, List<Vector2> location1)
            {
                effect = effect1;
                location = location1;
            }
        }
        public List<Projectile>[,] projectileList;
        public effectsList stageEffects;

        public Stage() 
        {
            panelDef = Game.panelTypes;

            stageEffects = new effectsList(new List<Animation>(), new List<Vector2>());
            projectileList = new List<Projectile>[width, height];

            width = 6;
            height = 3;

            spriteWidth = 40;
            spriteHeight = new int[height];
            spriteHeight[0] = 24;
            spriteHeight[1] = 23;
            spriteHeight[2] = 33;
    
            PanelType = new string[width,height];
            Area = new string[width, height];
            actorArray = new Actor[width, height];

            //Set up projectile list and initialize each list
            projectileList = new List<Projectile>[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    projectileList[i, j] = new List<Projectile>();

            for (int i = 0;i < width;++i)
                for(int j = 0; j <height;j++)
                    PanelType[i,j] = "Null";
            
            for (int i = 0;i < width;++i)
                for(int j = 0; j <height;j++)
                    Area[i,j] = "blue";

            for (int i = 0; i < width/2; ++i)
                for (int j = 0; j < height; j++)
                    Area[i, j] = "red";
            
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; j++)
                    actorArray[i, j] = null;

        }

        public void Initialize(ContentManager content)
        {
            textures = new Dictionary<string, Texture2D>();

            string path = "gfx/panels/";

            List<String> fileArray = Scripting.getFilesFromFolder(path);

            foreach (string t in fileArray)
                textures.Add(t, Scripting.loadImage(path + t + ".png"));
        }

        public void Update(GameTime gameTime)
        {
            for (int i = stageEffects.effect.Count - 1; i >= 0; i--)
            {
                if (!stageEffects.effect[i].active)
                {
                    stageEffects.effect.RemoveAt(i);
                    stageEffects.location.RemoveAt(i);
                }
            }

            foreach (List<Projectile> proj in projectileList)
            {
                for (int i = proj.Count - 1; i >= 0; i--)
                {
                    if (!proj[i].isActive)
                    {
                        proj.RemoveAt(i);
                    }
                }
            }

            foreach (Animation foo in stageEffects.effect)
                foo.Update(gameTime);
            //This needs to be a for loop, since projectiles may clone during run
            //And we can't do foreach of the size changes
            foreach (List<Projectile> proj in projectileList)
                for (int i = 0; i < proj.Count; i++) proj[i].Update(gameTime);
        }

        public void setStage(string panelType)
        {
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; j++)
                    setPanel(new Vector2(i, j), panelType);
        }

        public float damageMod(Vector2 position, string damageType)
        {
            string panel = getPanelType(position);

            if (panelDef[panel].damageMod.ContainsKey(damageType))
                return panelDef[panel].damageMod[damageType];
            else return 0;
        }

        public void Draw(SpriteBatch spriteBatch, float resolution)
        {
            int stagePositionX = 0;
            int stagePositionY = 72;
            int stageY;

            for (int i = 0; i < Area.GetLength(0); i++)
            {
                stageY = 0;
                for (int j = 0; j < Area.GetLength(1); j++)
                {
                    //Finds the panel from the texture file
                    Rectangle panel = new Rectangle(
                        0, stageY, spriteWidth, spriteHeight[j]);

                    spriteBatch.Draw(textures[Area[i,j]],
                        new Vector2(stagePositionX + i * spriteWidth, stagePositionY + stageY) * resolution,
                        sourceRectangle: panel, scale: new Vector2(1, 1) * resolution, color: Color.White);
                    spriteBatch.Draw(textures[PanelType[i, j]],
                        new Vector2(stagePositionX + i * spriteWidth, stagePositionY + stageY) * resolution,
                        sourceRectangle: panel, scale: new Vector2(1, 1) * resolution, color: Color.White);

                    stageY += spriteHeight[j];
                }
            }
        }

        public virtual void addEffect(Animation animation, Vector2 location)
        {
            stageEffects.effect.Add(animation);
            stageEffects.location.Add(location);
        }

        public Actor getActor(Vector2 position)
        {
            return actorArray[(int)position.X, (int)position.Y];
        }

        public string getPanelType(Vector2 position)
        {
            return PanelType[(int)position.X, (int)position.Y];
        }

        public void setPanel(Vector2 position, string panelType)
        {
            PanelType[(int)position.X, (int)position.Y] = panelType;
            if (getActor(position) != null)
                getActor(position).onStep(getActor(position), panelType);
        }

        public virtual void addProjectile(Projectile projectile, Vector2 location)
        {
            projectileList[(int)location.X, (int)location.Y].Add(projectile);
        }
    }
}
