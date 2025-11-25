using Apis;
using Apis.Managers;
using NewNewInvenSpace;
using PlayerState;
using Save.Schema;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class ActiveSkill_PickUp : Apis.DropItem
    {

        ActiveSkillItem item; // 획득할 아이템
        SpriteRenderer render;

        private UnityEvent<ActiveSkill_PickUp> _onCollect;
        public UnityEvent<ActiveSkill_PickUp> OnCollect => _onCollect ??= new();
        // public string activeSkillName;
        public int activeSkillItemId;
        private void Awake()
        {
            isInteractable = true;
            render = GetComponentInChildren<SpriteRenderer>();
        }
        public override void InvokeInteraction()
        {
            if (item == null)
            {
                item = GameManager.Item.GetActiveSkill(activeSkillItemId);
            }
            int addInd = InvenManager.instance.AttackItem.Invens[InvenType.Storage].GetEmptySlot();
            if (addInd < 0)
            {
                return;
            }
            InvenManager.instance.AttackItem.Add(addInd, item, InvenType.Storage);
            
            // TODO: 마법도 codex 해금 요소 만들어야 함.
            // DataAccess.Codex.UnLock(CodexData.CodexType.Item,item.Index);
            OnCollect.Invoke(this);
            GameManager.Item.ActiveSkillPickUp.Return(this);
        }       

        public void CreateNew(int skillItem)
        {
            item = GameManager.Item.ActiveSkillItem.CreateNew(skillItem);
            item.SetParent(transform);
            
            render.sprite = item.Image;
        }

        public void CreateExisting(ActiveSkillItem item)
        {
            this.item = item;
            this.item.SetParent(transform);
            render.sprite = item.Image;
        }
        protected override void ReturnObject(SceneData _)
        {
            GameManager.Item.ActiveSkillPickUp.Return(this);
        }

        protected override void OnDisable()
        { 
            base.OnDisable();
            OnCollect.RemoveAllListeners();
        }
    }
}