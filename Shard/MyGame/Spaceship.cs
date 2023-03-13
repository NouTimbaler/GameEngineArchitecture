using SDL2;
using Shard;
using System.Drawing;

namespace MyGame
{
    class Spaceship : NetworkObject, InputListener, CollisionHandler
    {
        bool up, down, turnLeft, turnRight;
        int upKey, downKey, leftKey, rightKey;

        bool fired;
        
        public Spaceship(bool own, int id) : base(own,id)
        {}

        public override void disown()
        {
            Bootstrap.getInput().removeListener(this);
            this.isOwner = false;
        }

        public override void initialize()
        {
            this.Transform.X = 400.0f;
            this.Transform.Y = 300.0f;
            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("spaceship.png");

            if(this.isOwner) Bootstrap.getInput().addListener(this);

            up = false;
            down = false;
            fired = false;

            upKey = InputCode.Shard_W;
            downKey = InputCode.Shard_S;
            leftKey = InputCode.Shard_A;
            rightKey = InputCode.Shard_D;

            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.MaxForce = 10;
            MyBody.MaxTorque = 5;
            MyBody.AngularDrag = 0.5f;
            MyBody.Drag = 0.5f;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.ImpartForce = false;
            MyBody.Kinematic = false;


            MyBody.addRectCollider();

            addTag("Spaceship");
        }

        public void fireBullet()
        {

            Bullet b = new Bullet();
            b.initialize();

            b.setupBullet(this, this.Transform.Centre.X, this.Transform.Centre.Y);

            b.Transform.rotate(this.Transform.Rotz);
            
            fired = true;

            //Bootstrap.getSound().playSound("fire.wav");
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
                    turnRight = true;
                }

                if (inp.Key == leftKey)
                {
                    turnLeft = true;
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
                    turnRight = false;
                }

                if (inp.Key == leftKey)
                {
                    turnLeft = false;
                }
            }

            if (eventType == "KeyUp")
            {
                if (inp.Key == InputCode.Shard_SPACE)
                {
                    fireBullet();
                }
            }
        }

        public override void physicsUpdate()
        {

            if (turnLeft)
            {
                MyBody.addTorque(-1f);
            }

            if (turnRight)
            {
                MyBody.addTorque(1f);
            }

            if (up)
            {
                MyBody.addForce(this.Transform.Forward, 1f);
            }

            if (down)
            {
                MyBody.addForce(this.Transform.Forward, -1f);
            }
        }

        public override void update()
        {
            Bootstrap.getDisplay().showText(id.ToString(), Transform.Centre.X, Transform.Centre.Y - Transform.Ht, 12, 255, 255, 255);
            Bootstrap.getDisplay().addToDraw(this);
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Bullet") == false)
            {
                MyBody.DebugColor = Color.Red;
            }
        }

        public void onCollisionExit(PhysicsBody x)
        {
            MyBody.DebugColor = Color.Green;
        }

        public void onCollisionStay(PhysicsBody x)
        {
            MyBody.DebugColor = Color.Blue;
        }

        public override string ToString()
        {
            return "Spaceship: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Wid + ", " + Transform.Ht + "]";
        }

        public string getState()
        {
            string f = "no";
            if(fired)
            {
                f = "fire";
                fired = false;
            }
            return this.id + "," + "ship" + "," + Transform.X + "," + Transform.Y + "," + Transform.Rotz + "," + f;
        }
        public void updateState(string m)
        {
            string[] s = m.Split(",");
            
            Transform.X = float.Parse(s[2]);
            Transform.Y = float.Parse(s[3]);
            Transform.Rotz = float.Parse(s[4]);
        }

    }
}
