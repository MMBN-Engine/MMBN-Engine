using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman.Chips
{
    class Spread : standardChip
    {
        public string effect;
        public Spread(string code) : base(code)
        {
        }
        public override void Use(Actor actor)
        {
            base.Use(actor);
            actor.attackTypes.Spreader(actor, (attack + damageMod) * damageMult, element, effect);
        }
    }

    class Shotgun : Spread
    {
        public Shotgun(string code) : base(code)
        {
            name = "ShotGun";
            element = "null";
            attack = 30;
            MB = 8;
            effect = "long";
        }
    }

    class Vgun : Spread
    {
        public Vgun(string code) : base(code)
        {
            name = "V-Gun";
            element = "null";
            attack = 30;
            MB = 8;
            effect = "V";
        }
    }

    class Sidegun : Spread
    {
        public Sidegun(string code) : base(code)
        {
            name = "SideGun";
            element = "null";
            attack = 30;
            MB = 8;
            effect = "wide";
        }
    }
}
