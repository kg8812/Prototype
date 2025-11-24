using System;
using System.Collections.Generic;
using System.Linq;
using Apis.SkillTrees;
using Default;
using Save.Schema;
using UI;
using UnityEngine;

namespace Apis
{
    public class UI_SkillTree : UI_Scene
    {
        public enum GameObjects
        {
            SkillTreeContents,
        }
        private const string buttonAddress = "SkillTreeSlot";
        private Transform listParent;
        public SkillTreeSlot[] lowerSlots;
        public SkillTreeSlot[] higherSlots;
        private List<SkillTreeSlot> inven = new();
        
        public UI_DragItem dragItem;

        public override void Init()
        {
            base.Init();
            Bind<GameObject>(typeof(GameObjects));
            listParent = Get<GameObject>((int)GameObjects.SkillTreeContents).transform;
            SkillTreeSlot.DragImg = dragItem;
            
            dragItem.Init();
            inven ??= new();
            CreateSkillTreeInvenSlots();
            ResetEquipSlots();
            SetSlots();
        }

        public override void TryActivated(bool force = false)
        {
            base.TryActivated(force);
            SkillTreeSlot.DragImg = dragItem;

            ResetEquipSlots();
            SetSlots();
        }

        protected override void Activated()
        {
            base.Activated();
        }

        void ResetEquipSlots()
        {
            foreach (var x in lowerSlots)
            {
                x.OnSlotChanged(null);
            }

            foreach (var x in higherSlots)
            {
                x.OnSlotChanged(null);
            }
        }
        void CreateSkillTreeInvenSlots()
        {
            for (int i = 0; i < 20; i++)
            {
                SkillTreeSlot slot = GameManager.UI.MakeSubItem(buttonAddress, listParent).GetComponent<SkillTreeSlot>();
                slot.slotType = SkillTreeSlot.SlotType.Inven;
                slot.OnSlotChanged(null);
                inven.Add(slot);
            }
        }
        public void SetSlots()
        {
            var equippedSkillTrees = GameManager.Save.currentSlotData.TempSaveData.SkillTreeData.equippedSkillTrees;
            SkillTrees.SkillTree tree;

            var keys = equippedSkillTrees.Keys.ToList();
            
            foreach (var key in keys)
            {
                var slotData = equippedSkillTrees[key];
                tree = SkillTreeDatas.GetSkillTree(key);
                
                if (slotData.slotType == SkillTreeSlot.SlotType.High)
                {
                    foreach (var slot in higherSlots)
                    {
                        if (slot.index == slotData.slotIndex &&
                            tree.PlayerType == GameManager.instance.Player.playerType)
                        {
                            slot.OnSlotChanged(tree);
                            break;
                        }
                    }
                }
                else if (slotData.slotType == SkillTreeSlot.SlotType.Low)
                {
                    foreach (var slot in lowerSlots)
                    {
                        if (slot.index == slotData.slotIndex &&
                            tree.PlayerType == GameManager.instance.Player.playerType)
                        {
                            slot.OnSlotChanged(tree);
                            break;
                        }
                    }
                }
            }
            
            var skillTrees = SkillTreeDatas.GetAvailableSkillTrees();

            for (int i = 0; i < inven.Count; i++)
            {
                inven[i].OnSlotChanged(i < skillTrees.Count ? skillTrees[i] : null);
            }
        }
    }
}