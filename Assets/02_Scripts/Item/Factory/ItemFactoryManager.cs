using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.Managers;
using NewNewInvenSpace;
using Save.Schema;
using Sirenix.Utilities;
using UnityEngine;

namespace Apis
{
    public class ItemFactoryManager
    {
        // 팩토리 매니저       

        public Factory_AccPickUp AccPickUp { get; private set; } // 악세 픽업 팩토리
        public Factory_WeaponPickUp WeaponPickUp { get; private set; } // 무기 픽업 팩토리
        
        public Factory_ActiveSkillPickUp ActiveSkillPickUp { get; private set; }
        public Factory_Acc Acc { get; private set; } // 악세 팩토리
        public Factory_Weapon Weapon { get; private set; } // 무기 팩토리
        public Factory_ActiveSkillItem ActiveSkillItem { get; private set; }
        public Factory_Gold Gold { get; private set; } // 골드 팩토리
        public Factory_Etc Etc { get; private set; } // 기타 팩토리
        public Factory_Germ Germ { get; private set; }

        // public ItemStorage Storage; // 아이템 보관소 (스킬로 인한 무기 교체 등 인벤토리 외 위치에 보관이 필요할 떄 사용)
        // inven용 item저장이라 invenmanager.instance.Storage로 이전.

        private bool isInit = false;
        public ItemFactoryManager()
        {
            LoadItems();
        }

        public void LoadItems()
        {
            if (isInit) return;
            isInit = true;
            // 팩토리 초기화
            var accs = Default.ResourceUtil.LoadAll<Accessory>("Prefabs/Items/Accessory");
            var weapons = Default.ResourceUtil.LoadAll<Weapon>("Prefabs/Items/Weapon");
            var activeSkills = Default.ResourceUtil.LoadAll<ActiveSkill>("Prefabs/Items/ActiveSkill");
            var activeSkillItem = new[]
            {
                Default.ResourceUtil.Load<ActiveSkillItem>("ActiveSkillItem"),
            };
            
            var golds = Default.ResourceUtil.LoadAll<Gold_Drop>("Prefabs/Items/Gold");
            var germs = Default.ResourceUtil.LoadAll<Germ>("Prefabs/Items/Germ");
            var etcs = Default.ResourceUtil.LoadAll<EtcItem>("EtcItems");
            AccPickUp = new Factory_AccPickUp(new[]
                { Default.ResourceUtil.Load<Acc_PickUp>("Prefabs/Items/Accessory/AccPickUp") });
            WeaponPickUp = new Factory_WeaponPickUp(new[]
                { Default.ResourceUtil.Load<Weapon_PickUp>("Prefabs/Items/Weapon/WeaponPickUp") });
            ActiveSkillPickUp = new Factory_ActiveSkillPickUp(new[]
                { Default.ResourceUtil.Load<ActiveSkill_PickUp>("Prefabs/Items/ActiveSkill/ActiveSkillPickUp") });
            foreach (var x in accs)
            {
                x.Init();
                if (x.Data.unlock)
                {
                    DataAccess.Codex.UnLock(CodexData.CodexType.Item, x.ItemId);
                }
            }

            foreach (var x in weapons)
            {
                x.Init();
                if (x.Data.unlock)
                {
                    DataAccess.Codex.UnLock(CodexData.CodexType.Item, x.ItemId);
                }
            }
            Acc = new Factory_Acc(accs);
            Weapon = new Factory_Weapon(weapons);
            // TODO: skill list
            ActiveSkillItem = new Factory_ActiveSkillItem(activeSkills, activeSkillItem);
            Gold = new Factory_Gold(golds);
            Germ = new Factory_Germ(germs);
            Etc = new Factory_Etc(etcs);
        }
        public List<Weapon> WeaponList => Weapon.WpDict.Values.ToList();
        public List<Accessory> Accessories => Acc.AccDict.Values.ToList();
        public Accessory GetAcc(int itemId)
        {
            return Acc.CreateNew(itemId);
        }

        public Accessory RandAcc => Acc.CreateRandom();

        public Weapon GetWeapon(int itemId)
        {
            return Weapon.CreateNew(itemId);
        }

        public Weapon RandWeapon => Weapon.CreateRandom();
        
        public ActiveSkillItem GetActiveSkill(int skillId)
        {
            return ActiveSkillItem.CreateNew(skillId);
        }
        
        public ActiveSkillItem RandActiveSkill => ActiveSkillItem.CreateRandom();

        public EtcItem GetEtcItem(int itemId)
        {
            return Etc.CreateNew(itemId);
        }
        //인벤에 악세서리 추가
        public void AddAcc(int index)
        {
            if (!AccessoryData.DataLoad.TryGetData(index, out var data)) return;
            // string n = LanguageManager.Str(data.accName);
            if (InvenManager.instance.Acc.IsFull(InvenType.Storage))
            {
                var pickUp = GameManager.Item.AccPickUp.CreateNew(data.accId);
                pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
            }
            else
            {
                var acc = GameManager.Item.GetAcc(data.accId);
                InvenManager.instance.Acc.Add(acc, InvenType.Storage);
            }
        }

        // 인벤에 무기 추가
        public void AddWeapon(int index)
        {
            if (!WeaponData.DataLoad.TryGetWeaponData(index, out var data)) return;
            // string n = LanguageManager.Str(data.weaponNameString);
            if (InvenManager.instance.AttackItem.IsFull(InvenType.Storage))
            {
                var pickUp = GameManager.Item.WeaponPickUp.CreateNew(data.weaponId);
                pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
            }
            else
            {
                var wp = GameManager.Item.GetWeapon(data.weaponId);
                InvenManager.instance.AttackItem.Add(wp, InvenType.Storage);
            }
        }
    }
}