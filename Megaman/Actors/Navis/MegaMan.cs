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
        public MegaMan() : base()
        {
            armLocation = new Vector2(30, -32);

            origin = new Vector2(3, 53);
            spriteWidth = 66;
            folder = "sprites/navi/megaman/";

            name = "MegaMan";
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            //This needs to be last, textures have to be initialized first so they are assigned correctly
            base.Initialize(content, position, stage);

            //Need to redo attack sprites!, wrong palette!!!!       
        }

        public override void styleChange(String Element, String Style)
        {
            //palette2 = (List<Color>)typeof(MegaMan).GetField(Element).GetValue(this);

            base.styleChange(Element, Style);

        }
    }           
}
