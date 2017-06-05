using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Megaman.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Megaman
{
    public class PanelType
    {
        public string name;
        public int index;
        public Action<Actor, string> onStep;
        public Dictionary<string, float> damageMod;

        public PanelType()
        {
            damageMod = new Dictionary<string, float>();

            onStep = delegate (Actor actor, string panel) { };
        }
    }
}
