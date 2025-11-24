using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using UnityEngine;

public class AtkItemData
{
    public enum PresetType
    {
        InvenPreset,    // 직접 프리셋
        OverridePreset  // 간접 프리셋
    }
    public struct Preset
    {
        public readonly PresetType presetType;
        public readonly string[] atkItems;

        public Preset(int count, PresetType presetType)
        {
            this.presetType = presetType;
            atkItems = new string[count];
        }
    }

    /* 프리셋 인덱스 목록
     * 0 ~ 5 : 캐릭터별 프리셋 (아징릴주고비)
     * 6 : 릴파 스킬
     * 7 : 주르르
     * 8 : 고세구 메카
     * 9 : 비챤 야수
     */
    Dictionary<int, Preset> _presets;
    public Dictionary<int,Preset> Presets
    {
        get
        {
            if (_presets == null)
            {
                _presets = new()
                {
                    { 0, new Preset(AttackItemManager.MaxCount, PresetType.InvenPreset) },
                    { 1, new Preset(AttackItemManager.MaxCount, PresetType.InvenPreset) },
                    { 2, new Preset(AttackItemManager.MaxCount, PresetType.InvenPreset) },
                    { 3, new Preset(AttackItemManager.MaxCount, PresetType.InvenPreset) },
                    { 4, new Preset(AttackItemManager.MaxCount, PresetType.InvenPreset) },
                    { 5, new Preset(AttackItemManager.MaxCount, PresetType.InvenPreset) },
                    { 6, new Preset(AttackItemManager.MaxCount, PresetType.OverridePreset) },
                    { 7, new Preset(AttackItemManager.MaxCount, PresetType.OverridePreset) },
                    { 8, new Preset(AttackItemManager.MaxCount, PresetType.OverridePreset) },
                    { 9, new Preset(AttackItemManager.MaxCount, PresetType.OverridePreset) },
                };
                _presets[6].atkItems[0] = "LilpaWeapon";
            }

            return _presets;
        }
    }
    
}
