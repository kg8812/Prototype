using System;
using Apis;
using Apis.UI.Focus;
using Apis.InvenSpace;
using Sirenix.OdinInspector;
using UnityEngine;

public class UITab_ItemViewer : UI_InventoryContent
{
    public FocusParent ItemDisplayParent; // 아이템을 보여줄 FocusParent
    [SerializeField] private ItemSlot[] itemSlots; // 미리 할당된 또는 동적으로 채워질 아이템 슬롯들
    
    [Tooltip("어떤 아이템 인벤토리를 보여줄지 선택합니다.")]
    public Define.ItemType itemType;
    
    [Tooltip("어떤 타입의 인벤토리를 보여줄지 선택합니다.")]
    public InvenType inventoryTypeToShow = InvenType.Equipment; // 기본값은 장비창

    private int _currentSlotCount;

    [LabelText("시작 인덱스")] [SerializeField] private int _startIndex;

    [LabelText("인벤에 맞춰서 슬롯 수 조절")][SerializeField] private bool adjustSlotCount = true;
    protected override InventoryGroup invenGroupManager
    {
        get
        {
            switch (itemType)
            {
                //TODO : 아이템 타입에 따라서 인벤토리 반환하기
            }

            return null;
        }
    }

    #region 초기화 관련

    public override void Init()
    {
        base.Init();
        if (ItemDisplayParent == null)
        {
            Debug.LogError("ItemDisplayParent (FocusParent) is not assigned!");
            return;
        }

        if (invenGroupManager == null)
        {
            Debug.LogError("InventoryGroup manager is not assigned!");
            return;
        }

        ItemDisplayParent.FocusGroup = this; // IFocusGroup의 ChangeFocusParent를 이 클래스가 받도록 설정
        ItemDisplayParent.InitCheck();

        _currentSlotCount = itemSlots?.Length ?? 0;

        // 지정된 타입의 슬롯 초기화
        SlotInit(itemSlots, inventoryTypeToShow, _currentSlotCount);

        // 지정된 타입의 인벤토리 개수 변경 감지
        if (invenGroupManager.Invens.TryGetValue(inventoryTypeToShow, out var targetInventory))
        {
            int currentItemCountInInventory = targetInventory.Count - _startIndex;
            if (currentItemCountInInventory != _currentSlotCount)
            {
                SlotCntChanged(currentItemCountInInventory, inventoryTypeToShow);
            }

            targetInventory.OnCountChanged += (cnt) => SlotCntChanged(cnt - _startIndex, inventoryTypeToShow);
        }
        else
        {
            Debug.LogError($"{inventoryTypeToShow} inventory not found in invenGroupManager!");
        }

        // 초기 포커스 설정
        if (itemSlots != null && itemSlots.Length > 0)
        {
            ItemDisplayParent.SetIndex(0); // 첫 번째 슬롯으로 포커스
        }

        ChangeFocusParent(ItemDisplayParent); // 현재 활성화된 포커스 그룹
        IsChanging = false; // 아이템 교환 상태는 사용하지 않지만, 베이스 클래스 호환을 위해 초기화
    }

    private void SlotCntChanged(int newCount, InvenType type)
    {
        if (type != inventoryTypeToShow) return; // 설정된 타입의 인벤토리만 처리
        if (!adjustSlotCount) return;
        
        FocusParent parent = ItemDisplayParent;
        int prevCnt = _currentSlotCount;
        _currentSlotCount = newCount;

        ItemSlot[] targetSlots = itemSlots;

        if (newCount > prevCnt)
        {
            ItemSlot[] newSlotsToAdd = new ItemSlot[newCount - prevCnt];
            for (int i = 0; i < newSlotsToAdd.Length; i++)
            {
                newSlotsToAdd[i] = GameManager.UI.MakeSubItem("ItemSlot", parent.transform) as ItemSlot;
                if (newSlotsToAdd[i] == null)
                {
                    Debug.LogError($"Failed to create ItemSlot for {inventoryTypeToShow} slot {i + prevCnt}");
                    continue;
                }

                parent.RegisterElement(newSlotsToAdd[i]);
            }

            Array.Resize(ref targetSlots, newCount);
            for (int i = 0; i < newSlotsToAdd.Length; i++)
            {
                targetSlots[i + prevCnt] = newSlotsToAdd[i];
            }

            SlotInit(newSlotsToAdd, inventoryTypeToShow, newCount, prevCnt);
        }
        else if (newCount < prevCnt)
        {
            for (int i = newCount; i < prevCnt; i++)
            {
                if (targetSlots[i] != null)
                {
                    ReturnSlot(targetSlots[i], inventoryTypeToShow);
                    parent.RemoveElement(targetSlots[i]);
                    // Destroy(targetSlots[i].gameObject); // 필요시 오브젝트 파괴
                    targetSlots[i] = null;
                }
            }

            Array.Resize(ref targetSlots, newCount);
        }

        itemSlots = targetSlots;
    }

    protected virtual void SlotInit(ItemSlot[] slotList, InvenType type, int slotTotalCount, int startIndex = 0)
    {
        if (slotList == null) return;

        for (int i = 0; i < slotList.Length; i++)
        {
            ItemSlot slot = slotList[i];
            if (slot == null) continue;

            int currentIndex = startIndex + i;
            if (currentIndex < slotTotalCount)
            {
                slot.InitCheck();
                slot.invenType = type;
                slot.InventoryList = invenGroupManager.Invens[type];
                slot.index = currentIndex + _startIndex;

                slot.OnSelected.RemoveAllListeners();
                slot.OnDeselected.RemoveAllListeners();

                slot.OnSelected.AddListener(() => { curFocusedSlot = slot; });
                slot.OnDeselected.AddListener(() =>
                {
                    if (curFocusedSlot == slot)
                    {
                        curFocusedSlot = null;
                    }
                });

                // 읽기 전용이므로 드래그 관련 이벤트는 연결하지 않음
                slot.OnDragChanged = null;
                slot.OnPointerChanged = null;

                invenGroupManager.Invens[type].OnSlotChanged -= slot.OnSlotChanged;
                invenGroupManager.Invens[type].OnSlotChanged += slot.OnSlotChanged;

                slot.UpdateItem();
            }
            else
            {
                ReturnSlot(slot, type);
                slot.gameObject.SetActive(false);
            }
        }
    }

    private void ReturnSlot(ItemSlot itemSlot, InvenType type)
    {
        if (itemSlot == null) return;
        itemSlot.OnSelected?.RemoveAllListeners();
        itemSlot.OnDeselected?.RemoveAllListeners();
        if (invenGroupManager.Invens.ContainsKey(type))
        {
            invenGroupManager.Invens[type].OnSlotChanged -= itemSlot.OnSlotChanged;
        }
        // itemSlot.CloseOwn(); // ItemSlot 자체 리소스 정리 (풀링 반환 등) - 필요시 주석 해제
    }

    #endregion

    #region 탭 오픈/클로즈

    public override void OnOpen()
    {
        base.OnOpen();
        IsChanging = false; // 아이템 교환 상태는 사용 안 함
        if (itemSlots != null)
        {
            foreach (var slot in itemSlots)
            {
                slot?.UpdateItem();
            }
        }

        // 초기 포커스 다시 설정
        if (ItemDisplayParent != null && itemSlots != null && itemSlots.Length > 0)
        {
            int targetIndex = (curFocusedSlot != null && curFocusedSlot.invenType == inventoryTypeToShow)
                ? curFocusedSlot.index
                : 0;
            if (targetIndex >= _currentSlotCount) targetIndex = 0;
            ItemDisplayParent.SetIndex(targetIndex);
            ChangeFocusParent(ItemDisplayParent);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        // IsChanging 관련 CancelChange는 호출할 필요 없음 (상호작용 없으므로)
    }

    #endregion

    #region 컨트롤 (키보드, 게임패드) - 보기 전용이므로 최소화

    public override void KeyControl()
    {
        // base.KeyControl(); // IsChanging 관련 로직 필요 없음
        // 오직 FocusParent 내부의 포커스 이동만 처리
        _curFocus?.KeyControl();
    }

    public override void GamePadControl()
    {
        // base.GamePadControl(); // IsChanging 관련 로직 필요 없음
        _curFocus?.GamePadControl();
    }

    #endregion

    #region 아이템 교환/상호작용 관련 메소드 - 읽기 전용이므로 비활성화/단순화

    public override void TryChange(ItemSlot slot)
    {
        // 읽기 전용이므로 아이템을 집는 동작 없음
        // base.TryChange(slot); 
        IsChanging = false;
    }

    public override void SelectedForChange(ItemSlot slotToPlace)
    {
        // 읽기 전용이므로 아이템을 내려놓는 동작 없음
        // base.SelectedForChange(slotToPlace);
        IsChanging = false;
    }

    public override bool CancelChange()
    {
        // 읽기 전용이므로 교환 취소 로직도 단순화
        bool result = base.CancelChange(); // IsChanging = false 등 기본 처리
        IsChanging = false;
        return result;
    }

    #endregion
}