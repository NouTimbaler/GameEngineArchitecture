using MyGame;
using System;
using System.Collections.Generic;
using System.Drawing;

using System.Net;
using System.Text;

namespace Shard
{
    class MyGame : NetworkGame, InputListener
    {
        GameObject background;
        List<GameObject> asteroids;

        Spaceship ship;
        List<Spaceship> ships;

        public override void updateState(Message message)
        {
            //Debug.getInstance().log("INSIDE GAME: " + m.message);

            if(message.type == "ADD")
            {
                Spaceship t = new Spaceship(false);
                t.initialize();
                t.id = Int32.Parse(message.message);
                ships.Add(t);
            }
            else if(message.type == "ACC")
            {
                ship.id = Int32.Parse(message.message);
            }
            else if(message.type == "DEL")
            {
                Spaceship t = null;
                int id = Int32.Parse(message.message);
                foreach(Spaceship sh in ships)
                {
                    if(sh.id == id)
                    {
                        sh.ToBeDestroyed = true;
                        t = sh;
                        break;
                    }
                }
                if(t != null) ships.Remove(t);
            }
            else if(message.type == "MESSAGE")
            {
                string[] s = message.message.Split(",");
                int id = Int32.Parse(s[0]);
                foreach(Spaceship sh in ships)
                {
                    if (sh.id == id)
                    {
                        sh.updateState(message.message);
                        break;
                    }

                }

            }
        }

        public override void spawnInState(string state)
        {
            string[] st = state.Split(":");
            foreach(string s in st)
            {
                Spaceship t = new Spaceship(false);
                t.initialize();
                t.id = Int32.Parse(s.Split(",")[0]);
                ships.Add(t);
                t.updateState(s);
            }
        }

        public override string getFullState()
        {
            string s = ship.getState();
            foreach(Spaceship sh in ships)
            {
                s = s + ":" + sh.getState();
            }
            return s;
        }

        public override string getState()
        {
            string m = ship.getState();
            //TODO: only send if host accepted or game started
            
            return m;
        }

        public override void update()
        {
            
            Bootstrap.getDisplay().showText("FPS: " + Bootstrap.getSecondFPS() + " / " + Bootstrap.getFPS(), 10, 10, 12, 255, 255, 255);
        }

        public override int getTargetFrameRate()
        {
            return 100;

        }
        public void createObjects()
        {
            background = new GameObject();
            background.Transform.SpritePath = getAssetManager().getAssetPath("background2.jpg");
            background.Transform.X = 0;
            background.Transform.Y = 0;

            ship = new Spaceship(true);
            ship.initialize();
            ships = new List<Spaceship>();

            //createButtons();
        }

        public void createButtons()
        {
            MenuButton s = new MenuButton();
            s.setUpButton("Server", 340, 400, 200, 100, Color.Black, Color.White, Color.Gray);
            MenuButton c = new MenuButton();
            c.setUpButton("Client", 740, 400, 200, 100, Color.Black, Color.White, Color.Gray);
        }

        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);

            createObjects();

            MouseCollider mo = new MouseCollider();

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
                if (inp.Key == InputCode.Shard_H)
                {
                    if(!Bootstrap.getNetworkManager().IsHost)
                    {
                        Bootstrap.getNetworkManager().makeHost();
                        ship.id = 0;
                    }
                    else 
                        Bootstrap.getNetworkManager().stopHost();
                }

                if (inp.Key == InputCode.Shard_C)
                {
                    if(!Bootstrap.getNetworkManager().IsClient)
                    {
                        if (!Bootstrap.getNetworkManager().makeClient("loopback", 6000)) 
                            Debug.getInstance().log("Something wrong");
                    }
                    else
                        Bootstrap.getNetworkManager().stopClient();
                }
            }
        }
    }
}

