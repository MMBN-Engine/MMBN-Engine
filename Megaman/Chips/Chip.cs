using System;
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
    class Chip
    {
        public string code;
        public string type;
        public string name;
        public string element;
        public int attack;
        public int MB;
        public string description;
        public Texture2D image;
        public Texture2D icon;
        public Texture2D background;

        public bool selected;

        public List<string> effects;

        public Chip(string code) 
        {
            this.code = code;
        }

        public void Initialize(ContentManager content)
        {
            image = content.Load<Texture2D>("sprites/chips/images/" + name);
            icon = content.Load<Texture2D>("sprites/chips/icons/" + name);
            //background = content.Load<Texture2D>("sprites/chips/" + type);
        }

        public virtual void Use(Actor actor)
        {

        }

        public void attackPlus(int i, Navi navi)
        {
            //check to see if chip before exists/ has attack then add i
        }

        public void addEffect(string effect)
        {
            //check to see if chip before exists/ has attack then add effects
        }
    }
}
