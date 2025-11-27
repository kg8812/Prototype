using System.Collections.Generic;
using System.Linq;
using Apis.DataType;

namespace Apis
{
    public class WeaponData : Database
    {
        private static Datas datas = new();
        private static bool isInit;

        public static Dictionary<int, WeaponDataType> dataDict = new();
        public static Dictionary<int, MotionDataType> motionDict = new();
        public static Dictionary<int, MotionGroupDataType> motionGroupDict = new();

        public static Datas DataLoad
        {
            get
            {
                if (GameManager.Data == null) return null;

                return datas ??= new Datas();
            }
        }

        public override void ProcessDataLoad()
        {
            if (!isInit)
            {
                isInit = true;
                {
                    var dict = GameManager.Data.GetDataTable<WeaponDataType>(DataTableType.Weapon);
                    dataDict = dict.ToDictionary(x => int.Parse(x.Key), x => x.Value);
                }
                {
                    motionDict = GameManager.Data.GetDataTable<MotionDataType>(DataTableType.Motion)
                        .ToDictionary(x => int.Parse(x.Key), x => x.Value);
                    motionGroupDict = GameManager.Data.GetDataTable<MotionGroupDataType>(DataTableType.MotionGroup)
                        .ToDictionary(x => int.Parse(x.Key), x => x.Value);
                }
            }
        }

        public class Datas
        {
            public bool TryGetWeaponData(int id, out WeaponDataType data)
            {
                return dataDict.TryGetValue(id, out data);
            }

            public bool TryGetMotion(int id, out MotionDataType data)
            {
                return motionDict.TryGetValue(id, out data);
            }

            public bool TryGetMotionGroup(int id, out MotionGroupDataType data)
            {
                return motionGroupDict.TryGetValue(id, out data);
            }
        }
    }
}