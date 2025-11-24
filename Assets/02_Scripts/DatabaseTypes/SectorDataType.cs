using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Apis.DataType
{

    public enum SectorType
    {
        NonBattle, Battle, Shop, Elite, Boss
    }
    
    [System.Serializable]
    public class SectorDataType
    {
        public int sectorId;
        public int reward;
        public string iconImg;
        [JsonConverter(typeof(StringEnumConverter))]
        public SectorType sectorType;
    }
    
    [System.Serializable]
    public class FloorDataType
    {
        // public int floorId;
        public int stage;
        public int floor;
        public int selection;
        public int[] sectorId;
        public int[] chance;
    }
}