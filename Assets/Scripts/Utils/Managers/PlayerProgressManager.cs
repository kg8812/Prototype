using System;
using Apis.DataType;

namespace Managers
{
    public class PlayerProgressManager
    {
        private int _level = 1;
        private int _exp;

        public Action<int> levelChange;
        public Action<int> expChange;

        public int Level
        {
            get => _level;
            set
            {
                if (value <= 0)
                    _level = 1;
                else if (value > 100)
                    _level = 100;
                else
                    _level = value;
                levelChange?.Invoke(_level);
            }
        }

        public int Exp
        {
            get => _exp;
            set
            {
                if (value < 0) return;

                _exp = value;
                var levelData = LevelDatabase.GetLevelData(_level);
                if (levelData == null)
                {
                    expChange?.Invoke(_exp);
                    return;
                }

                var maxExp = levelData.exp;
                while (_exp >= maxExp)
                {
                    _exp -= maxExp;
                    Level++;
                    levelData = LevelDatabase.GetLevelData(Level);
                    if (levelData == null) break;
                    maxExp = levelData.exp;
                }

                expChange?.Invoke(_exp);
            }
        }
    }
}
