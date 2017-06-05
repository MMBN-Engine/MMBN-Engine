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
    public class DamageType
    {
        public string name;
        public bool defaultDef;
        public Dictionary<string, float> damageMod;
        public Action<Stage, Vector2, string> onHit;

        public DamageType()
        {
            onHit = delegate (Stage stage, Vector2 position, string panel) { };
        }
    }
}
