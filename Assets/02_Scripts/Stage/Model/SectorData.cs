using Apis.DataType;
using System.Collections.Generic;
using System.Linq;
using Apis.Util;
using UnityEngine;


namespace Apis.Model
{
    public class FloorCleanUpData
    {
        public int stage;
        public int floor;
        public int selection;
        public SectorChance[] sectorChances;
    }

    public class SectorChance: HasChance
    {
        public int sectorId;
        public int chance { get; set; }

        public SectorChance(int sectorId, int chance)
        {
            this.sectorId = sectorId;
            this.chance = chance;
        }
    }
    public class SectorData : Database
    {
        // 나중에 define 클래스로 넘기던가 거시기 처리 해야됨
        private const int stageCnt = 7;
        
        public static Dictionary<int, SectorDataType> sectorDict;
        public static List<Dictionary<int, FloorCleanUpData>> floorList;

        

        public override void ProcessDataLoad()
        {
            sectorDict = GameManager.Data.GetDataTable<SectorDataType>(DataTableType.Sector)
                .ToDictionary(x => int.Parse((x.Key)), x => x.Value);
            
            {
                floorList = new();
                for (var i = 0; i < stageCnt; i++)
                {
                    floorList.Add(new Dictionary<int, FloorCleanUpData>());
                }
                foreach (var value in GameManager.Data.GetDataTable<FloorDataType>(DataTableType.Floor))
                {
                    if (value.Value.sectorId.Length != value.Value.chance.Length)
                    {
                        Debug.LogError($"{value.Key}번째 floor 데이터에서 sectorId와 chance의 개수가 동일하지 않습니다.");
                    }
                    FloorCleanUpData floorCleanUpData = new FloorCleanUpData();
                    // floorCleanUpData.floorId = value.Value.floorId;
                    floorCleanUpData.stage = value.Value.stage;
                    floorCleanUpData.floor = value.Value.floor;
                    floorCleanUpData.selection = value.Value.selection;

                    SectorChance[] sectorChances = new SectorChance[value.Value.sectorId.Length];
                    for (int i = 0; i < value.Value.sectorId.Length; i++)
                    {
                        sectorChances[i] = new SectorChance(value.Value.sectorId[i], value.Value.chance[i]);
                    }
                    floorCleanUpData.sectorChances = sectorChances;
                    floorList[value.Value.stage-1].Add(value.Value.floor, floorCleanUpData);
                }
            }
        }
    }
}