using System.Collections.Generic;
using System.Text;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NewNewInvenSpace
{
    public class PresetManager
    {
        private readonly Dictionary<InvenGroupType, InventoryGroup> _groups;
        // private readonly Dictionary<string, Item> _overrideItem;

        public readonly OverrideItemGetter OverrideItems;

        private int _curInvenPreset = -1;

        public PresetManager(Dictionary<InvenGroupType, InventoryGroup> groups)
        {
            _groups = groups;
            OverrideItems = new OverrideItemGetter();
            // preset data init
            PresetData = Object.Instantiate(ResourceUtil.Load<PresetData>("ScriptableObjects/Datas/BasePresetData"));

            foreach (var value in PresetData.Presets)
                if (value.Value.type == PresetType.OverridePreset)
                    for (var i = 0; i < value.Value.Blocks.Length; i++)
                    {
                        var curBlock = value.Value.Blocks[i];

                        for (var j = 0; j < curBlock.presets.Length; j++) AddNewOverrideItem(curBlock.presets[j]);
                    }

            foreach (var keyValue in groups) ToggleSavePreset(keyValue.Value, true);
        }

        public PresetData PresetData { get; }

        public Item AddNewOverrideItem(int skillItemId)
        {
            return OverrideItems.AddNewOverrideItem(skillItemId);
        }

        private void SavePresetWithPlayerType(int ind, Item item)
        {
            SavePreset((int)GameManager.instance.Player.playerType);
        }

        private void SaveOverridePresetWithPlayerType(int ind, Item item)
        {
            // active skill 거시기.
            SavePreset((int)GameManager.instance.Player.playerType + 6);
        }

        private void ToggleSavePreset(InventoryGroup ig, bool isOn)
        {
            if (isOn)
            {
                if (ig.Invens.TryGetValue(InvenType.Equipment, out var value1))
                    value1.OnSlotChanged += SavePresetWithPlayerType;
                if (ig.Invens.TryGetValue(InvenType.Hidden, out var value2))
                    value2.OnSlotChanged += SaveOverridePresetWithPlayerType;
            }
            else
            {
                if (ig.Invens.TryGetValue(InvenType.Equipment, out var value1))
                    value1.OnSlotChanged -= SavePresetWithPlayerType;
                if (ig.Invens.TryGetValue(InvenType.Hidden, out var value2))
                    value2.OnSlotChanged -= SaveOverridePresetWithPlayerType;
            }
        }

        /// <summary>
        ///     프리셋 인덱스 목록
        ///     0 ~ 5 : 캐릭터별 프리셋 (아징릴주고비)
        ///     6 : 릴파 스킬
        ///     7 : 주르르
        ///     8 : 고세구 메카
        ///     9 : 비챤 야수
        ///     10 : 아이네 스킬
        ///     11 : 아이네 달의 영역
        /// </summary>
        /// <param name="index"></param>
        public void ApplyPreset(int index)
        {
            if (PresetData.Presets.TryGetValue(index, out var preset))
            {
                // TODO: 999칸이 모두 안채워진다는 가정하에 unequip하는거라 나중에 temp 처리해줘야 함.
                // 말이 999칸이지 무한으로 만들어달라는뜻이라 다 찰일 없게 해야합니다.

                for (var i = 0; i < preset.Blocks.Length; i++)
                {
                    var curBlock = preset.Blocks[i];
                    var ig = _groups[curBlock.invenGroupType];

                    // save preset 해제

                    var prevType = ig.PresetType;
                    var nextType = preset.type;


                    // unequip section
                    if (prevType == PresetType.OverridePreset)
                        for (var j = 0; j < ig.Invens[InvenType.Hidden].Count; j++)
                            ig.Invens[InvenType.Hidden].Remove(j);

                    if (nextType == PresetType.InvenPreset)
                        if (prevType == PresetType.InvenPreset && _curInvenPreset != index)
                            ig.MoveInvenTypeAll(InvenType.Equipment, InvenType.Storage);


                    if (prevType != preset.type) ig.PresetType = preset.type;

                    // equip section
                    if (nextType == PresetType.InvenPreset)
                        for (var j = 0; j < curBlock.presets.Length; j++)
                        {
                            var itemId = curBlock.presets[j];
                            if (itemId == 0) continue;
                            var originId = ig.Invens[InvenType.Equipment][j]?.ItemId ?? 0;

                            // 원래라면 기존 프리셋으로 변경했다면 문제 x
                            // 하지만 기존 프리셋이 변경되었다면, 틀린 부분만 바로잡기
                            if (originId != itemId)
                            {
                                // 장착되어있는게 잘못됨 -> 장착되어있는 거 해제 + 
                                if (originId != 0)
                                    ig.MoveInvenType(j, InvenType.Equipment, InvenType.Storage);
                                var ind = ig.Invens[InvenType.Storage].FindById(originId);
                                if (ind < 0)
                                {
                                    Debug.Log($"{StrUtil.GetEquipmentName(itemId)}를 가지고 있지 않음");
                                }
                                else
                                {
                                    var item = ig.Invens[InvenType.Storage].Remove(ind);
                                    ig.Add(j, item, InvenType.Equipment);
                                }
                            }
                        }
                    else
                        for (var j = 0; j < curBlock.presets.Length; j++)
                        {
                            var itemId = curBlock.presets[j];
                            if (itemId == 0) continue;
                            var eqItem = OverrideItems.GetItemFromStorage(itemId);
                            if (eqItem != null)
                                ig.Add(j, eqItem, InvenType.Hidden);
                            else
                                Debug.LogError($"{StrUtil.GetEquipmentName(itemId)}의 아이템이 미리 없음.");
                        }
                }

                if (preset.type == PresetType.InvenPreset) _curInvenPreset = index;
            }
        }

        public void SavePreset(int index)
        {
            if (PresetData.Presets.TryGetValue(index, out var preset))
            {
                for (var i = 0; i < preset.Blocks.Length; i++)
                {
                    var curBlock = preset.Blocks[i];
                    var ig = _groups[curBlock.invenGroupType];

                    for (var j = 0; j < curBlock.presets.Length; j++) curBlock.presets[j] = 0;

                    for (var j = 0; j < curBlock.presets.Length; j++)
                        if (ig.Invens.TryGetValue(
                                preset.type == PresetType.InvenPreset ? InvenType.Equipment : InvenType.Hidden,
                                out var value))
                        {
                            if (j >= value.Count || value[j] == null) continue;
                            var itemId = value[j].ItemId;
                            curBlock.presets[j] = itemId;
                        }
                        else
                        {
                            Debug.LogError(
                                $"{(preset.type == PresetType.InvenPreset ? InvenType.Equipment : InvenType.Hidden)}타입의 inven이 없음.{index} ");
                        }

                    preset.Blocks[i] = curBlock;
                }

                PresetData.Presets[index] = preset;
            }
        }

        /// <summary>
        ///     특정 위치의 프리셋 아이템 변경.
        ///     만약 간접 프리셋의 아이템이면 아이템 자동 추가.
        ///     슬롯을 비우고 싶을 때는 itemName에 null넣으면 됨
        /// </summary>
        /// <param name="item">만약 외부에서 미리 item을 생성하는 경우, 여기에 item 넣으면 됨.</param>
        public void ModifyPresetItem(int index, int blockInd, int slotInd, int itemId, Item item = null)
        {
            if (PresetData.Presets.TryGetValue(index, out var preset))
            {
                if (preset.Blocks.Length > blockInd)
                {
                    if (preset.Blocks[blockInd].presets.Length > slotInd)
                    {
                        Debug.Log($"preset {blockInd} {slotInd}을 {itemId}으로 수정.");
                        preset.Blocks[blockInd].presets[slotInd] = itemId;
                        PresetData.Presets[index] = preset;
                        if (preset.type == PresetType.OverridePreset && itemId != 0)
                        {
                            if (item == null)
                                OverrideItems.AddNewOverrideItem(itemId);
                            else
                                OverrideItems.RegisterExternalItem(item);
                        }
                    }
                    else
                    {
                        Debug.LogError($"modify preset item: 해당 프리셋 블록에 {slotInd}번째 slot이 존재하지 않음.");
                    }
                }
                else
                {
                    Debug.LogError($"modify preset item: 해당 프리셋에 {blockInd}번째 preset block이 존재하지 않음.");
                }
            }
            else
            {
                Debug.LogError($"modify preset item: 해당 {index} index의 프리셋이 존재하지 않음.");
            }
        }

        public Item GetOverrideItem(int itemId)
        {
            if (itemId == 0) return null;
            return OverrideItems.GetItem(itemId);
        }

        public List<Item> GetOverrideItems(int index, int blockInd)
        {
            List<Item> items = new();

            if (PresetData.Presets.TryGetValue(index, out var preset))
                if (preset.Blocks.Length > blockInd)
                    foreach (var itemName in preset.Blocks[blockInd].presets)
                        items.Add(GetOverrideItem(itemName));

            return items;
        }

        [Button]
        public void PrintPreset()
        {
            foreach (var keyValue in PresetData.Presets)
            {
                var sb = new StringBuilder();
                sb.Append(keyValue.Key);
                sb.Append(" ");
                foreach (var block in keyValue.Value.Blocks)
                {
                    foreach (var preset in block.presets) sb.Append($"{preset} ");

                    sb.Append(" / ");
                }

                Debug.Log($"preset-{sb}");
            }
        }
    }
}