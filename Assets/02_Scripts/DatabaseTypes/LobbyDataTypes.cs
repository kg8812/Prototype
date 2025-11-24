namespace Apis.DataType
{
    [System.Serializable]
    public class ConvenienceDataType
    {
        public int index;
        public string name;
        public string desc;
        public int priceWarmth;
        public int unlock;
        public int[] amounts;
    }

    [System.Serializable]
    public class GrowthDataType
    {
        public int index;
        public ActorStatType statGroup;
        public int lvl;
        public int value;
        public int addValue;
        public int cost;
    }
}