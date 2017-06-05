using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Megaman.Overworld;

namespace Megaman.Actors.Navis
{
    class MegaMan : Navi
    {
        public List<Color> Aqua, Elec, Heat, Wood, Null;

        public MegaMan(int HP, Area area) : base(HP, area)
        {
            armLocation = new Vector2(30, -32);

            origin = new Vector2(3, 53);
            spriteWidth = 66;
            folder = "sprites/navi/megaman/";
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {

            Null = content.Load<Texture2D>("sprites/navi/megaman/palettes/null").getPalette();
            Aqua = content.Load<Texture2D>("sprites/navi/megaman/palettes/aqua").getPalette();
            Elec = content.Load<Texture2D>("sprites/navi/megaman/palettes/elec").getPalette();
            Heat = content.Load<Texture2D>("sprites/navi/megaman/palettes/heat").getPalette();
            Wood = content.Load<Texture2D>("sprites/navi/megaman/palettes/wood").getPalette();

            palette1 = Null;

            //This needs to be last, textures have to be initialized first so they are assigned correctly
            base.Initialize(content, position, stage);

            busterSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/busterGuts"),
                new Vector2(-1, 3), 40, 30, false);
            busterSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/busterShield"),
                new Vector2(0, 3), 41, 30, false);

            standingSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/overworld/standing"),
                new Vector2(11, 36), 22, 0, false);

            //Need to redo attack sprites!, wrong palette!!!!       
        }

        public override void chargedAttack()
        {
            attackTypes.MegaBuster(this, Attack * 10);
        }

        public override void styleChange(String Element, String Style)
        {
            palette2 = (List<Color>)typeof(MegaMan).GetField(Element).GetValue(this);

            base.styleChange(Element, Style);

        }
    }           
}
