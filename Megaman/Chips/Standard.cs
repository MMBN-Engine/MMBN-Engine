using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman.Chips
{
    class standardChip : Chip
    {
        public standardChip(string code) : base(code)
        {
            type = "standard";
        }
    }

    class Cannon : standardChip
    {
        public Cannon(string code) : base(code)
        {
            name = "Cannon";
            element = "null";
            attack = 40;
            MB = 12;
        }
    }

    class Airshot1 : standardChip
    {
        public Airshot1(string code) : base(code)
        {
            name = "AirShot1";
            element = "wind";
            attack = 20;
            MB = 4;
        }
    }

    class Minibomb : standardChip
    {
        public Minibomb(string code) : base(code)
        {
            name = "miniBomb";
            element = "null";
            attack = 50;
            MB = 6;
        }
    }

    class Sword : standardChip
    {
        public Sword(string code) : base(code)
        {
            name = "Sword";
            element = "null";
            attack = 80;
            MB = 10;
        }
    }

    class Widesword : standardChip
    {
        public Widesword(string code): base(code)
        {
            name = "WideSwrd";
            element = "null";
            attack = 80;
            MB = 16;
        }
    }

    class Panelout1 : standardChip
    {
        public Panelout1(string code) : base(code)
        {
            name = "PanlOut1";
            element = "null";
            MB = 4;
        }
    }

    class Areagrab : standardChip
    {
        public Areagrab(string code) : base(code)
        {
            name = "AreaGrab";
            element = "null";
            MB = 15;
        }
    }

    class Attack10 : standardChip
    {
        public Attack10(string code) : base(code)
        {
            name = "Atk+10";
            element = "null";
            MB = 4;
        }
    }
}
