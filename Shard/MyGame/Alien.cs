using Shard;
using System;
using System.Numerics;

namespace MyGame
{
    class Alien : NetworkObject, InputListener, CollisionHandler
    {
        private int spriteToUse;
        private string[] sprites;

        private float animCounter;
        private float timeToSwap;

        bool up, down, left, right;
        int upKey, downKey, leftKey, rightKey;

        public Alien(bool own, int id) : base(own, id)
        {}

        public override void initialize()
        {
            sprites = new string[2];
            sprites[0] = "invader1.png";
            sprites[1] = "invader2.png";
            spriteToUse = 0;
            timeToSwap = 0.5f;

            Random rnd = new Random();
            int x  = rnd.Next(20, 580);
            int y  = rnd.Next(20, 380);
            this.Transform.X = (float)x;
            this.Transform.Y = (float)y;
            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath(sprites[spriteToUse]);

            if(this.isOwner) Bootstrap.getInput().addListener(this);

            up = false;
            down = false;
            left = false;
            right = false;

            upKey = InputCode.Shard_W;
            downKey = InputCode.Shard_S;
            leftKey = InputCode.Shard_A;
            rightKey = InputCode.Shard_D;

            setPhysicsEnabled();

            MyBody.Mass = 1.2f;
            MyBody.MaxForce = 7;
            MyBody.Drag = 0.5f;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.ImpartForce = false;
            MyBody.Kinematic = false;


            MyBody.addRectCollider();

            addTag("Alien");
        }


        public void changeSprite()
        {
            spriteToUse += 1;

            if (spriteToUse >= sprites.Length)
            {
                spriteToUse = 0;
            }

            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath(sprites[spriteToUse]);
        }

        public override void update()
        {
            animCounter += (float)Bootstrap.getDeltaTime();
            if (animCounter > timeToSwap)
            {
                animCounter -= timeToSwap;
                changeSprite();
            }

            Bootstrap.getDisplay().addToDraw(this);
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            if (eventType == "KeyDown")
            {
                if (inp.Key == upKey)
                {
                    up = true;
                }

                if (inp.Key == downKey)
                {
                    down = true;
                }

                if (inp.Key == rightKey)
                {
                    right = true;
                }

                if (inp.Key == leftKey)
                {
                    left = true;
                }

            }
            else if (eventType == "KeyUp")
            {
                if (inp.Key == upKey)
                {
                    up = false;
                }

                if (inp.Key == downKey)
                {
                    down = false;
                }

                if (inp.Key == rightKey)
                {
                    right = false;
                }

                if (inp.Key == leftKey)
                {
                    left = false;
                }
            }
        }

        public override void physicsUpdate()
        {

            if (left)
            {
                MyBody.addForce(new Vector2(1, 0), -1f);
            }

            if (right)
            {
                MyBody.addForce(new Vector2(1, 0), 1f);
            }

            if (up)
            {
                MyBody.addForce(new Vector2(0, -1), 1f);
            }

            if (down)
            {
                MyBody.addForce(new Vector2(0, -1), -1f);
            }
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Bullet"))
            {
                ((GameMyGame)Bootstrap.getRunningGame()).destroyAlien(this);
            }
        }

        public void onCollisionExit(PhysicsBody x)
        {
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "Alien: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Wid + ", " + Transform.Ht + "]";
        }

        public string getState()
        {
            return this.id + ",alien," + Transform.X + "," + Transform.Y;
        }
        public void updateState(string m)
        {
            string[] s = m.Split(",");
            
            Transform.X = float.Parse(s[2]);
            Transform.Y = float.Parse(s[3]);
        }

    }
}

