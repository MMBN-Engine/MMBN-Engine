﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Megaman.Actors;
using Megaman.Actors.Viruses;
using Megaman.Actors.Navis;
using Megaman.Chips;
using Megaman.Overworld;
using Megaman.Projectiles;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
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
    public partial class Game : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState currentKeyboard;
        KeyboardState oldKeyboard;
        Stage stage;
        Custom custom;

        public static string modulePath;

        public static Dictionary<string, PanelType> panelTypes;
        public static Dictionary<string, DamageType> damageTypes;

        Area currentArea, ACDC1;
        Dictionary<string, Area> areaList;
        Dictionary<string, Tileset> tilesetList;

        SoundEffect charge, chargeComplete;
        bool playedCharge, playedChargeComplete;
        bool inBattle;
        
        Dictionary<string, Song> songList;

        bool debug;

        public float screenSize;

        public static Dictionary<string, AttackType> attackTypes;
        
        Dictionary<string, Navi> naviList;
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

            modulePath = "modules/undernet/";

            debug = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            loadPanelTypesFromFile();
            loadDamageTypesFromFile();

            loadAreasFromFile();

            loadAttackTypesFromFile();

            loadNavisFromFile();
            navi = naviList["MegaMan"];

            currentKeyboard = new KeyboardState();
            oldKeyboard = new KeyboardState();
            stage = new Stage();
            custom = new Custom();
            
            virus = new List<Virus>();
            virus.Add(new Mettaur());
            virus.Add(new Mettaur2());
            virus.Add(new MettaurΩ());

            currentArea = areaList["ACDC1"];
            navi.area = currentArea;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            navi.Initialize(Content, new Vector2(1,1), stage);

            foreach (KeyValuePair<string, AttackType> entry in attackTypes)
                entry.Value.Initialize(Content);     
            
            virus[0].Initialize(Content, new Vector2(4, 1), stage);
            virus[1].Initialize(Content, new Vector2(3, 0), stage);
            virus[2].Initialize(Content, new Vector2(5, 2), stage);

            foreach (Virus foo in virus) foo.AiInitialize();
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            loadSongsFromFile();
            
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(songList["network"]);
            MediaPlayer.IsRepeating = true;

            loadTilesetsFromFile();

            charge = Content.Load<SoundEffect>("soundFX/battle/charge");
            chargeComplete = Content.Load<SoundEffect>("soundFX/battle/chargeComplete");

            foreach (KeyValuePair<string, Area> entry in areaList)
            {
                entry.Value.loadTileset(tilesetList[entry.Value.tilesetName]);
                entry.Value.generateMap();
            }
            newGame();

            debugContent();
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

            if (debug) debugCommands();

            if (inBattle)
            {

                //Custom screen commands
                if (custom.open) customScreenCommands();

                //Only run these during battle
                if (custom.closed)
                {

                    battleCommands();

                    foreach (Actor foo in stage.actorArray)
                    {
                        if (foo != null) foo.Update(gameTime);
                    }

                    stage.Update(gameTime);
                }

                custom.Update(gameTime);
            }
            else
            {
                if (overworldMoveKey().Length() != 0) navi.overWorldMove(overworldMoveKey());
            }

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

            if (inBattle)
            {
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
            }
            else
            {
                currentArea.Draw(spriteBatch, screenSize);
                navi.overWorldDraw(spriteBatch, screenSize);
            }

            if (debug) debugDraw();

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

        public Vector2 overworldMoveKey()
        {
            Vector2 moveVector = new Vector2();

            if (IsHeld(Keys.Left)) moveVector.X = -1;
            if (IsHeld(Keys.Right)) moveVector.X = 1;
            if (IsHeld(Keys.Up)) moveVector.Y = -1;
            if (IsHeld(Keys.Down)) moveVector.Y = 1;

            return moveVector;
        }

        public void newGame()
        {
            if (debug) debugFolder();
            else defaultFolder();

            foreach (Chip chip in navi.chipFolder) chip.Initialize(Content);
        }

        public void defaultFolder()
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
            navi.chipFolder[11] = new Airshot1("@");
            navi.chipFolder[12] = new Airshot1("@");
            navi.chipFolder[13] = new Airshot1("@");
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
            navi.chipFolder[28] = new Attack10("@");
            navi.chipFolder[29] = new Attack10("@");
        }

        public void battleStart()
        {
            inBattle = true;

            MediaPlayer.Play(songList["battle"]);

            foreach (Chip foo in navi.chipFolder) foo.selected = false;

            navi.customFolder = navi.chipFolder.ToList();
            navi.customFolder.Shuffle();

            custom.Initialize(Content, navi);
            stage.Initialize(Content);
        }

        public void customScreenCommands()
        {
            if (JustPressed(Keys.Space)) custom.Select();
            if (JustPressed(Keys.Z) | JustPressed(Keys.X)) custom.unSelect();
            if (moveKey() != new Vector2(0, 0)) custom.moveCursor(moveKey());
        }

        public void battleCommands()
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
            if (IsHeld(Keys.Z))
            {
                if (!(navi.charged | navi.isAttacking))
                {
                    navi.isCharging = true;
                    if (!playedCharge)
                    {
                        charge.Play();
                        playedCharge = true;
                    }
                }
                if (!playedChargeComplete && navi.charged)
                {
                    playedChargeComplete = true;
                    chargeComplete.Play();
                }
            }

            if (IsReleased(Keys.Z))
            {
                if (navi.charged) navi.chargedAttack(navi);
                playedCharge = false;
                playedChargeComplete = false;
                navi.isCharging = false;
                navi.charged = false;
            }
        }

        void loadPanelTypesFromFile()
        {
            panelTypes = new Dictionary<string, PanelType>();
            ScriptState state = Scripting.parse(modulePath + "panelTypes.txt");

            List<ScriptVariable> vlist = state.Variables.ToList();
            
            foreach (ScriptVariable v in vlist)
            {
                PanelType panel = (PanelType) v.Value;

                panelTypes.Add(panel.name, panel);
            }
        }

        void loadDamageTypesFromFile()
        {
            damageTypes = new Dictionary<string, DamageType>();
            ScriptState state = Scripting.parse(modulePath + "damageTypes.txt");

            List<ScriptVariable> vlist = state.Variables.ToList();

            foreach (ScriptVariable v in vlist)
            {
                DamageType damage = (DamageType)v.Value;
                damageTypes.Add(damage.name, damage);
            }
        }

        void loadAttackTypesFromFile()
        {
            attackTypes = new Dictionary<string, AttackType>();
            ScriptState state = Scripting.parse(modulePath + "attackTypes.txt");

            List<ScriptVariable> vlist = state.Variables.ToList();

            foreach (ScriptVariable v in vlist)
            {
                AttackType attack = (AttackType)v.Value;
                attackTypes.Add(attack.name, attack);
            }
        }

        void loadNavisFromFile()
        {
            naviList = new Dictionary<string, Navi>();
            ScriptState state = Scripting.parse(modulePath + "navis.txt");

            List<ScriptVariable> vlist = state.Variables.ToList();

            foreach (ScriptVariable v in vlist)
            {
                Navi navi = (Navi)v.Value;
                naviList.Add(navi.name, navi);
            }
        }
        void loadSongsFromFile()
        {
            songList = new Dictionary<string, Song>();

            ScriptState state = Scripting.parse("Content/music/songs.txt");

            ScriptVariable v = state.Variables[0];
            PropertyInfo[] p = v.Value.GetType().GetProperties();

            for (int i = 0; i < p.GetLength(0); i++)
            {
                string s = (string) Scripting.getScriptValue(p[i].Name, v);
                songList.Add(p[i].Name, Content.Load<Song>(s));
            }
        }

        void loadTilesetsFromFile()
        {
            tilesetList = new Dictionary<string, Tileset>();

            string name;
            Vector2 origin;
            int spriteWidth, tileWidth, tileHeight;
            Func<String[,], Vector2, string> mapParse;

            ScriptState state = Scripting.parse(modulePath + "/areas/tilesets.txt");

            for (int i = 0; i < state.Variables.Count(); i++)
            {
                ScriptVariable v = state.Variables[i];

                name = (string) Scripting.getScriptValue("name", v);
                origin = (Vector2) Scripting.getScriptValue("origin", v);

                spriteWidth =(int) Scripting.getScriptValue("spriteWidth", v);
                tileWidth =  (int) Scripting.getScriptValue("tileWidth", v);
                tileHeight = (int) Scripting.getScriptValue("tileHeight", v);

                mapParse = (Func<String[,], Vector2, string>)Scripting.getScriptValue("mapParse", v);

                tilesetList.Add(name, new Tileset(name, origin, spriteWidth,
                    tileWidth, tileHeight, mapParse));
            }
        }

        void loadAreasFromFile()
        {
            areaList = new Dictionary<string, Area>();

            ScriptState state = Scripting.parse(modulePath + "areas/areas.txt");

            List<ScriptVariable> v = state.Variables.ToList();

            foreach (ScriptVariable foo in v)
            {
                Area a = new Area();
                Scripting.equateFields(a, foo);
                areaList.Add(a.name, a);
            }
        }
    }
}