using System;
using System.Net;
using System.Net.Sockets;

using System.Text;
using System.Threading;


namespace Shard
{
    class NetListener
    {
        private readonly int PORT;

        private TcpListener socket;
        private IPEndPoint listenerEP;

        public void initialize()
        {
            listenerEP = new IPEndPoint(IPAddress.Any, PORT);
            socket = new TcpListener(listenerEP);
            socket.Start(100);
        }

        public NetListener(int port)
        {
            PORT = port;
            listenerEP = new IPEndPoint(IPAddress.None, port);
        }

        public void waitForConnections()
        {
            NetworkManager nm = Bootstrap.getNetworkManager();
            while(nm.IsHost)
            {
                try
                {
                    TcpClient tcpClient = socket.AcceptTcpClient();

                    //Add new client
                    NetClient p = new NetClient(tcpClient);
                    string s = "NET;NEW";
                    nm.addMessage(p, s);
                }
                catch
                {
                    nm.stopHost();
                }
            }
        }
        public void stop()
        {
            socket.Stop();
        }
    }


    class NetClient
    {
        public int id {get; set; }
        private readonly TcpClient socket;
        private Stream stream;
        private readonly bool isHost;
        private readonly IPEndPoint ep;

        private static string separator = "<end>";

        public NetClient(TcpClient sock)
        {
            socket = sock;
            stream = sock.GetStream();
            isHost = true;
        }
        public NetClient(IPEndPoint e)
        {
            socket = new TcpClient();
            ep = e;
            isHost = false;
        }
        public bool connect()
        {
            if(isHost) return false;
            try
            {
                socket.Connect(ep);
                stream = socket.GetStream();
                return true;
            }
            catch(Exception e)
            {
                Debug.getInstance().log(e.ToString());
                return false;
            }

        }
        public void send(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message + separator);
            stream.Write(buffer, 0, buffer.Length);
        }
        public void start()
        {
            Thread t = new Thread(recieveLoop);
            t.IsBackground = true;
            t.Start();
        }
        public void recieveLoop()
        {
            byte[] buffer = new byte[2048];
            NetworkManager nm = Bootstrap.getNetworkManager();
            try
            {
                bool control = true;
                string data = "";
                int index = -1;
                while(true && control)
                {
                    while (index < 0)
                    {
                        int recievedBytes;
                        recievedBytes = stream.Read(buffer, 0, buffer.Length);
                        if (recievedBytes < 1){
                            control = false;
                            break;
                        }
                        data = data + Encoding.UTF8.GetString(buffer, 0, recievedBytes);
                        index = data.IndexOf(separator);
                    }
                    if(!control) break;

                    while (index > 0)
                    {
                        string message = data.Substring(0, index);
                        if (message.Length > 0) nm.addMessage(this, message);

                        data = data.Substring(index + separator.Length);
                        index = data.IndexOf(separator);
                    }
                }
            }
            catch(Exception ex) //IOException ObjectDisposedException
            {
                Debug.getInstance().log("NET: Client communication error");
            }
            nm.addMessage(this, "NET;DEAD");
            Debug.getInstance().log("NET: Client disconnected");
        }
        public void stop()
        {
            socket.Close();
        }
    }

    class NetworkManager
    {
        private NetListener host;
        private NetClient client;
        protected readonly Object informLock = new Object();
        protected readonly Object queueLock = new Object();

        private bool isHost;
        public bool IsHost {get => isHost;}
        private bool isClient;
        public bool IsClient {get => isClient;}

        public int id {get; protected set; }

        private List<NetClient> clients;

        private Queue<(NetClient, string)> messages;

        public NetworkManager()
        {
            isHost = false;
            isClient = false;
            clients = new List<NetClient>();
            messages = new Queue<(NetClient, string)>();
            id = 0;

        }

        protected int pollMessage(out (NetClient, string) m)
        {
            int c = -1;
            lock(queueLock)
            {
                m = (null, "");
                if (messages.Count < 1) return c;
                m = messages.Dequeue();
                c = messages.Count;
            }
            return c;
        }

        public void addMessage(NetClient p, string m)
        {
            lock(queueLock)
                messages.Enqueue((p, m));
        }

        public bool active()
        {
            return isHost || isClient;
        }

        public virtual void initialize(){}
        public virtual void getMessage(){}
        public virtual void informState(){}


        // HOST methods
        public bool makeHost()
        {
            if(isHost || isClient) return false;

            int port;
            if (!Bootstrap.checkEnvironmentalVariable("port")) return false;
            port = int.Parse(Bootstrap.getEnvironmentalVariable("port"));

            Debug.getInstance().log("NET: Im host");
            host = new NetListener(port);
            host.initialize();
            isHost = true;
            id = 0;

            //start new thread for the listen for connections loop
            Thread t = new Thread(host.waitForConnections);
            t.IsBackground = true;
            t.Start();

            return true;
        }

        public void stopHost()
        {
            if (!isHost) return;
            host.stop(); 
            lock(informLock)
            {
                foreach (NetClient client in clients)
                {
                    client.stop();
                }
                clients.Clear();
            }
            isHost = false;
            lock(queueLock)
                messages.Clear();
        }

        protected void informClients(NetClient sender, string m)
        {
            if (!isHost) return;

            lock(informLock)
            {
                foreach (NetClient client in clients)
                {
                    if(client.Equals(sender)) continue;
                    try
                    {
                        client.send(m);
                    }
                    catch
                    {
                        addMessage(client, "NET;DEAD");
                    }
                }
            }
        }

        protected void addClient(NetClient client)
        {
            client.id = getNewId();
            client.send("NET;ACC;" + client.id.ToString()); //IMPORTANT ACCEPTED Message
            Debug.getInstance().log("NET: New connection accepted");
            Debug.getInstance().log("NET: Client added");
            lock(informLock)
                clients.Add(client);
            client.start();
        }
        protected void removeClient(NetClient client)
        {
            Debug.getInstance().log("NET: Client removed");
            lock(informLock)
                clients.Remove(client);
            client.stop();
        }

        private int getNewId()
        {
            int n = 0;
            bool b = false;
            while (!b)
            {
                n += 1;
                b = true;
                foreach (NetClient client in clients)
                {
                    if(client.id == n) b = false;
                }
            }
            return n;
        }


        // CLIENT methods
        public bool makeClient()
        {
            if(isHost || isClient) return false;

            int port;
            if (!Bootstrap.checkEnvironmentalVariable("port")) return false;
            port = int.Parse(Bootstrap.getEnvironmentalVariable("port"));

            string address;
            if (!Bootstrap.checkEnvironmentalVariable("ip")) return false;
            address = Bootstrap.getEnvironmentalVariable("ip");

            IPAddress ip;
            if(address == "loopback") ip = IPAddress.Loopback;
            else ip = IPAddress.Parse(address);

            if (!(ip.AddressFamily.ToString() == "InterNetwork")) return false;

            var ep = new IPEndPoint(ip, port);
            client = new NetClient(ep);

            if (!client.connect()) return false;

            Debug.getInstance().log("NET: Im client");
            client.start();
            isClient = true;
            return true;
        }

        protected void informHost(string s)
        {
            if(!isClient) return;
            try
            {
                client.send(s);
            }
            catch 
            { 
                addMessage(client, "NET;DEAD");
            }
        }

        public void stopClient()
        {
            if (!isClient) return;
            client.stop();
            isClient = false;
            lock(queueLock)
                messages.Clear();
        }
    }
}

