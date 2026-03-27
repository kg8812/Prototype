namespace Apis
{
    #region 공격방식 인터페이스

    public interface IAttackStrategy
    {
        public float DmgRatio { get; set; }
        float Calculate(IOnHit target);
    }

    public class FixedAmount : IAttackStrategy
    {
        public FixedAmount(float dmg)
        {
            DmgRatio = dmg;
        }

        public float DmgRatio { get; set; }

        public float Calculate(IOnHit target)
        {
            return DmgRatio;
        }
    }

    public class AtkItemCalculation : IAttackStrategy
    {
        private readonly IAttackItemStat _atkItem;

        private readonly Actor _user;

        public AtkItemCalculation(Actor user, IAttackItemStat atkItem, float dmgRatio = 100)
        {
            _atkItem = atkItem;
            _user = user;
            DmgRatio = dmgRatio;
        }

        public float DmgRatio { get; set; }

        public float Calculate(IOnHit target)
        {
            return (_atkItem.Atk + _user.Atk) * DmgRatio / 100f;
        }
    }

    public class AtkBase : IAttackStrategy
    {
        private readonly float baseDmg;
        private readonly IAttackable user;

        public AtkBase(IAttackable user, float dmgRatio = 100, float baseDmg = 0)
        {
            this.user = user;
            this.DmgRatio = dmgRatio;
            this.baseDmg = baseDmg;
        }

        public float DmgRatio { get; set; }

        public float Calculate(IOnHit target)
        {
            return baseDmg + user.Atk * DmgRatio * 0.01f;
        }
    }

    public class HpBase : IAttackStrategy
    {
        private readonly IOnHit actor;

        public HpBase(IOnHit actor, float dmgRatio = 100)
        {
            this.actor = actor;
            this.DmgRatio = dmgRatio;
        }

        public float DmgRatio { get; set; }

        public float Calculate(IOnHit target)
        {
            return actor.MaxHp * DmgRatio * 0.01f;
        }
    }

    public class TargetCurHpRatio : IAttackStrategy
    {
        public TargetCurHpRatio(float hpRatio)
        {
            DmgRatio = hpRatio;
        }

        public float DmgRatio { get; set; }

        public float Calculate(IOnHit target)
        {
            return target.CurHp * DmgRatio / 100;
        }
    }

    public class StatBase : IAttackStrategy
    {
        private readonly float _baseDmg;
        private readonly ActorStatType _statType;
        private readonly Actor _user;

        public StatBase(Actor user, ActorStatType statType, float baseDmg = 0, float dmgRatio = 100)
        {
            DmgRatio = dmgRatio;
            _baseDmg = baseDmg;
            _user = user;
            _statType = statType;
        }

        public float DmgRatio { get; set; }


        public float Calculate(IOnHit target)
        {
            return _baseDmg + _user.StatManager.GetFinalStat(_statType) * DmgRatio / 100f;
        }
    }

    #endregion
}