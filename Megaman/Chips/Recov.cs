using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman.Chips
{
    class Recover : standardChip
    {
        public int recov;
        public Recover(string code) : base(code)
        {
            element = "null";
        }
        public override void Use(Actor actor)
        {
            base.Use(actor);
            actor.attackTypes.Recover(actor, recov);
        }
    }

    class Recover10 : Recover
    {
        public Recover10(string code) : base(code)
        {
            name = "Recov10";
            recov = 10;
            MB = 5;
        }
    }

    class Recover30 : Recover
    {
        public Recover30(string code) : base(code)
        {
            name = "Recov30";
            recov = 30;
            MB = 8;
        }
    }

    class Recover50 : Recover
    {
        public Recover50(string code) : base(code)
        {
            name = "Recov50";
            recov = 30;
            MB = 14;
        }
    }

    class Recover80 : Recover
    {
        public Recover80(string code) : base(code)
        {
            name = "Recov80";
            recov = 80;
            MB = 20;
        }
    }

    class Recover120 : Recover
    {
        public Recover120(string code) : base(code)
        {
            name = "Recov120";
            recov = 120;
            MB = 35;
        }
    }

    class Recover150 : Recover
    {
        public Recover150(string code) : base(code)
        {
            name = "Recov150";
            recov = 150;
            MB = 50;
        }
    }

    class Recover200 : Recover
    {
        public Recover200(string code) : base(code)
        {
            name = "Recov200";
            recov = 200;
            MB = 65;
        }
    }

    class Recover300 : Recover
    {
        public Recover300(string code) : base(code)
        {
            name = "Recov300";
            recov = 300;
            MB = 80;
        }
    }
}
