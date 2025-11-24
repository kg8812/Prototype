using Apis.Managers;
using Default;

namespace Apis
{
    public class StrUtil
    {
        #region 카테고리

        public const int UICategory = 10;
        public const int EquipNameCategory = 11;
        public const int SkillTreeNameCategory = 12;
        public const int TitleCategory = 13;
        public const int BuffNameCategory = 14;
        public const int MonsterNameCategory = 15;
        public const int MapBoxNameCategory = 16;
        public const int ObjectNameCategory = 17;
        public const int TagNameCategory = 18;

        public const int FlavorTextCategory = 20;
        public const int EquipDescCategory = 21;
        public const int SkillTreeDescCategory = 22;
        public const int ContentCategory = 23;
        public const int BuffDescCategory = 24;
        public const int TagDescCategory = 28;

        #endregion
        
        


        public static string GetEquipmentName(int equipId) =>
            LanguageManager.Str(Calc.ConcatInts(EquipNameCategory, equipId));
        
        public static string GetFlavorText(int mainId) =>
            LanguageManager.Str(Calc.ConcatInts(FlavorTextCategory, mainId));
        
        
        public static string GetTagName(int tagId) => 
            LanguageManager.Str(Calc.ConcatInts(TagNameCategory, tagId));
        
        public static string GetEquipmentDesc(int equipId) =>
            LanguageManager.Str(Calc.ConcatInts(EquipDescCategory, equipId));
        
        public static string GetTagDesc(int tagId) => 
            LanguageManager.Str(Calc.ConcatInts(TagDescCategory, tagId));
    }
}