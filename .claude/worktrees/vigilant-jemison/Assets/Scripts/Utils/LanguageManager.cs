using System;
using System.Collections.Generic;
using System.Linq;
using Apis.DataType;
using Save.Schema;
using TMPro;

namespace Apis.Managers
{
    public class LanguageManager : Database
    {
        public static Action<LanguageType> LanguageTypeChanged;
        private static Dictionary<int, StringDataType> _stringDataTable;

        private static Dictionary<TextMeshProUGUI, (int, Func<string, string>)> _textMeshes;
        public static LanguageType LanguageType => DataAccess.Settings.Data.languageType;

        public override void ProcessDataLoad()
        {
            if (_stringDataTable != null) return;
            _stringDataTable = GameManager.Data.GetDataTableInJson<StringDataType>("StringTable")
                .ToDictionary(x => int.Parse(x.Key), x => x.Value);

            _textMeshes = new Dictionary<TextMeshProUGUI, (int, Func<string, string>)>();
            LanguageTypeChanged += _ =>
            {
                if (_textMeshes != null)
                    foreach (var textMesh in _textMeshes.Keys)
                        UpdateText(textMesh);
            };
        }

        public static string Str(int index)
        {
            if (_stringDataTable.TryGetValue(index, out var data))
                return LanguageType switch
                {
                    LanguageType.Korean => data.kr,
                    LanguageType.English => data.en,
                    _ => data.kr
                };

            return string.Empty;
        }

        public static void ChangedLanguageType(LanguageType type)
        {
            if (LanguageType == type) return;
            DataAccess.Settings.Data.languageType = type;
            LanguageTypeChanged?.Invoke(type);
            DataAccess.GameData.Save();
        }


        #region TextMeshProUGUI 관리

        public static void RegisterText(TextMeshProUGUI textMesh, int index, Func<string, string> transformer = null)
        {
            if (_textMeshes.ContainsKey(textMesh))
                _textMeshes[textMesh] = (index, transformer);
            else
                _textMeshes.TryAdd(textMesh, (index, transformer));
            UpdateText(textMesh);
        }

        private static void UpdateText(TextMeshProUGUI textMesh)
        {
            if (_textMeshes.TryGetValue(textMesh, out var value))
            {
                var strValue = Str(value.Item1);
                if (value.Item2 != null)
                    strValue = value.Item2(strValue);
                textMesh.text = strValue;
            }
        }

        public static bool UnregisterText(TextMeshProUGUI textMesh)
        {
            return _textMeshes.Remove(textMesh);
        }

        #endregion
    }
}