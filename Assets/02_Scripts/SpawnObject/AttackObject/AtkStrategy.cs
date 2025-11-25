using UnityEngine;

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
        public float DmgRatio { get; set; }

        public FixedAmount(float dmg)
        {
            DmgRatio = dmg;
        }
        public float Calculate(IOnHit target)
        {
            return DmgRatio;
        }
    }
    
    public class AtkItemCalculation : IAttackStrategy
    {
        public float DmgRatio { get; set; }

        private Actor _user;
        private IAttackItemStat _atkItem;

        public AtkItemCalculation(Actor user, IAttackItemStat atkItem,float dmgRatio = 100)
        {
            _atkItem = atkItem;
            _user = user;
            DmgRatio = dmgRatio;
        }
        public float Calculate(IOnHit target)
        {
            return (_atkItem.Atk + _user.Atk) * DmgRatio / 100f;
        }
    }

    public class AtkBase : IAttackStrategy
    {
        private IAttackable user;
        private float dmgRatio;
        private float baseDmg;
        public AtkBase(IAttackable user, float dmgRatio = 100,float baseDmg = 0)
        {
            this.user = user;
            this.dmgRatio = dmgRatio;
            this.baseDmg = baseDmg;
        }

        public float DmgRatio
        {
            get => dmgRatio;
            set => dmgRatio = value;
        }

        public float Calculate(IOnHit target)
        {
            return baseDmg + user.Atk * dmgRatio * 0.01f;
        }
    }

    public class HpBase : IAttackStrategy
    {
        private IOnHit actor;
        private float dmgRatio;
        public float DmgRatio
        {
            get => dmgRatio;
            set => dmgRatio = value;
        }
        public HpBase(IOnHit actor, float dmgRatio = 100)
        {
            this.actor = actor;
            this.dmgRatio = dmgRatio;
        }
        public float Calculate(IOnHit target)
        {
            return actor.MaxHp * dmgRatio * 0.01f;
        }
    }
    
    public class TargetCurHpRatio : IAttackStrategy
    {
        private float dmgRatio;
        
        public float DmgRatio
        {
            get => dmgRatio;
            set => dmgRatio = value;
        }

        public TargetCurHpRatio(float hpRatio)
        {
            dmgRatio = hpRatio;
        }
        public float Calculate(IOnHit target)
        {
            return target.CurHp * dmgRatio / 100;
        }
    }
    public class StatBase : IAttackStrategy
    {
        private Actor _user;
        private ActorStatType _statType;
        private float _baseDmg;
        public StatBase(Actor user,ActorStatType statType, float baseDmg = 0, float dmgRatio = 100)
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
