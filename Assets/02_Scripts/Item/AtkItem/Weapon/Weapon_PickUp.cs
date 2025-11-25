using Apis;
using Apis.Managers;
using NewNewInvenSpace;
using Save.Schema;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class Weapon_PickUp : DropItem
    {
        // 무기 픽업 클래스

        // 획득할 무기
        Weapon weapon;
        public Weapon Weapon => weapon;
        private SpriteRenderer render;
        
        [HideInInspector] public UnityEvent<Weapon_PickUp> OnCollect = new();

        // public string weaponName;
        public int weaponId;
        private void Awake()
        {
            isInteractable = true;
            render = GetComponent<SpriteRenderer>();
        }

        public override void InvokeInteraction()
        {
            int addInd = InvenManager.instance.AttackItem.Invens[InvenType.Storage].GetEmptySlot();
            if (addInd < 0)
            {
                return;
            }

            if (weapon == null)
            {
                weapon = GameManager.Item.GetWeapon(weaponId);
            }
            InvenManager.instance.AttackItem.Add(addInd, weapon, InvenType.Storage);
            OnCollect.Invoke(this);
            GameManager.Item.WeaponPickUp.Return(this);
        }

        // 획득할 무기 넣기
        public void CreateNew(Weapon weapon)
        {
            if (weapon == null) return;
            
            this.weapon = GameManager.Item.Weapon.CreateNew(weapon.ItemId);
            if (this.weapon == null) return;
                
            this.weapon.SetParent(transform);
            render.sprite = this.weapon.Image;
        }
       
        public void CreateAlready(Weapon weapon)
        {
            this.weapon = weapon;
            if (weapon != null)
            {
                weapon.SetParent(transform);
            }
        }
        

        protected override void ReturnObject(SceneData _)
        {
            weapon = null;
            GameManager.Item.WeaponPickUp.Return(this);
        }

        protected override void OnDisable()
        { 
            base.OnDisable();
            OnCollect.RemoveAllListeners();
        }
    }
}
