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
            //ACDC1.Draw(spriteBatch, screenSize);
        }

        public void debugCommands()
        {
            // Development keys, will be removed
            if (JustPressed(Keys.N)) stage.setStage("Null");
            if (JustPressed(Keys.C)) stage.setStage("Cracked");
            if (JustPressed(Keys.E)) stage.setStage("Broken");
            if (JustPressed(Keys.G)) stage.setStage("Grass");
            if (JustPressed(Keys.S)) stage.setStage("Sand");
            if (JustPressed(Keys.M)) stage.setStage("Metal");
            if (JustPressed(Keys.I)) stage.setStage("Ice");
            if (JustPressed(Keys.P)) stage.setStage("Swamp");
            if (JustPressed(Keys.L)) stage.setStage("Lava");
            if (JustPressed(Keys.H)) stage.setStage("Holy");
            if (JustPressed(Keys.T)) stage.setStage("Hole");
            //if (JustPressed(Keys.Q)) navi.AirShoe = true;
            //if (JustPressed(Keys.W)) navi.AirShoe = false;
            if (JustPressed(Keys.E)) navi.styleChange("Elec", "Bug");
            if (JustPressed(Keys.A)) navi.styleChange("Aqua", "Bug");
            if (JustPressed(Keys.H)) navi.styleChange("Heat", "Bug");
            if (JustPressed(Keys.W)) navi.styleChange("Wood", "Bug");
            if (JustPressed(Keys.N)) navi.styleChange("Null", "Bug");

            if (JustPressed(Keys.B)) battleStart();

            //if (JustPressed(Keys.U)) attack.doDamage(navi.position, 10, "fire", stage);

            if (JustPressed(Keys.C)) navi.attackTypes["Recover"].Action(navi, 10);
        }

        public void debugContent()
        {
            //Content.Load<Texture2D>("sprites/navi/megaman/aqua").saveTexture("aqua2");
            //List<Color> Null = Content.Load<Texture2D>("sprites/navi/megaman/null").getPalette();
            //List<Color> Elec = Content.Load<Texture2D>("sprites/navi/megaman/elec").getPalette();
            //Content.Load<Texture2D>("sprites/navi/megaman/team").changeColor(Elec,Null).saveImage("team");

            //Texture2D test = Scripting.loadImage("gfx/navi/MegaMan/attacks/shoot2.png");
            //Texture2D test2 = Scripting.loadImage("gfx/navi/MegaMan/attacks/test.png");
            //test = test.changeColor(test2.getPalette(), navi.palettes["Null"]);

            //Stream stream = File.OpenWrite("shoot3.png");
            //test.SaveAsPng(stream, test.Width, test.Height);
        }

        public void debugFolder()
        {
            navi.chipFolder[0] = chipsList["HeatShot"].setCode("A");
            navi.chipFolder[1] = chipsList["HeatShot"].setCode("A");
            navi.chipFolder[2] = chipsList["HeatSide"].setCode("B");
            navi.chipFolder[3] = chipsList["HeatSide"].setCode("B");
            navi.chipFolder[4] = chipsList["Heat-V"].setCode("J");
            navi.chipFolder[5] = chipsList["Heat-V"].setCode("J");
            navi.chipFolder[6] = chipsList["HeatSprd"].setCode("D");
            navi.chipFolder[7] = chipsList["HeatSprd"].setCode("D");
            navi.chipFolder[8] = chipsList["HeatCros"].setCode("D");
            navi.chipFolder[9] = chipsList["HeatCros"].setCode("D");
            navi.chipFolder[10] = chipsList["BubCross"].setCode("S");
            navi.chipFolder[11] = chipsList["BubCross"].setCode("@");
            navi.chipFolder[12] = chipsList["Bubbler"].setCode("@");
            navi.chipFolder[13] = chipsList["Bubbler"].setCode("@");
            navi.chipFolder[14] = chipsList["Bubbler"].setCode("B");
            navi.chipFolder[15] = chipsList["Bubbler"].setCode("B");
            navi.chipFolder[16] = chipsList["Bubbler"].setCode("S");
            navi.chipFolder[17] = chipsList["Bubbler"].setCode("L");
            navi.chipFolder[18] = chipsList["Bubbler"].setCode("L");
            navi.chipFolder[19] = chipsList["Bubbler"].setCode("L");
            navi.chipFolder[20] = chipsList["WideSwrd"].setCode("L");;
            navi.chipFolder[21] = chipsList["PanlOut1"].setCode("B");;
            navi.chipFolder[22] = chipsList["PanlOut1"].setCode("B");;
            navi.chipFolder[23] = chipsList["AreaGrab"].setCode("L");;
            navi.chipFolder[24] = chipsList["Recov10"].setCode("A");
            navi.chipFolder[25] = chipsList["Recov10"].setCode("A");
            navi.chipFolder[26] = chipsList["Recov10"].setCode("L");
            navi.chipFolder[27] = chipsList["Recov10"].setCode("L");
            navi.chipFolder[28] = chipsList["Atk+10"].setCode("@");;
            navi.chipFolder[29] = chipsList["Atk+10"].setCode("@");;
        }
    }
}