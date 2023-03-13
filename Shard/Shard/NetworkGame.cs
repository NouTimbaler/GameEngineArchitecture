namespace Shard
{
    abstract class NetworkGame : Game
    {
        public abstract void updateState(Message message);
        public abstract string getState();
        public abstract string getFullState();
        public abstract void spawnInState(string state);
        public abstract void onDisconnect();
    }
}
