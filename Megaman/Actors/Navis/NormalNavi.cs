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
    
    class NormalNavi : Navi
    {
        public NormalNavi(int HP) : base(HP)
        {
            armLocation = new Vector2(30, -32);
        }

        public override void Initialize(ContentManager content, Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            staticSprite.Initialize(content.Load<Texture2D>("sprites/navi/normalnavi/navi"),
                new Vector2(0, 49), 34, 0, true);

            moveSprite.Initialize(content.Load<Texture2D>("sprites/navi/normalnavi/move"),
                new Vector2(0, 49), 35, 20, false);

            attackSprites.Add(new Animation());
            attackSprites[0].Initialize(content.Load<Texture2D>("sprites/navi/megaman/shoot"),
                new Vector2(0, 46), 35, 30, false);

            busterSprite.Initialize(content.Load<Texture2D>("sprites/navi/megaman/buster"),
                new Vector2(0, 3), 41, 30, false);                      
        }
    }
        
}
