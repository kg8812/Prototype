using System.Collections.Generic;
using System.Linq;
using Default;
using Apis.InvenSpace;

namespace Apis
{
    public class ItemFactoryManager
    {
        // public ItemStorage Storage; // 아이템 보관소 (스킬로 인한 무기 교체 등 인벤토리 외 위치에 보관이 필요할 떄 사용)
        // inven용 item저장이라 invenmanager.instance.Storage로 이전.

        private bool isInit;

        public ItemFactoryManager()
        {
            LoadItems();
        }
        // 팩토리 매니저       

        public void LoadItems()
        {
            if (isInit) return;
            isInit = true;
            // TODO : 팩토리 초기화
        }

        public Item GetItem(int itemId)
        {
            Item item = null;
            
            //TODO : 아이템 팩토리에서 생성해서 넣어줄것
            // item = ....
            
            return item;
        }
    }
}