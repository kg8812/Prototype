using Apis;
using Apis.InvenSpace;
using Apis.Managers;
using Apis.UI;
using UnityEngine;
using UnityEngine.Events;


namespace Apis
{
    public class ItemPickUp : DropItem
    {
        // 악세사리 픽업 (획득용) 스크립트

        Item item; // 획득할 아이템
        SpriteRenderer render;

        private UnityEvent<ItemPickUp> _onCollect;
        public UnityEvent<ItemPickUp> OnCollect => _onCollect ??= new();

        // public string accName;
        public int itemId;

        private void Awake()
        {
            isInteractable = true;
            render = GetComponentInChildren<SpriteRenderer>();
        }

        public override void InvokeInteraction()
        {
            if (item == null)
            {
                item = GameManager.Item.GetItem(itemId);
            }
            
            // TODO : 인벤토리에 아이템 추가 및 itemFactory에 pickup 리턴 필요
            //InvenManager.instance.Add(item, InvenType.Storage);
            OnCollect.Invoke(this);
            //GameManager.Item.PickUp.Return(this);
        }

        protected override void ReturnObject(SceneData _)
        {
            base.ReturnObject(_);
            // TODO : 팩토리에 오브젝트 반환
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnCollect.RemoveAllListeners();
        }
    }
}