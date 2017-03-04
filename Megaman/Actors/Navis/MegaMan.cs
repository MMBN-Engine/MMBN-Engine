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
        public List<Color> Aqua, Elec, Heat, Wood, Null;

        public MegaMan(int HP) : base(HP)
        {
            armLocation = new Vector2(30, -32);
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
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

            busterSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/busterGuts"),
                new Vector2(-1, 3), 40, 30, false);
            busterSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/busterShield"),
                new Vector2(0, 3), 41, 30, false);

            Null = content.Load<Texture2D>("sprites/navi/megaman/palettes/null").getPalette();
            Aqua = content.Load<Texture2D>("sprites/navi/megaman/palettes/aqua").getPalette();
            Elec = content.Load<Texture2D>("sprites/navi/megaman/palettes/elec").getPalette();
            Heat = content.Load<Texture2D>("sprites/navi/megaman/palettes/heat").getPalette();
            Wood = content.Load<Texture2D>("sprites/navi/megaman/palettes/wood").getPalette();

            palette1 = Null;

            //This needs to be last, textures have to be initialized first so they are assigned correctly
            base.Initialize(content, position, stage);

            //Need to redo attack sprites!, wrong palette!!!!          
        }

        public override void chargedAttack()
        {
            Shoot(busterSprite, Attack * 10, "Null", new List<string>(), Gun, null);
        }

        public override void styleChange(String Element, String Style)
        {
            AquaBody = false;
            ElecBody = false;
            FireBody = false;
            WoodBody = false;

            if (Element == "Aqua")
            {
                AquaBody = true;
                palette2 = Aqua;
            }
            
            if (Element == "Elec")
            {
                ElecBody = true;
                palette2 = Elec;
            }

            if (Element == "Wood")
            {
                WoodBody = true;
                palette2 = Wood;
            }

            if (Element == "Heat")
            {
                FireBody = true;
                palette2 = Heat;
            }

            if (Element == "Null")
            {
                palette2 = Null;
            }

            staticSprite.map = staticSprite.map.changeColor(palette1, palette2);
            moveSprite.map = moveSprite.map.changeColor(palette1, palette2);
            busterSprite.map = busterSprite.map.changeColor(palette1, palette2);
            foreach (Animation foo in attackSprites) foo.map = foo.map.changeColor(palette1, palette2);
            palette1 = palette2;

        }
    }           
}
