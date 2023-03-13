
using System;
using System.Collections.Generic;

namespace Shard
{
    class NetworkObject : GameObject
    {
        public  bool isOwner;
        public int id;

        public NetworkObject(bool own, int i) : base()
        {
            isOwner = own;
            id = i;
        }

        public virtual void disown(){}
        public virtual void updateState(Message m){}
    }
}

