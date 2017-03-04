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
    class ProtoMan : Navi
    {
        public ProtoMan(AttackList attackTypes, int HP) : base(attackTypes, HP)
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
        }

        public override void chargedAttack()
        {
            Slash(busterSprite, Attack * 10, "Null", new List<string>(), Sword, null);
        }        
    }           
}
