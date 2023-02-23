using SDL2;
using System.Drawing;

namespace Shard
{
    class MouseCollider : GameObject, InputListener, CollisionHandler
    {

        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);

            this.Transform.X = 50;
            this.Transform.Y = 50;
            int radius = 10;

            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.MaxForce = 0;
            MyBody.MaxTorque = 0;
            MyBody.AngularDrag = 0;
            MyBody.Drag = 0;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.ImpartForce = false;
            MyBody.Kinematic = true;
            MyBody.PassThrough = true;

            // offsetx, offsety, radius
            MyBody.addCircleCollider(0,0,radius);

            addTag("Mouse");
        }

        public override void physicsUpdate()
        {
            return;
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            if (eventType == "MouseMotion")
            {
                this.Transform.X = inp.X;
                this.Transform.Y = inp.Y;
                //MyBody.Trans.X = inp.X;
                //MyBody.Trans.Y = inp.Y;
            }
        }

        public override void update()
        {
            return;
        }

        public void onCollisionEnter(PhysicsBody x)
        {
        }

        public void onCollisionExit(PhysicsBody x)
        {
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "MouseCollider: [" + this.Transform.X + ", " + this.Transform.Y + "]";
        }

    }
}
