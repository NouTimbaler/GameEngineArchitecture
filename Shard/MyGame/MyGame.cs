using MyGame;
using System;
using System.Collections.Generic;
using System.Drawing;

using System.Net;
using System.Text;

namespace Shard
{
    class GameMyGame : NetworkGame, InputListener
    {
        GameObject background;

        MenuButton h, c;
        bool buttons;

        Spaceship ship;
        Alien alien;
        List<Alien> aliens;

        public override void updateState(Message message)
        {
            if(message.type == "ADD")
            {
                int id = Int32.Parse(message.message);
                Alien a = createAlien(false, id);
                aliens.Add(a);
            }
            else if(message.type == "ACC")
            {
                alien.id = Int32.Parse(message.message);
            }
            else if(message.type == "DEL")
            {
                Alien t = null;
                int id = Int32.Parse(message.message);
                foreach(Alien a in aliens)
                {
                    if(a.id == id)
                    {
                        a.ToBeDestroyed = true;
                        t = a;
                        break;
                    }
                }
                if(t != null) aliens.Remove(t);
            }
            else if(message.type == "MESSAGE")
            {
                string[] s = message.message.Split(",");
                int id = Int32.Parse(s[0]);
                string type = s[1];
                if(type == "ship")
                {
                    ship.updateState(message.message);
                    if(s[5] == "fire") ship.fireBullet();
                } 
                else if(type == "alien")
                {
                    foreach(Alien a in aliens)
                    {
                        if (a.id == id)
                        {
                            a.updateState(message.message);
                            break;
                        }
                    }
                }
            }
        }

        public override void spawnInState(string state)
        {
            string[] st = state.Split(":");
            foreach(string s in st)
            {
                int id = Int32.Parse(s.Split(",")[0]);
                string type = s.Split(",")[1];
                if(type == "ship")
                {
                    ship = createShip(false, id);
                    ship.updateState(s);
                }
                else if(type == "alien")
                {
                    Alien a = createAlien(false, id);
                    aliens.Add(a);
                    a.updateState(s);
                }
            }
        }

        public override string getFullState()
        {
            string s = ship.getState();
            foreach(Alien a in aliens)
            {
                s = s + ":" + a.getState();
            }
            return s;
        }

        public override string getState()
        {
            if(Bootstrap.getNetworkManager().IsHost)
                return ship.getState();
            if(Bootstrap.getNetworkManager().IsClient)
                return alien.getState();
            return "MESSAGE;hola";
        }

        public override void update()
        {
            
            Bootstrap.getDisplay().showText("FPS: " + Bootstrap.getSecondFPS() + " / " + Bootstrap.getFPS(), 10, 10, 12, 255, 255, 255);

            if(!Bootstrap.getNetworkManager().IsHost && !Bootstrap.getNetworkManager().IsClient && !buttons)
            {
                clearGame();
                createButtons();
            }

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

            createButtons();
        }

        public Spaceship createShip(bool own, int id)
        {
            Spaceship s = new Spaceship(own, id);
            s.initialize();
            return s;
        }

        public Alien createAlien(bool own, int id)
        {
            Alien a = new Alien(own, id);
            a.initialize();
            return a;
        }
        public void destroyAlien(Alien a)
        {
            if(alien != null && alien.id == a.id)
            {
                clearGame();
            }

            else
            {
                aliens.Remove(a);
                a.ToBeDestroyed = true;
            }
        }
        public void clearGame()
        {
            if(ship != null)
                ship.ToBeDestroyed = true;
            if(alien != null)
                alien.ToBeDestroyed = true;
            foreach(Alien al in aliens)
            {
                al.ToBeDestroyed = true;
            }
            aliens.Clear();
            if(Bootstrap.getNetworkManager().IsHost)
                Bootstrap.getNetworkManager().stopHost();
            if(Bootstrap.getNetworkManager().IsClient)
                Bootstrap.getNetworkManager().stopClient();
        }

        public void createButtons()
        {
            h = new MenuButton();
            h.initialize();
            h.setUpButton("Host", 150, 175, 100, 50, Color.Black, Color.White, Color.Gray, beHost);
            c = new MenuButton();
            c.initialize();
            c.setUpButton("Join", 350, 175, 100, 50, Color.Black, Color.White, Color.Gray, beClient);

            buttons = true;
        }

        public void beHost()
        {
            if(!Bootstrap.getNetworkManager().IsHost)
            {
                Bootstrap.getNetworkManager().makeHost();
                ship = createShip(true, 0);
            }
            buttons = false;
            Bootstrap.getInput().removeListener(h);
            Bootstrap.getInput().removeListener(c);
            h.ToBeDestroyed = true;
            c.ToBeDestroyed = true;

        }
        public void beClient()
        {
            if(!Bootstrap.getNetworkManager().IsClient)
            {
                string ip = "loopback";
                if (!Bootstrap.getNetworkManager().makeClient(ip, 6000)) 
                    Debug.getInstance().log("Something wrong");
                else
                {
                    alien = createAlien(true, 0);
                    buttons = false;
                    Bootstrap.getInput().removeListener(h);
                    Bootstrap.getInput().removeListener(c);
                    h.ToBeDestroyed = true;
                    c.ToBeDestroyed = true;
                }
            }
        }

        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);

            createObjects();

            MouseCollider mo = new MouseCollider();
            mo.initialize();

            aliens = new List<Alien>();
        }

        public void handleInput(InputEvent inp, string eventType)
        {

            if (eventType == "MouseDown")
            {
            }

            if (eventType == "MouseDown" && inp.Button == 1)
            {
            }

            if (eventType == "MouseDown" && inp.Button == 3)
            {
            }

            if (eventType == "KeyUp")
            {
                if (inp.Key == InputCode.Shard_ESCAPE)
                {
                    clearGame();
                    if(!buttons)
                        createButtons();
                }
            }
        }
    }
}

