using MyGame;
using System;
using System.Collections.Generic;
using System.Drawing;

using System.Net;
using System.Text;

namespace Shard
{
    class MyGame : Game, InputListener
    {
        GameObject background;
        List<GameObject> asteroids;
        bool isHost;
        NetHost host;
        NetClient cli;
        bool created;
        
        public override void update()
        {
            
            Bootstrap.getDisplay().showText("FPS: " + Bootstrap.getSecondFPS() + " / " + Bootstrap.getFPS(), 10, 10, 12, 255, 255, 255);
            if (isHost) host.recieve();
            if (created) cli.recieve();

        }

        public override int getTargetFrameRate()
        {
            return 100;

        }
        public void createShip()
        {
            background = new GameObject();

            Spaceship ship = new Spaceship();
            Spaceship ship2 = new Spaceship();
            ship2.changePlayer();

            Random rand = new Random();
            int offsetx = 0, offsety = 0;

    

            background.Transform.SpritePath = getAssetManager().getAssetPath("background2.jpg");
            background.Transform.X = 0;
            background.Transform.Y = 0;


        }

        public override void initialize()
        {

            isHost = false;
            created = false;
            Bootstrap.getInput().addListener(this);
            createShip();

            MouseCollider mo = new MouseCollider();

            MenuButton s = new MenuButton();
            s.setUpButton("Server", 340, 400, 200, 100, Color.Black, Color.White, Color.Gray);
            MenuButton c = new MenuButton();
            c.setUpButton("Client", 740, 400, 200, 100, Color.Black, Color.White, Color.Gray);

            asteroids = new List<GameObject>();

        }

        public void handleInput(InputEvent inp, string eventType)
        {

            if (eventType == "MouseDown") {
                Console.WriteLine ("Pressing button " + inp.Button);
            }

            if (eventType == "MouseDown" && inp.Button == 1)
            {
                //Asteroid asteroid = new Asteroid();
                //asteroid.Transform.X = inp.X;
                //asteroid.Transform.Y = inp.Y;
                //asteroids.Add (asteroid);
            }

            if (eventType == "MouseDown" && inp.Button == 3)
            {
                foreach (GameObject ast in asteroids) {
                    ast.ToBeDestroyed = true;
                }

                asteroids.Clear();
            }

            if (eventType == "KeyDown")
            {
                if (inp.Key == InputCode.Shard_X)
                {
                    Debug.getInstance().log("Im host");
                    host = new NetHost();
                    host.initialize();
                    isHost = true;
                }

                if (inp.Key == InputCode.Shard_C)
                {
                    if(!created)
                    {
                        Debug.getInstance().log("Im client");
                        cli = new NetClient();
                        cli.initialize(IPAddress.Loopback, 6000);
                        created = true;
                    }
                    Message m = new Message();
                    m.message = "uuwu";
                    cli.sendTo(m);
                }
            }


        }
    }
}
