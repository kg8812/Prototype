using System.Collections.Generic;
using Apis;

namespace Apis
{
    public class Factory_ActiveSkillPickUp : ItemFactory<ActiveSkill_PickUp>
    {
        //무기 픽업 팩토리

        readonly ActiveSkill_PickUp pickUp;
        public Factory_ActiveSkillPickUp(ActiveSkill_PickUp[] pickUps) : base(pickUps)
        {
            pickUp = pickUps[0];
        }

        public override ActiveSkill_PickUp CreateNew(int itemId)
        {
            ActiveSkill_PickUp pu = pool.Get(pickUp.name);
            pu.CreateExisting(GameManager.Item.GetActiveSkill(itemId));
            return pu;
        }

        public override ActiveSkill_PickUp CreateRandom()
        {
            ActiveSkill_PickUp pu = pool.Get(pickUp.name);
            pu.CreateExisting(GameManager.Item.RandActiveSkill);
            return pu;
        }

        public ActiveSkill_PickUp CreateExisting(ActiveSkillItem skill)
        {
            ActiveSkill_PickUp pu = pool.Get(pickUp.name);
            pu.CreateExisting(skill);
            return pu;
        }

        public override List<ActiveSkill_PickUp> CreateAll()
        {
            List<ActiveSkill_PickUp> list = new();
            List<ActiveSkillItem> skillList = GameManager.Item.ActiveSkillItem.CreateAll();

            foreach (var sk in skillList)
            {
                list.Add(CreateExisting(sk));
            }
            return list;
        }
    }
}