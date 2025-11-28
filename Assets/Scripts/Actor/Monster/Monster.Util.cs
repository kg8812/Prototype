using Apis.Managers;
using Sirenix.OdinInspector;

namespace Apis
{
    public partial class Monster
    {
        protected override void Start()
        {
            // TODO: 나중에 무조건 pool로 가져와서만 플레이 한다면, 없어도 되는 코드.
            if (isAlreadyCreated) Init(MonsterModel.monsterDict[monsterId]);

            OnActivate.Invoke(this);

            base.Start();
        }

        [Button(ButtonSizes.Large)]
        public void GetMonsterDataFromJson()
        {
            new DatabaseManager().Init();
            MonsterData = MonsterModel.monsterDict[monsterId];
            StatManager.BaseStat.Set(ActorStatType.MaxHp, MonsterData.maxHp);
            StatManager.BaseStat.Set(ActorStatType.Atk, MonsterData.atkPower);
            StatManager.BaseStat.Set(ActorStatType.MoveSpeed, MonsterData.moveSpeed);
            curHp = MonsterData.maxHp;
        }
    }
}