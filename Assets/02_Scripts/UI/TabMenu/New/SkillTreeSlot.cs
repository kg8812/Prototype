using System.Collections;
using System.Collections.Generic;
using Apis.SkillTrees;
using Apis.UI;
using Save.Schema;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillTreeSlot : UIEffector
{
    public enum SlotType
    {
        Low,High,Inven
    }
    
    public static bool IsDragging;
    public static UI_DragItem DragImg;
    public static SkillTreeSlot ToChangeSlot;
    private SkillTree _skillTree;
    public SlotType slotType;
    private TextMeshProUGUI _text;
    public int index;
    
    public TextMeshProUGUI Text => _text ??= GetComponentInChildren<TextMeshProUGUI>();
    Image _image;
    public Image Image => _image ??= GetComponent<Image>();
    
    private static bool TryDrag(SkillTree item)
    {
        if (IsDragging) return false;
        IsDragging = true;
        DragImg.TryActivated();
        return true;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (TryDrag(_skillTree))
        {
            IsDragging = true;
            ToChangeSlot = this;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        IsDragging = false;
        DragImg.TryDeactivated();
        if (ToChangeSlot != null)
        {
            if (ToChangeSlot._skillTree != null)
            {
                Debug.Log("이미 장착중임");
                return;
            }

            if (_skillTree == null || !_skillTree.CheckEquipable(ToChangeSlot))
            {
                Debug.Log("슬롯이 맞지않음");
                return;
            }
            ToChangeSlot.OnSlotChanged(_skillTree);
            OnSlotChanged(null);
        }
        else
        {
            Debug.Log("No Slot");
        }
    }

    public void OnSlotChanged(SkillTree skillTree)
    {
        _skillTree = skillTree;
        if (skillTree == null)
        {
            if (slotType == SlotType.Inven)
            {
                Image.color = Color.white;
            }
            Text.text = "없음";
            return;
        }
        
        Text.text = skillTree.Name;
        var trees = GameManager.Save.currentSlotData.TempSaveData.SkillTreeData.equippedSkillTrees;
        switch (slotType)
        {
            case SlotType.Low:
                SkillTreeDatas.ApplySkillTree(skillTree.Index,1);
                trees.TryAdd(skillTree.Index, new SkillTreeSaveData.SkillTreeSlotData());
                trees[skillTree.Index] = new SkillTreeSaveData.SkillTreeSlotData()
                {
                    slotIndex = index,
                    slotType = SlotType.Low,
                };
                break;
            case SlotType.High:
                SkillTreeDatas.ApplySkillTree(skillTree.Index, 2);
                trees.TryAdd(skillTree.Index, new SkillTreeSaveData.SkillTreeSlotData());
                trees[skillTree.Index] = new SkillTreeSaveData.SkillTreeSlotData()
                {
                    slotIndex = index,
                    slotType = SlotType.High,
                };
                break;
            case SlotType.Inven:
                SkillTreeDatas.DeApplySkillTree(skillTree.Index);
                trees.Remove(skillTree.Index);
                break;
        }
        SetColor(skillTree);
    }

    void SetColor(SkillTree skillTree)
    {
        if (slotType != SlotType.Inven) return;
        switch (skillTree.SlotType)
        {
            case SkillTree.SlotTypeEnum.Low:
                Image.color = Color.blue;
                break;
            case SkillTree.SlotTypeEnum.High:
                Image.color = Color.red;
                break;
            case SkillTree.SlotTypeEnum.Medium:
                Image.color = Color.yellow;
                break;
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (IsDragging)
        {
            ToChangeSlot = this;
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (IsDragging && ReferenceEquals(this, ToChangeSlot))
        {
            ToChangeSlot = null;
        }
    }
}
