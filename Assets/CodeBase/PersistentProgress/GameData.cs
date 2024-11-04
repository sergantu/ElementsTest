using System;
using System.Collections.Generic;

namespace CodeBase.PersistentProgress
{
    [Serializable]
    public class GameData
    {
        private List<List<string>> _elementsField;
        public int CurrentLevel { get; set; }
        public List<List<string>> ElementsField { get; set; }

        public GameData()
        {
            CurrentLevel = 0;
        }
    }
}