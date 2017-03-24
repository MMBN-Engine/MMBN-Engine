using System;
using System.Collections.Generic;
using System.Linq;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Megaman.Actors.Navis;
using Megaman.Chips;
using Megaman.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CustomExtensions;
using System.IO;

namespace Megaman
{
    //This will keep debug commands, so we can push changes to the main game without 
    public partial class Game : Microsoft.Xna.Framework.Game
    {
        public void debugDraw()
        {
            ACDC1.Draw(spriteBatch, screenSize);
        }

        public void debugCommands()
        {
            // Development keys, will be removed
            //if (JustPressed(Keys.N)) stage.setStage("Null");
            //if (JustPressed(Keys.C)) stage.setStage("Cracked");
            //if (JustPressed(Keys.B)) stage.setStage("Broken");
            if (JustPressed(Keys.G)) stage.setStage("Grass");
            //if (JustPressed(Keys.S)) stage.setStage("Sand");
            //if (JustPressed(Keys.M)) stage.setStage("Metal");
            //if (JustPressed(Keys.I)) stage.setStage("Ice");
            //if (JustPressed(Keys.P)) stage.setStage("Swamp");
            if (JustPressed(Keys.L)) stage.setStage("Lava");
            //if (JustPressed(Keys.H)) stage.setStage("Holy");
            //if (JustPressed(Keys.T)) stage.setStage("Hole");
            //if (JustPressed(Keys.Q)) navi.AirShoe = true;
            //if (JustPressed(Keys.W)) navi.AirShoe = false;
            if (JustPressed(Keys.E)) navi.styleChange("elec", "Bug");
            if (JustPressed(Keys.A)) navi.styleChange("aqua", "Bug");
            if (JustPressed(Keys.H)) navi.styleChange("fire", "Bug");
            if (JustPressed(Keys.W)) navi.styleChange("wood", "Bug");
            if (JustPressed(Keys.N)) navi.styleChange("Null", "Bug");

            //if (JustPressed(Keys.U)) attack.doDamage(navi.position, 10, "fire", stage);

            if (JustPressed(Keys.C)) navi.attackTypes.Recover(navi, 10);
        }

        public void debugFolder()
        {
            navi.chipFolder[0] = new Heatshot("A");
            navi.chipFolder[1] = new Heatshot("A");
            navi.chipFolder[2] = new Heatside("B");
            navi.chipFolder[3] = new Heatside("B");
            navi.chipFolder[4] = new Heatv("J");
            navi.chipFolder[5] = new Heatv("J");
            navi.chipFolder[6] = new Heatspread("J");
            navi.chipFolder[7] = new Heatspread("D");
            navi.chipFolder[8] = new Heatcross("D");
            navi.chipFolder[9] = new Heatcross("D");
            navi.chipFolder[10] = new Bubblecross("S");
            navi.chipFolder[11] = new Bubblecross("*");
            navi.chipFolder[12] = new Bubbler("*");
            navi.chipFolder[13] = new Bubbler("*");
            navi.chipFolder[14] = new Bubbler("B");
            navi.chipFolder[15] = new Bubbler("B");
            navi.chipFolder[16] = new Bubbler("S");
            navi.chipFolder[17] = new Bubbler("L");
            navi.chipFolder[18] = new Bubbler("L");
            navi.chipFolder[19] = new Bubbler("L");
            navi.chipFolder[20] = new Widesword("L");
            navi.chipFolder[21] = new Panelout1("B");
            navi.chipFolder[22] = new Panelout1("B");
            navi.chipFolder[23] = new Areagrab("L");
            navi.chipFolder[24] = new Recover10("A");
            navi.chipFolder[25] = new Recover10("A");
            navi.chipFolder[26] = new Recover10("L");
            navi.chipFolder[27] = new Recover10("L");
            navi.chipFolder[28] = new Attack10("*");
            navi.chipFolder[29] = new Attack10("*");
        }
    }
}