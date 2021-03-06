﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Megaman.Actors;
using Megaman.Actors.Navis;

namespace Megaman.Chips
{
    public class Chip
    {
        public string code;
        public string type;
        public string name;
        public string element;
        public int attack, damageMod, damageMult;
        public int MB;
        public string description;
        public Texture2D image;
        public Texture2D icon;
        public Texture2D background;

        public Action<Actor, Chip> onUse;
        public Dictionary<string, int> values;

        public int damage;   //damage that is actually applied
        public string keyWord; //keyword for various uses

        public string effect;

        public bool selected;

        public List<string> effects;

        public Chip() 
        {
            onUse = (Action<Actor, Chip>)delegate (Actor actor, Chip chip) { };
        }

        public void Initialize(ContentManager content)
        {
            damageMult = 1;
            image = Scripting.loadImage("gfx/chips/images/" + name + ".png");
            icon = Scripting.loadImage("gfx/chips/icons/" + name + ".png");
            //background = content.Load<Texture2D>("sprites/chips/" + type);
        }

        public virtual void Use(Actor actor)
        {
            damage = (attack + damageMod) * damageMult;

            onUse(actor, this);
        }

        public void attackPlus(int i, Navi navi)
        {
            //check to see if chip before exists/ has attack then add i
        }

        public Chip setCode(string code)
        {
            Chip temp = Clone();
            temp.code = code;
            return temp;
        }

        public Chip Clone()
        {
            return (Chip)this.MemberwiseClone();
        }

        public void addEffect(string effect)
        {
            //check to see if chip before exists/ has attack then add effects
        }
    }
}
