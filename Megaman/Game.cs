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

        Song song;

        public float screenSize;

        AttackList attackTypes;
        
        Navi navi;
        List<Virus> virus;
        
        public Game()
        {
            screenSize = 2;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int) (240 * screenSize);
            graphics.PreferredBackBufferHeight = (int) (160 * screenSize);
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
            attackTypes = new AttackList();

            currentKeyboard = new KeyboardState();
            oldKeyboard = new KeyboardState();
            stage = new Stage();
            custom = new Custom();
            
            navi = new MegaMan(attackTypes, 100);

            virus = new List<Virus>();
            virus.Add(new Mettaur(attackTypes));
            virus.Add(new Mettaur2(attackTypes));
            virus.Add(new MettaurΩ(attackTypes));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {            
            navi.Initialize(Content, new Vector2(1,1), stage);

            attackTypes.Initialize(Content);     
            
            virus[0].Initialize(Content, new Vector2(4, 1), stage);
            virus[1].Initialize(Content, new Vector2(3, 0), stage);
            virus[2].Initialize(Content, new Vector2(5, 2), stage);

            foreach (Virus foo in virus) foo.AiInitialize();
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            song = Content.Load<Song>("music/battle");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

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

            //Custom screen commands
            if (custom.open)
            {
                if (JustPressed(Keys.Space)) custom.Select();
                if (JustPressed(Keys.Z) | JustPressed(Keys.X)) custom.unSelect();
                if (moveKey() != new Vector2(0, 0)) custom.moveCursor(moveKey());
            }

            //Only run these during battle
            if (custom.closed)
            {
                if (JustPressed(Keys.Space) && custom.custom == custom.customMax)
                    custom.Open();

                if (moveKey() != new Vector2(0, 0)) navi.Move(moveKey());

                if (JustPressed(Keys.Z)) navi.Buster(); 
                if (JustPressed(Keys.X) && navi.chips.Count() > 0 && navi.canAttack())
                {
                    navi.chips[0].Use(navi);
                    navi.chips.RemoveAt(0);
                }
                
                //Does a charged attack                
                if (IsHeld(Keys.Z) && !(navi.charged | navi.isAttacking)) navi.isCharging = true;
                
                if (IsReleased(Keys.Z))
                {
                    if (navi.charged) navi.chargedAttack();
                    navi.isCharging = false;
                    navi.charged = false;
                }

                foreach (Actor foo in stage.actorArray)
                {
                    if (foo!=null) foo.Update(gameTime);
                }

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
            custom.drawBars(spriteBatch, screenSize);
            
            stage.Draw(spriteBatch, screenSize);

            foreach (Actor foo in stage.actorArray)
            {
                if (foo != null) foo.Draw(spriteBatch, screenSize);
            }

            foreach (Projectile foo in stage.projectileList)
                foo.Draw(spriteBatch, screenSize);

            //Draw effects on top of actors
            for (int i = 0; i < stage.stageEffects.effect.Count; i++)
            {
                stage.stageEffects.effect[i].Draw(spriteBatch, stage.stageEffects.location[i], screenSize);
            }

            //Draw this last, we want this to be on top of everything
            custom.Draw(spriteBatch, screenSize);

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

        public Vector2 moveKey()
        {
            if (JustPressed(Keys.Left)) return new Vector2(-1, 0);
            if (JustPressed(Keys.Right)) return new Vector2(1, 0);
            if (JustPressed(Keys.Up)) return new Vector2(0, -1);
            if (JustPressed(Keys.Down)) return new Vector2(0, 1);
            else return new Vector2(0, 0);
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
            foreach (Chip foo in navi.chipFolder) foo.selected = false;

            navi.customFolder = navi.chipFolder.ToList();
            navi.customFolder.Shuffle();

            custom.Initialize(Content, navi);
            stage.Initialize(Content);
        }

    }
}