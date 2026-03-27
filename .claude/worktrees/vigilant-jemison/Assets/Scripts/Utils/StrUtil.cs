using System;
using Apis.Managers;
using Default;

namespace Apis
{
    public class StrUtil
    {
        public static string GetEquipmentName(int equipId)
        {
            return LanguageManager.Str(Calc.ConcatInts(EquipNameCategory, equipId));
        }

        public static string GetFlavorText(int mainId,int subId) =>
            LanguageManager.Str(Calc.ConcatInts(FlavorTextCategory, (int)Math.Pow(10, Calc.GetDigits(mainId)) * subId + mainId));

        
        public static string GetEquipmentDesc(int equipId)
        {
            return LanguageManager.Str(Calc.ConcatInts(EquipDescCategory, equipId));
        }
        

        #region 카테고리

        public const int UICategory = 10;
        public const int EquipNameCategory = 11;
        public const int SkillTreeNameCategory = 12;
        

        public const int FlavorTextCategory = 20;
        public const int EquipDescCategory = 21;
        public const int SkillTreeDescCategory = 22;

        #endregion
    }
}