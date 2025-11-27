using System.Collections.Generic;
using System.Linq;
using Default;
using NewNewInvenSpace;

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
            // 팩토리 초기화
        }
    }
}