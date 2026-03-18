using System;
using EventData;

namespace Apis
{
    
    // 버프 사용 규칙, 정책, 진입점 관리 클래스
    
    public class SubBuffManager
    {
        private BonusStat stat;

        public SubBuffManager(IBuffUser user)
        {
            User = user;
            Collector = new SubBuffCollector(this);
        }
        //버프 관리 클래스

        public IBuffUser User { get; }

        public SubBuffCollector Collector { get; }

        public BonusStat Stats
        {
            get
            {
                stat ??= new BonusStat();
                stat.Reset();

                Traverse(x => { stat += x.Stat; });
                return stat;
            }
        }

        public void Traverse(Action<SubBuff> action)
        {
            foreach (var x in Collector.uniqueBuffs.Values)
            foreach (var y in x.buffs.Keys)
            foreach (var z in x[y])
                action(z);

            foreach (var x in Collector.subBuffs.Values)
            foreach (var y in x.List)
                action(y);
        }

        /// <summary>
        ///     특정 효과로 액터에 버프를 추가합니다.
        ///     버프의 수치를(데미지,지속시간 등) 공용이 아닌 다른 수치로 사용하고 싶을 때 사용합니다.
        /// </summary>
        /// <param name="target"> 버프를 부여한 유닛, null 처리해도됨</param>
        /// <param name="buff"> 이 버프를 부여하는 효과</param>
        /// <param name="subBuff"> 추가할 버프</param>
        public bool AddSubBuff(IBuffUser target, Buff buff, SubBuff subBuff)
        {
            if (subBuff == null || buff == null) return false;
            if (IsImmune(subBuff.Type)) return false;

            if (target != null) subBuff.target = target.gameObject;

            Collector.AddBuff(buff, subBuff);
            
            return true;
        }

        /// <summary>
        ///     액터에 타입으로 버프를 추가합니다.
        ///     수치는 SubBuffOptionTable에 입력된 공용 수치를 사용합니다.
        ///     공용 수치는 Type마다 공유합니다.
        /// </summary>
        /// <param name="target">버프를 부여한 유닛, null 처리해도됨</param>
        /// <param name="type">버프 타입</param>
        public SubBuff AddSubBuff(SubBuffType type, IBuffUser target)
        {
            if (IsImmune(type)) return null;

            var sub = Collector.AddSubBuff(type, target?.gameObject);

            return sub;
        }

        public bool RemoveSubBuff(Buff buff, SubBuff subBuff)
        {
            return Collector.RemoveSubBuff(buff, subBuff);
        }

        public SubBuff RemoveSubBuff(Buff buff)
        {
            return Collector.RemoveSubBuff(buff);
        }

        public bool RemoveBuff(Buff buff)
        {
            return Collector.RemoveBuff(buff);
        }

        public void RemoveType(SubBuffType type)
        {
            Collector.RemoveType(type);
        }

        public void RemoveType(SubBuffType type, int stack)
        {
            Collector.RemoveType(type, stack);
        }

        public bool Contains(SubBuffType type)
        {
            return Collector.Contains(type);
        }

        public int Count(SubBuffType type)
        {
            return Collector.Count(type);
        }

        public void Update()
        {
            Collector.Update();
        }

        private bool IsImmune(SubBuffType type)
        {
            return User.ImmunityController.IsImmune(type.ToString());
        }

        public Guid AddImmune(SubBuffType type)
        {
            var t = type.ToString();
            if (!User.ImmunityController.Contains(t)) User.ImmunityController.MakeNewType(t);

            return User.ImmunityController.AddCount(t);
        }

        public void RemoveImmune(SubBuffType type, Guid guid)
        {
            User.ImmunityController.MinusCount(type.ToString(), guid);
        }
    }
}