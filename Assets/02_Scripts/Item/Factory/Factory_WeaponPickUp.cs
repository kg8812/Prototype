using System.Collections.Generic;

namespace Apis
{
    public class Factory_WeaponPickUp : ItemFactory<Weapon_PickUp>
    {
        //무기 픽업 팩토리

        readonly Weapon_PickUp pickUp;
        public Factory_WeaponPickUp(Weapon_PickUp[] pickUps) : base(pickUps)
        {
            pickUp = pickUps[0];
        }

        public override Weapon_PickUp CreateNew(int itemId)
        {
            Weapon_PickUp pu = pool.Get(pickUp.name);
            pu.CreateAlready(GameManager.Item.GetWeapon(itemId));
            return pu;
        }

        public override Weapon_PickUp CreateRandom()
        {
            Weapon_PickUp pu = pool.Get(pickUp.name);
            pu.CreateAlready(GameManager.Item.RandWeapon);
            return pu;
        }

        public Weapon_PickUp CreateExisting(Weapon weapon)
        {
            Weapon_PickUp pu = pool.Get(pickUp.name);
            pu.CreateAlready(weapon);
            return pu;
        }

        public override List<Weapon_PickUp> CreateAll()
        {
            List<Weapon_PickUp> list = new();
            List<Weapon> wpList = GameManager.Item.Weapon.CreateAll();

            foreach (var wp in wpList)
            {
                list.Add(CreateExisting(wp));
            }
            return list;
        }
    }
}
