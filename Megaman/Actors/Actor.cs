using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Megaman.AttackType;
using Megaman.Chips;
using CustomExtensions;
using Megaman.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

//right now all movement code is for teleporting, will change it to allow sliding later

namespace Megaman.Actors
{
    public class Actor : Object
    {
        public int HP, MaxHP;
        public Animation staticSprite, moveSprite, guardSprite;
        public Animation busterSprite;

        public SoundEffect deathSound;

        public Dictionary<string, Animation> attackSprites;
        // 0 - shoot
        // 1 - sword
        // 2 - bomb
        // 3 - hammer
        public Dictionary<string, int> attackFrame;  //Animation frame for which attack is done, 0 is last frame
        protected string attackName;

        //Definitions for the loading the attack sprites
        public Vector2 origin;
        public int spriteWidth;
        public int attackSpeed, moveSpeed;

        public Dictionary<string, List<Color>> palettes;

        public string name;

        public Animation gunSprite;
        public bool isShooting;

        public Animation deathSprite;  //Sprite to play when we die

        public Dictionary<string, bool> Body;

        public bool isAttacking;
        public bool isSlashing;

        public Dictionary<string, AttackType> attackTypes;

        protected bool isGuarding;    //Check if we are in the process of doing the guard animation
        public bool Guard;         //True - guard effect is active
        internal int guardTime;    //Maximum time before guard resests
        internal int guardElapsed; //Time since timer was set 

        public bool noGuarding;  //If true, we can't guard

        protected bool moveStart, moveFin;
        public bool isSliding;
        public Vector2 move;

        public delegate void attackMethod(attackSpecs info);
        public attackMethod attackHandle;
        public attackSpecs info;                //structure containing attack information

        public List<Chip> chips;  //Battle chips that can be used

        public string gfxFolder;  //Folder where sprites are stored

        //Atributes
        public bool FlotShoe, AirShoe;

        public bool isDead;
  
        public Actor()
        {
            attackTypes = Game.attackTypes;

            staticSprite = new Animation();
            moveSprite = new Animation();
            guardSprite = new Animation();
            deathSprite = new Megaman.Animation();

            info = new attackSpecs();

            chips = new List<Chip>();

            Body = new Dictionary<string, bool>();
        }

        public override void Initialize(ContentManager content,  Vector2 position, Stage stage)
        {
            base.Initialize(content, position, stage);

            foreach(KeyValuePair<string, DamageType> entry in Game.damageTypes)
            {
                if (entry.Value.defaultDef) Body.Add(entry.Key, true);
                else Body.Add(entry.Key, false);
            }
            
            stage.actorArray[(int)position.X, (int)position.Y] = this;
            activeSprite = staticSprite;

            palettes = Scripting.loadImageFolder(gfxFolder + "palettes/").
                            ToDictionary(k => k.Key, k => k.Value.getPalette());

            loadAttackSprites();
        }

        public void loadPalettes()
        {
            palettes = new Dictionary<string, List<Color>>();

            List<String> fileArray = Scripting.getFilesFromFolder(gfxFolder + "palettes");

            foreach (string t in fileArray)
            {
                palettes.Add(t, Scripting.loadImage(gfxFolder + "palettes/" + t + ".png").getPalette());
            }
        }

        public void loadAttackSprites()
        {
            attackSprites = new Dictionary<string, Animation>();

            List<String> fileArray = Scripting.getFilesFromFolder(gfxFolder + "attacks");

            foreach (string t in fileArray)
            {
                Animation sprite = new Animation();
                sprite.Initialize(Scripting.loadImage(gfxFolder + "attacks/" + t + ".png"), 
                    origin, spriteWidth, attackSpeed, false);

                attackSprites.Add(t, sprite);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
                               
            if (moveStart)
            {
                if (!moveSprite.active)
                {
                    moveStart = false;
                    moveFin = true;
                    moveSprite.currentFrame = moveSprite.frameCount - 1;
                    moveSprite.active = true;
                    moveSprite.forward = false;
                    doMove();
                }
            }

            if (moveFin)
            {
                if (!moveSprite.active)
                {
                    isSliding = false;
                    moveFin = false;
                    activeSprite = staticSprite;

                    string panel = stage.getPanelType(position);
                    onStep(this, panel);
                }
            }

            if (isAttacking)
            {
                int Frame = activeSprite.currentFrame;
                int Target = attackFrame[attackName];

                if ((Frame >= Target) && (Target > 0) && (attackHandle != null))
                {
                    attackHandle(info);
                    attackHandle = null;
                }

                if (!activeSprite.active && !isShooting)
                {
                    isAttacking = false;
                    activeSprite = staticSprite;
                    //Shooting and slashing have special conditions for attacking
                    attackHandle?.Invoke(info);
                }
            }

            //Sets guard when sprite animation finishes
            if (isGuarding)
            {
                if (!guardSprite.active)
                {
                    isGuarding = false;
                    Guard = true;
                }
            }

            //Advance the guard timer, break guard if over
            if (Guard)
            {
                guardElapsed += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if ((guardElapsed > guardTime) && (guardTime > 0))
                {
                    breakGuard();
                }
            }
        }
        

        //as of now, this should only be used to move one space
        //i'll add a teleport bool later to avoid ice weirdness
        public virtual void Move(Vector2 move)
        {
            if (canMove(move))
            {
                //don't allow new movement if movement is being forced.
                if (!isSliding) this.move = move;
                
                moveStart = true;
                moveSprite.Reset();
                moveSprite.forward = true;
                activeSprite = moveSprite;
            }

        }

        public void onStep(Actor actor, string panel)
        {
            Game.panelTypes[panel].onStep(this, panel);
        }

        public override void Draw(SpriteBatch spriteBatch, float resolution)
        {
            base.Draw(spriteBatch, resolution);

            //Draw the chips
            if (chips.Count() > 0)
            {
                for (int i = chips.Count() - 1; i >= 0; i--)
                {
                    spriteBatch.Draw(chips[i].icon, (location + new Vector2(16 - 2 * i, -58 - 2 * i)) * resolution,
                        scale: new Vector2(1, 1) * resolution, color: Color.White);
                }
            }
        }

        public bool canMove(Vector2 move)
        {
            //checks to see if we're already moving
            if (moveStart | moveFin) return false;

            //checks to see if we're attacking
            if (isAttacking) return false;

            //checks to see if we are guarding
            if (isGuarding | Guard) return false;
           
            //checks to see if the tile exists and is controlled by the right dude and no one is there
            if (position.X + move.X > -1 && position.X + move.X < stage.PanelType.GetLength(0) &&
                position.Y + move.Y > -1 && position.Y + move.Y < stage.PanelType.GetLength(1)
                && color.Equals(stage.Area[(int)(position.X + move.X), (int)(position.Y + move.Y)])
                && stage.actorArray[(int)(position.X + move.X), (int)(position.Y + move.Y)] == null)
            {
                //can't step on broken panels without airshoes
                if (stage.PanelType[(int)(position.X + move.X), (int)(position.Y + move.Y)].Equals("Broken")
                    | stage.PanelType[(int)(position.X + move.X), (int)(position.Y + move.Y)].Equals("Hole"))
                    
                    if (AirShoe) return true;
                    else return false;

                return true;
            }
            else return false;
        }

        //does the actual movement
        private void doMove()
        {
            //checks to see if we float
            if (!FlotShoe)
            {
                //we crack broken panels
                if (stage.PanelType[(int)position.X, (int)position.Y].Equals("Cracked") && !FlotShoe)
                    stage.PanelType[(int)position.X, (int)position.Y] = "Broken";
            }

            // Tells the stage we moved
            stage.actorArray[(int)position.X, (int)position.Y] = null;
            position += move;
            stage.actorArray[(int)position.X, (int)position.Y] = this;
        }

        public bool canAttack()
        {
            if (moveStart | moveFin | isSliding | isAttacking | isGuarding) return false;
            else return true;
        }

        public virtual void doAttack(string attackName)
        {
            if (canAttack())
            {
                attackSprites[attackName].Reset();
                activeSprite = attackSprites[attackName];
                isAttacking = true;
                this.attackName = attackName;
            }
        }
        
        public void Heal(Actor actor, int value)
        {
            actor.HP += value;
            if (actor.HP > actor.MaxHP) actor.HP = actor.MaxHP;
        }

        public void Gun(attackSpecs info)
        {
            int y = (int)position.Y;

            if (color == "red")
            {
                for (int i = (int) position.X + 1; i < 6; i++)
                {
                    if (isBlue(i, y))
                    {
                        doDamage(new Vector2(i, y), info.damage, info.damageType, info.effects, info.effectSprite);
                        break;
                    }
                }
            }

            if (color == "blue")
            {
                for (int i = (int) position.X - 1; i > -1; i--)
                {
                    if (isRed(i, y))
                    {
                        doDamage(new Vector2(i, y), info.damage, info.damageType, info.effects, info.effectSprite);
                        break;
                    }
                }
            }
        }

        public void Sword(attackSpecs info)
        {
            int x = (int) position.X;
            int y = (int) position.Y;
            
            if (color == "red")
            {
                doDamage(new Vector2(x + 1, y), info.damage, info.damageType, info.effects, info.effectSprite);
            }

            if (color == "blue")
            {
                doDamage(new Vector2(x - 1, y), info.damage, info.damageType, info.effects, info.effectSprite);
            }
        }

        // Attack commands take two inputs, a sprite and an Attack handel
        // All other information for the attack (damage, damageType, etc.) is handled in the AttackList class
        // Eventually these methods should be unified to a single method
        public void Slash(Animation animation, attackMethod attackHandle)
        {
            if (!canAttack()) return;
            doAttack("sword");
            isSlashing = true;

            this.attackHandle = attackHandle;
        }

        public void Hammer(Animation animation, attackMethod attackHandle)
        {
            if (!canAttack()) return;
            doAttack("bomb");

            this.attackHandle = attackHandle;
        }

        public void createShot(attackSpecs info)
        {
            new Shot(this);
        }

        public void createWave(attackSpecs info)
        {
            new Wave(this);
        }

        //Initialize the guard animation and sets time to zero
        public void setGuard()
        {
            if (noGuarding) return;

            guardSprite.Reset();
            activeSprite = guardSprite;
            isGuarding = true;
            guardElapsed = 0;
        }

        public void breakGuard()
        {
            activeSprite = staticSprite;
            Guard = false;
        }

        public void Shoot(Animation animation, attackMethod attackHandle)
        {
            if (!canGuard()) return;
            gunSprite = animation;
            gunSprite.Reset();
            doAttack("shoot");
            isShooting = true;

            this.attackHandle = attackHandle;
        }

        public virtual void Delete()
        {
        }

        public bool canGuard()
        {
            if (moveStart | moveFin | isAttacking | isGuarding | noGuarding) return false;
            else return true;
        }

        public void paletteSwap(string palette1, string palette2)
        {
            staticSprite.map = staticSprite.map.changeColor(palettes[palette1], palettes[palette2]);
            guardSprite.map = guardSprite.map.changeColor(palettes[palette1], palettes[palette2]);
            foreach (KeyValuePair<string, Animation> foo in attackSprites)
            {
                foo.Value.map = foo.Value.map.changeColor(palettes[palette1], palettes[palette2]);
            }
        }
    }
}