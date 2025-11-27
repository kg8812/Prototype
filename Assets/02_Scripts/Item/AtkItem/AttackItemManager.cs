using NewNewInvenSpace;
using UnityEngine;

namespace Apis
{
    public static class AttackItemManager
    {
        public const int MinCount = 2;
        public const int MaxCount = 5;

        private static int _count = 2;

        public static IAttackItem CurrentItem;

        private static AtkItemInventoryGroup _atkInven;


        // 무기를 여러개 장착할 수 있게 됨으로써, 현재 사용중인 무기와 스킬을 할당하는 형식으로 변경함
        // 이 변수들을 사용해서 스킬을 캔슬하거나 쿨타임을 초기화하는 등의 효과들을 구현하도록 임시 변경하였음.
        // 이것과 관련해서 처리가 후에 필요함

        private static AtkItemData _atkItemData;

        public static int Count
        {
            get => _count;
            set
            {
                _count = Mathf.Clamp(value, MinCount, MaxCount);
                InvenManager.instance.AttackItem.AtkItemInven.Count = _count;
            }
        }

        public static AtkItemInventoryGroup AtkInven => _atkInven ??= InvenManager.instance.AttackItem;
        public static AtkItemData AtkItemData => _atkItemData ??= new AtkItemData();

        /// <summary>
        ///     0 ~ 5 : 캐릭터별 프리셋 (아징릴주고비)
        ///     6 : 릴파 스킬
        ///     7 : 주르르 주폭도
        ///     8 : 고세구 메카
        ///     9 : 비챤 야수
        ///     10 : 아이네 스킬
        /// </summary>
        public static void ApplyPreset(int index)
        {
            InvenManager.instance.PresetManager.ApplyPreset(index);
        }

        public static void SavePreset(int index)
        {
            InvenManager.instance.PresetManager.SavePreset(index);
        }

        // public static void ApplyPreset(int index)
        // {
        //     if (AtkItemData.Presets.TryGetValue(index, out var preset))
        //     {
        //         // 말이 999칸이지 무한으로 만들어달라는뜻이라 다 찰일 없게 해야합니다.
        //         
        //         InvenManager.instance.AttackItem.UnEquipAll();
        //         for (int i = 0; i < preset.atkItems.Length; i++)
        //         {
        //             if(preset.atkItems[i] == null) continue;
        //             int invenInd = InvenManager.instance.AttackItem.GetIndex(preset.atkItems[i], InvenType.Inven);
        //             if (invenInd != -1)
        //             {
        //                 InvenManager.instance.AttackItem.EquipWithoutSavePreset(invenInd);
        //             }
        //             else
        //             {
        //                 Debug.Log($"{preset.atkItems[i]}를 가지고 있지 않음");
        //             }
        //         }
        //     }
        // }
        //
        // public static void SavePreset(int index)
        // {
        //     if (AtkItemData.Presets.TryGetValue(index, out var preset))
        //     {
        //         for (int i = 0; i < preset.atkItems.Length; i++)
        //         {
        //             preset.atkItems[i] = null;
        //         }
        //
        //         for (int i = 0; i < AtkInven.AtkItemInven.Count; i++)
        //         {
        //             preset.atkItems[i] = AtkInven.AtkItemInven[i]?.Name;
        //         }
        //     }
        // }

        // 무장 사용
        public static void Attack(int index)
        {
            if (!AtkInven.CanUseAttackItem(index))
            {
                Debug.LogError("해당 인덱스에 무장을 장착하지 않음");
                return;
            }

            var item = AtkInven.AtkItemInven[index];

            if (item is IAttackItem atkItem && atkItem.TryAttack())
            {
                CurrentItem = atkItem;

                atkItem.BeforeAttack();
                atkItem.UseAttack();
            }
        }

        public static IAttackItem GetItem(int index)
        {
            if (AtkInven.AtkItemInven.Count <= index) return null;

            var item = AtkInven.AtkItemInven[index];

            return item as IAttackItem;
        }
    }
}