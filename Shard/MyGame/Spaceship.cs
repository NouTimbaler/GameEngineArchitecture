using SDL2;
using Shard;
using System.Drawing;

namespace MyGame
{
    class Spaceship : GameObject, InputListener, CollisionHandler
    {
        bool up, down, turnLeft, turnRight;
        int upKey, downKey, leftKey, rightKey;


        public override void initialize()
        {

            this.Transform.X = 250.0f;
            this.Transform.Y = 500.0f;
            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("spaceship.png");


            Bootstrap.getInput().addListener(this);

            up = false;
            down = false;

            upKey = InputCode.Shard_W;
            downKey = InputCode.Shard_S;
            leftKey = InputCode.Shard_A;
            rightKey = InputCode.Shard_D;

            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.MaxForce = 10;
            MyBody.AngularDrag = 0.01f;
            MyBody.Drag = 0f;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.ImpartForce = false;
            MyBody.Kinematic = false;


            //           MyBody.PassThrough = true;
            //            MyBody.addCircleCollider(0, 0, 5);
            //            MyBody.addCircleCollider(0, 34, 5);
            //            MyBody.addCircleCollider(60, 18, 5);
            //     MyBody.addCircleCollider();

            MyBody.addRectCollider();

            addTag("Spaceship");


        }

        public void changePlayer()
        {
            this.Transform.X = 250.0f * 2;
            this.Transform.Y = 500.0f;

            upKey = InputCode.Shard_I;
            downKey = InputCode.Shard_K;
            leftKey = InputCode.Shard_J;
            rightKey = InputCode.Shard_L;

        }

        public void fireBullet()
        {
            Bullet b = new Bullet();

            b.setupBullet(this, this.Transform.Centre.X, this.Transform.Centre.Y);

            b.Transform.rotate(this.Transform.Rotz);

            Bootstrap.getSound().playSound ("fire.wav");
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
                MyBody.addTorque(-0.3f);
            }

            if (turnRight)
            {
                MyBody.addTorque(0.3f);
            }

            if (up)
            {

                MyBody.addForce(this.Transform.Forward, 0.5f);

            }

            if (down)
            {
                MyBody.addForce(this.Transform.Forward, -0.2f);
            }


        }

        public override void update()
        {
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

    }
}
