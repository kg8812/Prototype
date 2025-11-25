using System.Collections.Generic;
using Apis.Managers;

namespace Apis
{
    public class DropItemFactoryManager : SingletonPersistent<DropItemFactoryManager>
    {
        public List<DropItem> GetDropItemById(int dropItemType, int dropItemIndex, int amount)
        {
            // 고쳐야함
            List<DropItem> dropItem = new();
            
            switch ((DropItemType)(dropItemType))
            {
                // accessory
                case DropItemType.Accessory:
                    foreach (var items in GameManager.Item.AccPickUp.CreateNews(dropItemIndex, amount))
                    {
                        dropItem.Add(items);
                    }
                    break;
                case DropItemType.ActiveSkill:
                    foreach (var item in GameManager.Item.ActiveSkillPickUp.CreateNews(dropItemIndex, amount))
                    {
                        dropItem.Add(item);
                    }
                    break;
                case DropItemType.Weapon:
                    foreach (var items in GameManager.Item.WeaponPickUp.CreateNews(dropItemIndex, amount))
                    {
                        dropItem.Add(items);
                    }
                    break;
            }
            
            foreach (var item in dropItem)
            {
                item.Dropping();
            }
            return dropItem;
        }


    
        private DropItemType GetDropItemTypeById(int dropId)
        {
            return (DropItemType)(dropId % 1000 - 1);
        }
    }
}
