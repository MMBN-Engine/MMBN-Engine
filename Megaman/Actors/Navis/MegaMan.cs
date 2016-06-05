using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman.Actors.Navis
{
    class MegaMan : Navi
    {
        public MegaMan(int HP) : base(HP)
        {
            armLocation = new Vector2(30, -32);
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);
            
            staticSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/navi"),
                new Vector2(0, 46), 34, 0, true);

            moveSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/move"),
                new Vector2(0, 46), 35, 20, false);

            attackSprites.Add(new Animation());
            attackSprites.Add(new Animation());
            attackSprites[0].Initialize(content.Load<Texture2D>("sprites/navi/megaman/shoot"),
                new Vector2(0, 46), 35, 30, false);
            attackSprites[1].Initialize(content.Load<Texture2D>("sprites/navi/megaman/sword"),
                new Vector2(3, 53), 66, 30, false);

            busterSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/buster"),
                new Vector2(0, 3), 41, 30, false);

            Null = content.Load<Texture2D>("sprites/navi/megaman/null").getPallete();
            Aqua = content.Load<Texture2D>("sprites/navi/megaman/aqua").getPallete();
            Elec = content.Load<Texture2D>("sprites/navi/megaman/elec").getPallete();
            Heat = content.Load<Texture2D>("sprites/navi/megaman/heat").getPallete();
            Wood = content.Load<Texture2D>("sprites/navi/megaman/wood").getPallete();
            pallete1 = Null;

            //Need to redo attack sprites!, wrong pallete!!!!          
        }

        public void chargedAttack()
        {
            Shoot(busterSprite, Attack * 10, "Null", new List<string>(), Gun, null);
        }

        public void styleChange(String Element)
        {
            AquaBody = false;
            ElecBody = false;
            FireBody = false;
            WoodBody = false;

            if (Element == "Aqua")
            {
                AquaBody = true;
                staticSprite.map = staticSprite.map.changeColor(pallete1, Aqua);
                moveSprite.map = moveSprite.map.changeColor(pallete1, Aqua);
                busterSprite.map = busterSprite.map.changeColor(pallete1, Aqua);
                foreach (Animation foo in attackSprites) foo.map = foo.map.changeColor(pallete1, Aqua);
                pallete1 = Aqua;
            }
            
            if (Element == "Elec")
            {
                ElecBody = true;
                staticSprite.map = staticSprite.map.changeColor(pallete1, Elec);
                moveSprite.map = moveSprite.map.changeColor(pallete1, Elec); 
                busterSprite.map = busterSprite.map.changeColor(pallete1, Elec);
                foreach (Animation foo in attackSprites) foo.map = foo.map.changeColor(pallete1, Elec);
                pallete1 = Elec;
            }

            if (Element == "Wood")
            {
                WoodBody = true;
                staticSprite.map = staticSprite.map.changeColor(pallete1, Wood);
                moveSprite.map = moveSprite.map.changeColor(pallete1, Wood);
                busterSprite.map = busterSprite.map.changeColor(pallete1, Wood);
                foreach (Animation foo in attackSprites) foo.map = foo.map.changeColor(pallete1, Wood);
                pallete1 = Wood;
            }

            if (Element == "Heat")
            {
                FireBody = true;
                staticSprite.map = staticSprite.map.changeColor(pallete1, Heat);
                moveSprite.map = moveSprite.map.changeColor(pallete1, Heat);                
                busterSprite.map = busterSprite.map.changeColor(pallete1, Heat);
                foreach (Animation foo in attackSprites) foo.map = foo.map.changeColor(pallete1, Heat);
                pallete1 = Heat;
            }

            if (Element == "Null")
            {
                staticSprite.map = staticSprite.map.changeColor(pallete1, Null);
                moveSprite.map = moveSprite.map.changeColor(pallete1, Null);                
                busterSprite.map = busterSprite.map.changeColor(pallete1, Null);
                foreach (Animation foo in attackSprites) foo.map = foo.map.changeColor(pallete1, Null);
                pallete1 = Null;
            }
        }
    }           
}
