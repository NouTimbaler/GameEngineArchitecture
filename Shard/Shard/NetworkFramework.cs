
namespace Shard
{
        //NET
            //NEW new connecton as a host
            //ACC i have been accepted as a client
            //STATE spawn in this state
            //ADD add new player
            //DEAD host: client disconnected. client: can't reach host
            //DEL delete that player
        //MESSAGE actual message
    class Message
    {
        public string message {get; set; }
        public string type {get; set; }

        public string toString()
        {
            return type + ";" + message;
        }
    }

    class NetworkFramework : NetworkManager
    {
        public override void getMessage()
        {
            (NetClient, string) m;
            NetworkGame game = ((NetworkGame)Bootstrap.getRunningGame());
            while (this.pollMessage(out m) >= 0)
            {

                string[] message = m.Item2.Split(";");
                NetClient c = m.Item1;
                if(message[0] == "NET")
                {
                    if(message[1] == "NEW") {
                        this.addClient(c);
                        string state = game.getFullState();
                        c.send("NET;STATE;" + state);
                        this.informClients(c, "NET;ADD;" + c.id.ToString());

                        Message a = new Message();
                        a.type = "ADD";
                        a.message = c.id.ToString();
                        game.updateState(a);
                    }
                    else if(message[1] == "ACC")
                    {
                        int id = Int32.Parse(message[2]);
                        this.id = id;

                        Message a = new Message();
                        a.type = "ACC";
                        a.message = message[2];
                        game.updateState(a);
                    }
                    else if(message[1] == "STATE") {
                        game.spawnInState(message[2]);
                    }
                    else if(message[1] == "ADD") {
                        Message a = new Message();
                        a.type = "ADD";
                        a.message = message[2];
                        game.updateState(a);
                    }
                    else if(message[1] == "DEAD") {
                        if(this.IsHost)
                        {
                            this.removeClient(c);
                            this.informClients(c, "NET;DEL;" + c.id.ToString());

                            Message a = new Message();
                            a.type = "DEL";
                            a.message = c.id.ToString();
                            game.updateState(a);
                        }
                        else if(this.IsClient)
                        {//TODO: on disconnect
                            this.stopClient();
                        }
                    }
                    else if(message[1] == "DEL") {
                        Message a = new Message();
                        a.type = "DEL";
                        a.message = message[2];
                        game.updateState(a);
                    }
                }
                else if(message[0] == "MESSAGE")
                {
                    if(this.IsHost)
                        this.informClients(null, "MESSAGE;" + message[1]);
                    Message a = new Message();
                    a.type = "MESSAGE";
                    a.message = message[1];
                    ((NetworkGame)Bootstrap.getRunningGame()).updateState(a);
                }
            }
        }

        public override void initialize()
        {
        }

        public override void informState()
        {
            NetworkGame game = ((NetworkGame)Bootstrap.getRunningGame());
            string state = game.getState();
            if(this.IsHost)
                this.informClients(null, "MESSAGE;" + state);
            if(this.IsClient)
                this.informHost("MESSAGE;" + state);
        }
    }
}


