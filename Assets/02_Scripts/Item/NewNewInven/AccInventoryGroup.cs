using Apis;
using Apis;

namespace NewNewInvenSpace
{
    public class AccInventoryGroup: InventoryGroup
    {
        
        private BonusStat _bonusStat;
        
        // 가져갈때마다 Reset 하는것 같지만 정작 호출은 Player에서만 해서 최적화 불필요
        public BonusStat BonusStat
        {
            get
            {
                _bonusStat ??= new BonusStat();
                _bonusStat.Reset();
                for (int i = 0; i < Invens[InvenType.Equipment].Count; i++)
                {
                    Item item = Invens[InvenType.Equipment][i];
                    if (item != null && item is Accessory acc)
                    {
                        _bonusStat += acc.BonusStat;
                    } 
                }
                return _bonusStat;
            }
        }
        
        
        public AccInventoryGroup(int eqMaxCnt, int eqCnt, int stMaxCnt, int stCnt) : base(eqMaxCnt, eqCnt, stMaxCnt, stCnt)
        {
            Invens[InvenType.Equipment].ItemAddedTo += Equipped;
            Invens[InvenType.Equipment].ItemRemovedFrom += UnEquipped;
            
        }
        
        

        public override Item Abandon(int index, InvenType type)
        {
            Item item = base.Abandon(index, type);
            
            if (item != null  && item is Accessory accessory)
            {
                Acc_PickUp pu = GameManager.Item.AccPickUp.CreateExisting(InvenManager.instance.Storage.Get(accessory));
                pu.transform.position = GameManager.instance.ControllingEntity.transform.position;
            }

            return item;
        }

        private void Equipped(int index, Item item)
        {
            if (item != null && item is Accessory acc)
            {
                acc.Equip(GameManager.instance.Player);
            }
        }
        
        private void UnEquipped(int index, Item item)
        {
            if (item != null && item is Accessory acc)
            {
                acc.UnEquip();
            }
        }
    }
}