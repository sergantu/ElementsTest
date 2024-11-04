using System;

namespace CodeBase.PersistentProgress
{
    [Serializable]
    public class PlayerProgress
    {
        public PlayerProgress()
        {
            GameData = new GameData();
        }

        public GameData GameData { get; set; }
    }
}