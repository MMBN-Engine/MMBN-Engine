using System;
using System.Collections.Generic;
using System.Linq;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Megaman.Actors.Navis;
using Megaman.Chips;
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D stageTiles;
        KeyboardState currentKeyboard;
        KeyboardState oldKeyboard;
        Stage stage;
        Custom custom;
        
        Navi navi;
        List<Virus> virus;
        
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 240;
            graphics.PreferredBackBufferHeight = 160;
            Window.Title = "MegaMan Battle Network";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            currentKeyboard = new KeyboardState();
            oldKeyboard = new KeyboardState();
            stage = new Stage();
            custom = new Custom();
            
            navi = new MegaMan(100);

            virus = new List<Virus>();
            virus.Add(new Mettaur());
            virus.Add(new Mettaur2());
            virus.Add(new MettaurΩ());

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {            
            navi.Initialize(Content, new Vector2(1,1), stage);            
            
            virus[0].Initialize(Content, new Vector2(4, 1), stage);
            virus[1].Initialize(Content, new Vector2(3, 0), stage);
            virus[2].Initialize(Content, new Vector2(5, 2), stage);

            foreach (Virus foo in virus) foo.AiInitialize();
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            newGame();

            battleStart();

            //Content.Load<Texture2D>("sprites/navi/megaman/aqua").saveTexture("aqua2");
            //List<Color> Null = Content.Load<Texture2D>("sprites/navi/megaman/null").getPalette();
            //List<Color> Elec = Content.Load<Texture2D>("sprites/navi/megaman/elec").getPalette();
            //Content.Load<Texture2D>("sprites/navi/megaman/team").changeColor(Elec,Null).saveImage("team");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            oldKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            // Development keys, will be removed
            //if (JustPressed(Keys.N)) stage.setStage("Null");
            //if (JustPressed(Keys.C)) stage.setStage("Cracked");
            //if (JustPressed(Keys.B)) stage.setStage("Broken");
            //if (JustPressed(Keys.G)) stage.setStage("Grass");
            //if (JustPressed(Keys.S)) stage.setStage("Sand");
            //if (JustPressed(Keys.M)) stage.setStage("Metal");
            //if (JustPressed(Keys.I)) stage.setStage("Ice");
            //if (JustPressed(Keys.P)) stage.setStage("Swamp");
            //if (JustPressed(Keys.L)) stage.setStage("Lava");
            //if (JustPressed(Keys.H)) stage.setStage("Holy");
            //if (JustPressed(Keys.T)) stage.setStage("Hole");
            //if (JustPressed(Keys.Q)) navi.AirShoe = true;
            //if (JustPressed(Keys.W)) navi.AirShoe = false;
            if (JustPressed(Keys.E)) navi.styleChange("Elec","Bug");
            if (JustPressed(Keys.A)) navi.styleChange("Aqua","Bug");
            if (JustPressed(Keys.H)) navi.styleChange("Heat","Bug");
            if (JustPressed(Keys.W)) navi.styleChange("Wood","Bug");
            if (JustPressed(Keys.N)) navi.styleChange("Null","Bug");            
            
            //if (JustPressed(Keys.U)) attack.doDamage(navi.position, 10, "Fire", stage);

            if (JustPressed(Keys.A)) navi.Heal(navi, -1);

            if (JustPressed(Keys.Space) && custom.open == true) custom.Close();
            if (JustPressed(Keys.Space) && custom.closed == true && custom.custom == custom.customMax) 
                custom.Open();

            //Only run these during battle
            if (custom.closed)
            {
                if (JustPressed(Keys.Left)) navi.Move(new Vector2(-1, 0));
                if (JustPressed(Keys.Right)) navi.Move(new Vector2(1, 0));
                if (JustPressed(Keys.Up)) navi.Move(new Vector2(0, -1));
                if (JustPressed(Keys.Down)) navi.Move(new Vector2(0, 1));

                if (JustPressed(Keys.Z)) navi.Buster(); 
                
                //Does a charged attack                
                if (IsHeld(Keys.Z) && !(navi.charged | navi.isAttacking)) navi.isCharging = true;
                
                if (IsReleased(Keys.Z))
                {
                    if (navi.charged) navi.chargedAttack();
                    navi.isCharging = false;
                    navi.charged = false;
                }

                navi.Update(gameTime);
                foreach (Virus foo in virus) foo.Update(gameTime);

                stage.Update(gameTime);
            }
            
            custom.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Draws the custom bar and hp windom (maybe emotion window later)
            //ONly the background should be drawed before this
            custom.drawBars(spriteBatch);
            
            stage.Draw(spriteBatch);
            navi.Draw(spriteBatch);

            foreach (Virus foo in virus)
            {
                //Will need this when I add stunned etc. effects
                //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                foo.Draw(spriteBatch);
            }

            foreach (Virus foo in virus) foo.DrawHP(spriteBatch);
            
            //Draw this last
            custom.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Use this to determine if the key is being held
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>returns true if the key is held.</returns>
        public bool IsHeld(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Use this to check if the key was just released
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Returns true if the key was released</returns>
        public bool IsReleased(Keys key)
        {
            if (currentKeyboard.IsKeyUp(key) && oldKeyboard.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Use this to see if the key was just pressed
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Returns true if the key was just pressed</returns>
        public bool JustPressed(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void newGame()
        {            
            navi.chipFolder[0] = new Cannon("A");
            navi.chipFolder[1] = new Cannon("A");
            navi.chipFolder[2] = new Cannon("B");
            navi.chipFolder[3] = new Cannon("B");
            navi.chipFolder[4] = new Shotgun("J");
            navi.chipFolder[5] = new Shotgun("J");
            navi.chipFolder[6] = new Shotgun("J");
            navi.chipFolder[7] = new Vgun("D");
            navi.chipFolder[8] = new Vgun("D");
            navi.chipFolder[9] = new Vgun("D");
            navi.chipFolder[10] = new Sidegun("S");
            navi.chipFolder[11] = new Airshot1("*");
            navi.chipFolder[12] = new Airshot1("*");
            navi.chipFolder[13] = new Airshot1("*");
            navi.chipFolder[14] = new Minibomb("B");
            navi.chipFolder[15] = new Minibomb("B");
            navi.chipFolder[16] = new Minibomb("S");
            navi.chipFolder[17] = new Sword("L");
            navi.chipFolder[18] = new Sword("L");
            navi.chipFolder[19] = new Sword("L");
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

            foreach (Chip chip in navi.chipFolder) chip.Initialize(Content);
        }

        public void battleStart()
        {
            navi.customFolder = navi.chipFolder.ToList();
            navi.customFolder.Shuffle();

            custom.Initialize(Content, navi);
            stage.Initialize(Content);
        }

    }
}