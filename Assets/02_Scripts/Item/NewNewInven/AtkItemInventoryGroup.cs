namespace NewNewInvenSpace
{
    public class AtkItemInventoryGroup : InventoryGroup
    {
        public AtkItemInventoryGroup(int eqMaxCnt, int eqCnt, int stMaxCnt, int stCnt) : base(eqMaxCnt, eqCnt, stMaxCnt,
            stCnt)
        {
            Invens.Add(InvenType.Hidden, new InventoryList(eqMaxCnt, eqMaxCnt));

            Invens[InvenType.Equipment].ItemAddedTo += EquipmentEquipped;
            Invens[InvenType.Equipment].ItemRemovedFrom += EquipmentUnEquipped;
            Invens[InvenType.Equipment].OnSlotChanged += ItemSlotChanged;

            Invens[InvenType.Hidden].ItemAddedTo += HiddenEquipped;
            Invens[InvenType.Hidden].ItemRemovedFrom += HiddenUnEquipped;
        }

        public InventoryList AtkItemInven => PresetType == PresetType.InvenPreset
            ? Invens[InvenType.Equipment]
            : Invens[InvenType.Hidden];

        public override PresetType PresetType
        {
            get => _presetType;
            set
            {
                if (_presetType == value) return;
                // inven to override
                if (value == PresetType.OverridePreset)
                {
                    Invens[InvenType.Equipment].OnSlotChanged -= ItemSlotChanged;
                    Invens[InvenType.Hidden].OnSlotChanged -= ItemSlotChanged;
                    Invens[InvenType.Hidden].OnSlotChanged += ItemSlotChanged;
                    for (var i = 0; i < Invens[InvenType.Equipment].Count; i++)
                    {
                        var item = Invens[InvenType.Equipment][i];
                        if (item != null)
                            UnEquipped(i, item);
                    }
                }

                // override to inven
                if (value == PresetType.InvenPreset)
                {
                    Invens[InvenType.Equipment].OnSlotChanged -= ItemSlotChanged;
                    Invens[InvenType.Equipment].OnSlotChanged += ItemSlotChanged;
                    Invens[InvenType.Hidden].OnSlotChanged -= ItemSlotChanged;
                    for (var i = 0; i < Invens[InvenType.Equipment].Count; i++)
                    {
                        // 기존 inven 프리셋으로 돌아가는 경우 => 자체적인 해제, 장착이 없으니, equip을 호출해줘야 함.
                        // 다른 inven 프리셋으로 돌아가는 경우 => PresetType 변경 전에 미리 UnEquipAll 처리해줘서 전부 null되서 상관 x
                        var item = Invens[InvenType.Equipment][i];
                        ItemSlotChanged(i, item);
                        if (item != null) Equipped(i, item);
                    }
                }
                // Override -> Inven은 Equipment에 있는 item들이 이미 unEquipped 상태라 자체 unEquip 필요 x

                _presetType = value;
            }
        }

        public bool CanUseAttackItem(int index)
        {
            return AtkItemInven.Count > index && AtkItemInven[index] != null;
        }

        private bool CheckEquipCondition(IAttackItem atkItem)
        {
            // TODO: CheckEquipCondition 나중에 수정.
            return true;
            // return !Array.Exists(Invens[InvenType.Equipment].Slots,
            //  x => x is IAttackItem item && (item == atkItem || item.Category == atkItem.Category));
        }

        /// <summary>
        ///     플레이어 스킬인 경우에는 작동 x 그냥 equip inven만 바뀌고 실직적 착용과 item equipped호출 x
        /// </summary>
        public Item ChangeAttackItem(Item item, int index = 0)
        {
            var preItem = Invens[InvenType.Equipment].Remove(index);
            Invens[InvenType.Equipment].AddItem(index, item);
            return preItem;
        }


        private void EquipmentEquipped(int index, Item item)
        {
            if (PresetType == PresetType.InvenPreset)
                Equipped(index, item);
        }

        private void EquipmentUnEquipped(int index, Item item)
        {
            if (PresetType == PresetType.InvenPreset)
                UnEquipped(index, item);
        }

        private void HiddenEquipped(int index, Item item)
        {
            if (PresetType == PresetType.OverridePreset)
                Equipped(index, item);
        }

        private void HiddenUnEquipped(int index, Item item)
        {
            if (PresetType == PresetType.OverridePreset) UnEquipped(index, item);
        }

        private void ItemSlotChanged(int index, Item item)
        {
            if (item != null && item is IAttackItem attackItem) attackItem.AtkSlotIndex = index;
            // attackItem.SetIcon(UI_MainHud.Instance.atkItemIcons[index]);
            // UI_MainHud.Instance.atkItemIcons[index].SetItem(attackItem);
            //UI_MainHud.Instance.atkItemIcons[index].SetItem(null);
        }

        private void Equipped(int index, Item item)
        {
            if (item != null && item is IAttackItem attackItem)
                //아이콘 설정을 먼저해야함. 순서 변경해서 아이콘 설정보다 장착이 위로오면 아이콘 작동안함.
                attackItem.Equip(GameManager.instance.Player);
        }

        private void UnEquipped(int index, Item item)
        {
            if (item != null && item is IAttackItem attackItem)
            {
                // GameManager.instance.Player.UnEquip(weapon);
                //UI_AtkItemIcon icon = UI_MainHud.Instance.atkItemIcons[attackItem.AtkSlotIndex];
                //icon.SetItem(null);
                attackItem.UnEquip();
                attackItem.SetIcon(null);
            }
        }


        public override bool Add(int index, Item item, InvenType type)
        {
            if (type == InvenType.Equipment && item is IAttackItem atkItem && !CheckEquipCondition(atkItem))
                return false;

            return base.Add(index, item, type);
        }

        protected override bool ChangeCheck(int index1, InvenType type1, int index2, InvenType type2)
        {
            if (type1 == InvenType.Storage && type2 == InvenType.Equipment)
            {
                var item = Invens[type1][index1];
                if (item != null && item is IAttackItem atkItem && !CheckEquipCondition(atkItem))
                    return false;
            }

            return true;
        }

        public void IncreaseSlotCount(int amount)
        {
            Invens[InvenType.Equipment].Count += amount;
        }
    }
}