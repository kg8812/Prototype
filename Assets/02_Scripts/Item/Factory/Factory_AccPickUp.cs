using System.Collections.Generic;

namespace Apis
{
    public class Factory_AccPickUp : ItemFactory<Acc_PickUp>
    {
        //아이템 픽업 팩토리

        private readonly Acc_PickUp pickUp;

        public Factory_AccPickUp(Acc_PickUp[] pickUps) : base(pickUps)
        {
            pickUp = pickUps[0];
        }

        public override Acc_PickUp CreateNew(int itemId)
        {
            var pu = pool.Get(pickUp.name);
            pu.CreateExisting(GameManager.Item.GetAcc(itemId));
            return pu;
        }

        public override Acc_PickUp CreateRandom()
        {
            var pu = pool.Get(pickUp.name);
            pu.CreateExisting(GameManager.Item.RandAcc);
            return pu;
        }

        public Acc_PickUp CreateExisting(Accessory acc)
        {
            var pu = pool.Get(pickUp.name);
            pu.CreateExisting(acc);
            return pu;
        }

        public override List<Acc_PickUp> CreateAll()
        {
            List<Acc_PickUp> list = new();
            var accList = GameManager.Item.Acc.CreateAll();

            foreach (var acc in accList) list.Add(CreateExisting(acc));
            return list;
        }
    }
}