using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NewNewInvenSpace
{
    
    [Serializable]
    public enum PresetType
    {
        InvenPreset, // 직접 프리셋
        OverridePreset // 간접 프리셋
    }

    [Serializable]
    public enum InvenGroupType
    {
        AtkItem,
        Acc
    }
    
    [Serializable]
    public struct Preset
    {
        public PresetType type;
        public PresetBlock[] Blocks;
        public Preset(PresetType type, PresetBlock[] blocks)
        {
            this.type = type;
            Blocks = blocks;
        }
    }

    [Serializable]
    public struct PresetBlock
    {
        public InvenGroupType invenGroupType;
        public int[] presets;

        public PresetBlock(int count, InvenGroupType invenGroupType)
        {
            presets = new int[count];
            this.invenGroupType = invenGroupType;
        }
    }
    
    [InfoBox(
        @"프리셋 인덱스 목록
0 ~ 5 : 캐릭터별 프리셋 (아징릴주고비)
6 : 릴파 스킬
7 : 주르르
8 : 고세구 메카
9 : 비챤 야수
10 : 아이네 스킬
11 : 아이네 달의 영역")]
    [CreateAssetMenu(menuName = "Scriptable/Datas/PresetData", fileName="New PresetData")]
    public class PresetData: ScriptableObject
    {

        /* 프리셋 인덱스 목록
         * 0 ~ 5 : 캐릭터별 프리셋 (아징릴주고비)
         * 6 : 릴파 스킬
         * 7 : 주르르
         * 8 : 고세구 메카
         * 9 : 비챤 야수
         * 10 : 아이네 스킬
         * 11 : 아이네 달의 영역
         */

        public const int AtkCnt = 4;
        public const int AccCnt = 12;
        
        [Serializable]
        public class IntPresetDictionary: UnitySerializedDictionary<int, Preset> {}

        public IntPresetDictionary Presets;
    }
}