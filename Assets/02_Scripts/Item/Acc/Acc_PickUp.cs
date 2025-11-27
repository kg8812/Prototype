using Apis.Managers;
using NewNewInvenSpace;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class Acc_PickUp : DropItem
    {
        // public string accName;
        public int accId;

        private UnityEvent<Acc_PickUp> _onCollect;
        // 악세사리 픽업 (획득용) 스크립트

        private Accessory item; // 획득할 아이템
        private SpriteRenderer render;
        public UnityEvent<Acc_PickUp> OnCollect => _onCollect ??= new UnityEvent<Acc_PickUp>();

        private void Awake()
        {
            isInteractable = true;
            render = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnCollect.RemoveAllListeners();
        }

        public override void InvokeInteraction()
        {
            if (item == null) item = GameManager.Item.GetAcc(accId);
            if (InvenManager.instance.Acc.IsFull(InvenType.Storage)) return;

            InvenManager.instance.Acc.Add(item, InvenType.Storage);
            OnCollect.Invoke(this);
            GameManager.Item.AccPickUp.Return(this);
        }

        public void CreateNew(Accessory item)
        {
            this.item = GameManager.Item.Acc.CreateNew(item.ItemId);
            this.item.SetParent(transform);
            render.sprite = this.item.Image;
        }

        public void CreateExisting(Accessory item)
        {
            this.item = item;
            this.item.SetParent(transform);
            render.sprite = item.Image;
        }

        protected override void ReturnObject(SceneData _)
        {
            GameManager.Item.AccPickUp.Return(this);
        }
    }
}