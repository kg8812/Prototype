using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Apis.UI.Focus;
using NewNewInvenSpace;
using UnityEngine;

namespace Apis
{
    public class UITab_GuitarInventory : UI_InventoryContent
    {
        private GuitarInventoryGroup GuitarInven => InvenManager.instance.GuitarInven;
        [Serializable]
        public class GuitarInventoryGroupDataset : UnitySerializedDictionary<GuitarInvenType, FocusParent>
        {
        }

        [SerializeField] private GuitarInventoryGroupDataset dataset;

        private Dictionary<GuitarInvenType, int> slotCnt;
        private Dictionary<GuitarInvenType, ItemSlot[]> itemSlots;

        public override void Init()
        {
            base.Init();
            slotCnt = new();
            itemSlots = new();
            foreach (GuitarInvenType type in Enum.GetValues(typeof(GuitarInvenType)))
            {
                if (!dataset.ContainsKey(type)) Debug.LogError($"{type} slot 세팅 안됨.");

                FocusParent focus = dataset[type];
                itemSlots.Add(type, focus.focusList.Cast<ItemSlot>().ToArray());
                focus.InitCheck();
                focus.FocusGroup = this;

                int cnt = CalcLastIndex(GuitarInven.Invens[type]) + 1;
                slotCnt.Add(type, cnt);
                SlotInit(itemSlots[type], type, cnt);
                    

                GuitarInven.Invens[type].OnSlotChanged += (_, _) =>
                {
                    int lastCnt = CalcLastIndex(GuitarInven.Invens[type])+1;
                    if (slotCnt[type] != lastCnt)
                    {
                        SlotCntChanged(lastCnt, type);
                    }
                };
            }

            ChangeOn();
            
            dataset[0].MoveTo(0);
            _curFocus = dataset[0];
        }

        private int CalcLastIndex(InventoryList invenList)
        {
            int lastInd = invenList.Count-1;
            for (int i = lastInd; i >= 0; i--)
            {
                if(invenList[i] != null)break;
            }
            return lastInd;
        }

        private void SlotInit(ItemSlot[] slotList, GuitarInvenType type, int slotCnt, int startInd = 0)
        {
            int cnt = slotCnt;
            for (int i = 0; i < slotList.Length; i++)
            {
                ItemSlot slot = slotList[i];
                int myI = startInd + i;
                if (myI < cnt && myI < GuitarInven.Invens[type].Count)
                {
                    slot.InitCheck();
                    slot.invenType = InvenType.Storage;
                    slot.InventoryList = GuitarInven.Invens[type];
                    slot.index = myI;
                    slot.OnValueChanged.AddListener(isOn =>
                    {
                        if (isOn)
                        {
                            CurFocusedSlot = slot;
                            // TODO: description setting
                        }else if (CurFocusedSlot == slot)
                        {
                            CurFocusedSlot = null;
                        }
                    });
                    GuitarInven.Invens[type].OnSlotChanged += slot.OnSlotChanged;
                    
                    slot.UpdateItem();
                }
                else
                {
                    ReturnSlot(slot, type);
                }
            }
        }

        private void SlotCntChanged(int cnt, GuitarInvenType type)
        {
            int prevCnt = slotCnt[type];
            FocusParent parent = dataset[type];

            slotCnt[type] = cnt;
            
            if (cnt > prevCnt)
            {
                // add new slots
                ItemSlot[] newSlots = new ItemSlot[cnt-prevCnt];
                for (int i = 0; i < cnt-prevCnt; i++)
                {
                    newSlots[i] = GameManager.UI.MakeSubItem("ItemSlot", parent.transform) as ItemSlot;
                    // new slot position setting
                    parent.RegisterElement(newSlots[i]);
                }

                ItemSlot[] slots = itemSlots[type];
                Array.Resize(ref slots, cnt);
                for (int i = 0; i < cnt-prevCnt; i++)
                {
                    slots[i + prevCnt] = newSlots[i];
                }
                SlotInit(newSlots, type, cnt, prevCnt);
            }
            else
            {
                // delete origin slots
                ItemSlot[] slots = itemSlots[type];
                for (int i = cnt; i < prevCnt; i++)
                {
                    if (slots.Length <= i)
                    {
                        Debug.LogError($"i 벗어남 {slots.Length} {i}");
                    }
                    ReturnSlot(slots[i], type);
                    parent.RemoveElement(slots[i]);
                    slots[i] = null;
                }
                Array.Resize(ref slots, cnt);
            }
        }
        
        private void ReturnSlot(ItemSlot itemSlot, GuitarInvenType type)
        {
            itemSlot.OnValueChanged?.RemoveAllListeners();
            GuitarInven.Invens[type].OnSlotChanged -= itemSlot.OnSlotChanged;
            // Debug.Log($"return slot {gameObject.name} {type} {itemSlot.index}");
            itemSlot.CloseOwn();
        }

        private void ChangeOn()
        {
            var datasetList = dataset.ToList();
            for (int i = 0; i < datasetList.Count; i++)
            {
                KeyValuePair<GuitarInvenType, FocusParent> data = datasetList[i];

                int prevInd = i == 0 ? datasetList.Count - 1 : i - 1;
                int nextInd = i == datasetList.Count - 1 ? 0 : i + 1;

                FocusParent prevFocus = datasetList[prevInd].Value;
                FocusParent curFocus = data.Value;
                FocusParent nextFocus = datasetList[nextInd].Value;

                data.Value.tableData.moveUp = x =>
                {
                    int realX = Default.FormatUtils.GetRatioIntByInt(x, curFocus.tableData.x, prevFocus.tableData.x);
            
                    // 해당하는 가장 밑의 개체로 이동.
                    prevFocus.MoveTo(realX + ((prevFocus.focusList.Count - realX - 1) / prevFocus.tableData.x) * prevFocus.tableData.x );
                    _curFocus = prevFocus;
                };
                
                data.Value.tableData.moveDown = x =>
                {
                    int realX = Default.FormatUtils.GetRatioIntByInt(x, curFocus.tableData.x, nextFocus.tableData.x);
            
                    // 해당하는 가장 밑의 개체로 이동.
                    nextFocus.MoveTo(realX);
                    _curFocus = nextFocus;
                };
            }
        }
        
        public override void ChangeFocusParent(FocusParent fp)
        {
            base.ChangeFocusParent(fp);
            // 해당 fp가 포커스 해제가 가능하다면
            // 이거 안해주면 focus reset 호출 -> move to 호출 -> 이 함수 호출로 스택오버플로우 생김
            // 애초에 기획적으로 존재하지 않지만 혹시 모르기에
            if (fp.canNoneFocus)
            {
                // 다른곳에서 선택되면 나머지 focusParent에서 포커스 해제
                foreach (var data in dataset)
                {
                    if(!ReferenceEquals(fp, data.Value))
                        data.Value.FocusReset();
                }
            }
        }
    }
}