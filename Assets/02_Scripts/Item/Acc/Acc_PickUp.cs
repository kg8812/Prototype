using Apis;
using Apis.Managers;
using NewNewInvenSpace;
using Save.Schema;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class Acc_PickUp : Apis.DropItem
    {
        // 악세사리 픽업 (획득용) 스크립트

        Accessory item; // 획득할 아이템
        SpriteRenderer render;

        private UnityEvent<Acc_PickUp> _onCollect;
        public UnityEvent<Acc_PickUp> OnCollect => _onCollect ??= new();

        // public string accName;
        public int accId;
        
        private void Awake()
        {
            isInteractable = true;
            render = GetComponentInChildren<SpriteRenderer>();
        }
        public override void InvokeInteraction()
        {
            if (item == null)
            {
                item = GameManager.Item.GetAcc(accId);
            }
            if (InvenManager.instance.Acc.IsFull(InvenType.Storage))
            {
                return;
            }
            
            InvenManager.instance.Acc.Add(item, InvenType.Storage);
            DataAccess.Codex.UnLock(CodexData.CodexType.Item,item.ItemId);
            OnCollect.Invoke(this);
            GameManager.Item.AccPickUp.Return(this);
            GameManager.UI.CreateUI("UI_ItemPopup", Apis.UIType.Main).GetComponent<UI_ItemPopUp>().Init(item);
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

        protected override void OnDisable()
        { 
            base.OnDisable();
            OnCollect.RemoveAllListeners();
        }
    }
}
