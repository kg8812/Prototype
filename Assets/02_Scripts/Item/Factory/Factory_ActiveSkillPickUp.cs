using System.Collections.Generic;

namespace Apis
{
    public class Factory_ActiveSkillPickUp : ItemFactory<ActiveSkill_PickUp>
    {
        //무기 픽업 팩토리

        private readonly ActiveSkill_PickUp pickUp;

        public Factory_ActiveSkillPickUp(ActiveSkill_PickUp[] pickUps) : base(pickUps)
        {
            pickUp = pickUps[0];
        }

        public override ActiveSkill_PickUp CreateNew(int itemId)
        {
            var pu = pool.Get(pickUp.name);
            pu.CreateExisting(GameManager.Item.GetActiveSkill(itemId));
            return pu;
        }

        public override ActiveSkill_PickUp CreateRandom()
        {
            var pu = pool.Get(pickUp.name);
            pu.CreateExisting(GameManager.Item.RandActiveSkill);
            return pu;
        }

        public ActiveSkill_PickUp CreateExisting(ActiveSkillItem skill)
        {
            var pu = pool.Get(pickUp.name);
            pu.CreateExisting(skill);
            return pu;
        }

        public override List<ActiveSkill_PickUp> CreateAll()
        {
            List<ActiveSkill_PickUp> list = new();
            var skillList = GameManager.Item.ActiveSkillItem.CreateAll();

            foreach (var sk in skillList) list.Add(CreateExisting(sk));
            return list;
        }
    }
}