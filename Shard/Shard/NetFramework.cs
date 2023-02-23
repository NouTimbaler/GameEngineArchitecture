using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using System.Text.Json;
using System.Text.Json.Serialization;


namespace Shard
{
    class Message
    {
        public int id {get; set; }
        public int time {get; set; }
        public string message {get; set; }
    }

    class NetHost
    {
        public const int PORT = 6000;

        private Socket socket;
        private EndPoint hostEp;
        private EndPoint remote;

        public byte[] data;

        public void initialize()
        {
            hostEp = new IPEndPoint(IPAddress.Any, PORT);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Blocking = false;
            socket.Bind(hostEp);
        }

        public NetHost()
        {
            data = new byte[4096];
        }

        public void recieve()
        {
            if (socket.Available <= 0) return;
            remote = hostEp;
            int recv = socket.ReceiveFrom(data, ref remote);
            if (recv > 0)
            {

                //TODO: check if recv is smaller than available
                var readOnlySpan = new ReadOnlySpan<byte>(data);
                Message m = JsonSerializer.Deserialize<Message>(readOnlySpan.Slice(0, recv))!;

                string dataRecieved = m.message;
                Debug.getInstance().log("HOST: From: " + remote.ToString() + " Message: " + dataRecieved);
                sendTo(remote, new Message{ id = 0, time = 0, message = "Hello back!" } );
            }
        }

        public void sendTo(EndPoint recipient, Message data)
        {
            byte[] toSend = JsonSerializer.SerializeToUtf8Bytes(data);
            socket.SendTo(toSend, toSend.Length, SocketFlags.None, recipient);
        }

    }

    class NetClient
    {
        private Socket socket;
        private EndPoint hostEp;
        private EndPoint remote;

        private byte[] data;

        public NetClient()
        {
            data = new byte[4096];
        }

        public void initialize(IPAddress address, int port)
        {
            hostEp = new IPEndPoint(address, port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void recieve()
        {
            if (socket.Available <= 0) return;
            remote = hostEp;
            int recv = socket.ReceiveFrom(data, ref remote);
            if (recv > 0)
            {
                var readOnlySpan = new ReadOnlySpan<byte>(data);
                Message m = JsonSerializer.Deserialize<Message>(readOnlySpan.Slice(0, recv))!;

                string dataRecieved = m.message;
                Debug.getInstance().log("CLIENT: From: " + remote.ToString() + " Message: " + dataRecieved);
            }
        }

        public void sendTo(Message data)
        {
            Debug.getInstance().log(JsonSerializer.Serialize(data));
            byte[] toSend = JsonSerializer.SerializeToUtf8Bytes(data);
            socket.SendTo(toSend, toSend.Length, SocketFlags.None, hostEp);
        }
    }
}

