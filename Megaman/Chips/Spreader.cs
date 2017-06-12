using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman.Chips
{
    abstract class Spread : standardChip
    {
        public Spread(string code) : base(code)
        {
            element = "null";
            attack = 30;
        }
        public override void Use(Actor actor)
        {
            base.Use(actor);
            actor.attackTypes["Spreader"].Action(actor, damage, effect);
        }
    }

    class Shotgun : Spread
    {
        public Shotgun(string code) : base(code)
        {
            name = "ShotGun";
            MB = 8;
            effect = "long";
        }
    }

    class Vgun : Spread
    {
        public Vgun(string code) : base(code)
        {
            name = "V-Gun";
            MB = 8;
            effect = "V";
        }
    }

    class Sidegun : Spread
    {
        public Sidegun(string code) : base(code)
        {
            name = "SideGun";
            MB = 8;
            effect = "wide";
        }
    }

    class Crossgun : Spread
    {
        public Crossgun(string code) : base(code)
        {
            name = "CrossGun";
            MB = 12;
            effect = "cross";
        }
    }

    class Spreader : Spread
    {
        public Spreader(string code) : base(code)
        {
            name = "Spreader";
            MB = 16;
            effect = "spread";
        }
    }

    abstract class aquaSpread : standardChip
    {
        public aquaSpread(string code) : base(code)
        {
            element = "aqua";
            attack = 60;
        }
        public override void Use(Actor actor)
        {
            base.Use(actor);
            actor.attackTypes["Bubbler"].Action(actor, damage, effect);
        }
    }

    class Bubbler : aquaSpread
    {
        public Bubbler(string code) : base(code)
        {
            name = "Bubbler";
            MB = 14;
            effect = "long";
        }
    }

    class Bubblev : aquaSpread
    {
        public Bubblev(string code) : base(code)
        {
            name = "Bub-V";
            MB = 22;
            effect = "V";
        }
    }

    class Bubbleside : aquaSpread
    {
        public Bubbleside(string code) : base(code)
        {
            name = "BublSide";
            MB = 30;
            effect = "wide";
        }
    }

    class Bubblecross : aquaSpread
    {
        public Bubblecross(string code) : base(code)
        {
            name = "BubCross";
            MB = 34;
            effect = "cross";
        }
    }

    class Bubblespread : aquaSpread
    {
        public Bubblespread(string code) : base(code)
        {
            name = "BubSprd";
            MB = 38;
            effect = "spread";
        }
    }

    abstract class heatSpread : standardChip
    {
        public heatSpread(string code) : base(code)
        {
            element = "fire";
            attack = 40;
        }
        public override void Use(Actor actor)
        {
            base.Use(actor);
            actor.attackTypes["Heater"].Action(actor, damage, effect);
        }
    }

    class Heatshot : heatSpread
    {
        public Heatshot(string code) : base(code)
        {
            name = "HeatShot";
            MB = 16;
            effect = "long";
        }
    }

    class Heatv : heatSpread
    {
        public Heatv(string code) : base(code)
        {
            name = "Heat-V";
            MB = 24;
            effect = "V";
        }
    }

    class Heatside : Spread
    {
        public Heatside(string code) : base(code)
        {
            name = "HeatSide";
            MB = 32;
            effect = "wide";
        }
    }

    class Heatcross : aquaSpread
    {
        public Heatcross(string code) : base(code)
        {
            name = "HeatCros";
            MB = 36;
            effect = "cross";
        }
    }

    class Heatspread : heatSpread
    {
        public Heatspread(string code) : base(code)
        {
            name = "HeatSprd";
            MB = 40;
            effect = "spread";
        }
    }
}
