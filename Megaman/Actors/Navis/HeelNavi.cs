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
    
    class HeelNavi : Navi
    {
        public HeelNavi(int HP, Area area) : base(HP, area)
        {
            armLocation = new Vector2(30, -32);
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            staticSprite.Initialize(content.Load<Texture2D>("sprites/navi/heelnavi/navi"),
                new Vector2(0, 46), 34, 0, true);

            moveSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/move"),
                new Vector2(0, 46), 35, 20, false);

            busterSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/buster"),
                new Vector2(0, 3), 41, 30, false);                      
        }
    }
        
}
