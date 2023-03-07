
using System;
using System.Collections.Generic;

namespace Shard
{
    class NetworkObject : GameObject
    {
        public  bool isOwner;

        public NetworkObject(bool own) : base()
        {
            isOwner = own;
        }

        public virtual void disown(){}
        public virtual void updateState(Message m){}
    }
}

