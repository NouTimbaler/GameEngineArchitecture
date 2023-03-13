using System.Drawing;
using Shard;

namespace MyGame
{
    class MenuButton : GameObject, InputListener, CollisionHandler
    {
        string name;
        Color borderCol, col, hoverCol;

        bool hover;
        Action act;


        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);

            hover = false;

            addTag("MenuButton");
        }

        public virtual void setUpButton(string name, int x, int y, int w, int h, Color col, Color border, Color hov, Action a)
        {
            this.name = name;

            this.Transform.X = x;
            this.Transform.Y = y;
            this.Transform.Wid = w;
            this.Transform.Ht = h;

            this.borderCol = border;
            this.col = col;
            this.hoverCol = hov;

            act = a;

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

            MyBody.addRectCollider();

        }

        public override void physicsUpdate()
        {
            return;
        }

        public virtual void action()
        {
            act();
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            if (eventType == "MouseUp" && inp.Button == 1)
            {
                if (hover) action();
            }
        }

        public override void update()
        {
            //Button
            if (hover) 
                Bootstrap.getDisplay().drawFilledRectangle((int)this.Transform.X, (int)this.Transform.Y, 
                                                            (int)this.Transform.Wid, (int)this.Transform.Ht, hoverCol);
            else 
                Bootstrap.getDisplay().drawFilledRectangle((int)this.Transform.X, (int)this.Transform.Y, 
                                                            (int)this.Transform.Wid, (int)this.Transform.Ht, col);
            //Border
            Bootstrap.getDisplay().drawRectangle((int)this.Transform.X, (int)this.Transform.Y, 
                                                  (int)this.Transform.Wid, (int)this.Transform.Ht, borderCol);

            // TODO: do something about centering lol
            Bootstrap.getDisplay().showText(name, (int)this.Transform.Centre.X - name.Length/2,
                                                (int)this.Transform.Centre.Y - 12, 24, 255, 255, 255);
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Mouse"))
            {
                hover = true;
            }
        }

        public void onCollisionExit(PhysicsBody x)
        {
            if (x != null && x.Parent.checkTag("Mouse"))
            {
                hover = false;
            }
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "MenuButton: [" + name + "," + this.Transform.X + ", " + this.Transform.Y + ", " + this.Transform.Wid + ", " + this.Transform.Ht + "]";
        }

    }
}
