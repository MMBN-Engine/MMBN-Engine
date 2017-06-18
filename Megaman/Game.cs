using System;
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

        Dictionary<string, Chip> chipsList;

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

            //Overwrite the log file
            System.IO.File.WriteAllText("log.txt", "");

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
            panelTypes = Scripting.getObjectsFromFile("panelTypes.txt").
                ToDictionary(k => k.Key, k => (PanelType)k.Value);
            damageTypes = Scripting.getObjectsFromFile("damageTypes.txt").
                ToDictionary(k => k.Key, k => (DamageType)k.Value);

            chipsList = Scripting.getObjectsFromFile("chips/chips.txt").
                            ToDictionary(k => k.Key, k => (Chip)k.Value);
            areaList = Scripting.getObjectsFromFile("areas/areas.txt").
                            ToDictionary(k => k.Key, k => (Area)k.Value);

            attackTypes = Scripting.getObjectsFromFile("attackTypes.txt").
                            ToDictionary(k => k.Key, k => (AttackType)k.Value); 

            naviList = Scripting.getObjectsFromFile("navis.txt").
                            ToDictionary(k => k.Key, k => (Navi)k.Value);
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

            foreach (KeyValuePair<string, Chip> entry in chipsList)
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
            navi.chipFolder[0] = chipsList["Cannon"].setCode("A");
            navi.chipFolder[1] = chipsList["Cannon"].setCode("A");
            navi.chipFolder[2] = chipsList["Cannon"].setCode("B");
            navi.chipFolder[3] = chipsList["Cannon"].setCode("B");
            navi.chipFolder[4] = chipsList["ShotGun"].setCode("J");;
            navi.chipFolder[5] = chipsList["ShotGun"].setCode("J");;
            navi.chipFolder[6] = chipsList["ShotGun"].setCode("J");;
            navi.chipFolder[7] = chipsList["V-Gun"].setCode("D");;
            navi.chipFolder[8] = chipsList["V-Gun"].setCode("D");;
            navi.chipFolder[9] = chipsList["V-Gun"].setCode("D");;
            navi.chipFolder[10] = chipsList["SideGun"].setCode("S");;
            navi.chipFolder[11] = chipsList["AirShot1"].setCode("@");;
            navi.chipFolder[12] = chipsList["AirShot1"].setCode("@");;
            navi.chipFolder[13] = chipsList["AirShot1"].setCode("@");;
            navi.chipFolder[14] = chipsList["MiniBomb"].setCode("B");;
            navi.chipFolder[15] = chipsList["MiniBomb"].setCode("B");;
            navi.chipFolder[16] = chipsList["MiniBomb"].setCode("S");;
            navi.chipFolder[17] = chipsList["Sword"].setCode("L");;
            navi.chipFolder[18] = chipsList["Sword"].setCode("L");;
            navi.chipFolder[19] = chipsList["Sword"].setCode("L");;
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
    }
}