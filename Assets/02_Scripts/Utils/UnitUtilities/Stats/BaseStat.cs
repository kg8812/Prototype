using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    [Serializable]
    [HideLabel]
    public class BaseStat
    {
        // 인스펙터와 실제값을 동일시 하기 위해 Dictionary가 아닌 기본값 형식을 사용했음.

        // 스탯값들 저장
        [MinValue(0)] [LabelText("최대체력")] [SerializeField]
        private float maxHp; // 최대 체력    

        [MinValue(0)] [LabelText("공격력")] [SerializeField]
        float atk; // 공격력

        [MinValue(0)] [LabelText("이동속도")] [Tooltip("100 = 초당 1m")] [SerializeField]
        float moveSpeed; // 이동속도

        [LabelText("공격속도")] [Tooltip("1 = 1% 추가")] [SerializeField]
        float atkSpeed; // 공격속도

        [LabelText("피해 감소량")] [Tooltip("1 = 1% 감소")] [SerializeField]
        float dmgReduce; // 피해 감소량

        [LabelText("방어력")] [SerializeField] float def; // 방어력

        [LabelText("정신력")] [MinValue(0)] [SerializeField]
        float mental; // 정신력

        [LabelText("크리티컬 확률")] [MinValue(0)] [Tooltip("1 = 1%")] [SerializeField]
        float critProb; // 크리티컬 확률

        [LabelText("크리티컬 데미지")] [MinValue(0)] [Tooltip("1대1 대응, 125 = 125% 추가 데미지")] [SerializeField]
        float critDmg; // 크리티컬 데미지

        [LabelText("결속력")] [SerializeField] float cdReduction; // 결속력 (쿨타임)
        [LabelText("재화 획득량")] [SerializeField] float goldRate; // 재화획득량

        [LabelText("추가 데미지 %")] [SerializeField]
        float extraDmg; // 추가 데미지

        [LabelText("회복량")] [SerializeField] private float healRate; // 회복량
        [LabelText("쉴드 획득량")] [SerializeField] private float shieldRate; // 쉴드 획득량

        [LabelText("크리티컬 당할 확률")] [Tooltip("1 = 1%")] [SerializeField]
        private float critHit; // 크르티컬 당할 확률

        [LabelText("신체")] [MinValue(0)] [SerializeField]
        private float body;

        [LabelText("영혼")] [MinValue(0)] [SerializeField]
        private float spirit;

        [LabelText("기량")] [MinValue(0)] [SerializeField]
        private float finesse;

        public BaseStat()
        {
            maxHp = 0;
            atk = 0;
            def = 0;
            mental = 0;
            moveSpeed = 0;
            atkSpeed = 0;
            dmgReduce = 0;
            critProb = 0;
            critDmg = 0;
            cdReduction = 0;
            goldRate = 0;
            extraDmg = 0;
            healRate = 0;
            shieldRate = 0;
            critHit = 0;
            body = 0;
            spirit = 0;
            finesse = 0;
        }
        public BaseStat(BaseStat other)
        {
            if (other == null) return;
            
            maxHp = other.maxHp;
            atk = other.atk;
            def = other.def;
            mental = other.mental;
            moveSpeed = other.moveSpeed;
            atkSpeed = other.atkSpeed;
            dmgReduce = other.dmgReduce;
            critProb = other.critProb;
            critDmg = other.critDmg;
            cdReduction = other.cdReduction;
            goldRate = other.goldRate;
            extraDmg = other.extraDmg;
            healRate = other.healRate;
            shieldRate = other.shieldRate;
            critHit = other.critHit;
            body = other.body;
            spirit = other.spirit;
            finesse = other.finesse;
        }
        public static BaseStat operator +(BaseStat a, BaseStat b)
        {
            if (a == null) return b;
            if (b == null) return a;
            BaseStat c = new();
            c.maxHp += a.maxHp + b.maxHp;
            c.atk += a.atk + b.atk;
            c.def += a.def + b.def;
            c.mental += a.mental + b.mental;
            c.moveSpeed += a.moveSpeed + b.moveSpeed;
            c.atkSpeed += a.atkSpeed + b.atkSpeed;
            c.dmgReduce += a.dmgReduce + b.dmgReduce;
            c.critProb += a.critProb + b.critProb;
            c.critDmg += a.critDmg + b.critDmg;
            c.cdReduction += a.cdReduction + b.cdReduction;
            c.goldRate += a.goldRate + b.goldRate;
            c.extraDmg += a.extraDmg + b.extraDmg;
            c.healRate += a.healRate + b.healRate;
            c.shieldRate += a.healRate + b.healRate;
            c.critHit += a.critHit + b.critHit;
            c.body += a.body + b.body;
            c.spirit += a.spirit + b.spirit;
            c.finesse += a.finesse + b.finesse;
            return c;
        }

        public void Add(ActorStatType type, float value)
        {
            switch (type)
            {
                case ActorStatType.MaxHp:
                    maxHp += value;
                    break;
                case ActorStatType.Atk:
                    atk += value;
                    break;
                case ActorStatType.Def:
                    def += value;
                    break;
                case ActorStatType.MoveSpeed:
                    moveSpeed += value;
                    break;
                case ActorStatType.AtkSpeed:
                    atkSpeed += value;
                    break;
                case ActorStatType.DmgReduce:
                    dmgReduce += value;
                    break;
                case ActorStatType.Mental:
                    mental += value;
                    break;
                case ActorStatType.CDReduction:
                    cdReduction += value;
                    break;
                case ActorStatType.CritProb:
                    critProb += value;
                    break;
                case ActorStatType.CritDmg:
                    critDmg += value;
                    break;
                case ActorStatType.GoldRate:
                    goldRate += value;
                    break;
                case ActorStatType.ExtraDmg:
                    extraDmg += value;
                    break;
                case ActorStatType.HealRate:
                    healRate += value;
                    break;
                case ActorStatType.ShieldRate:
                    shieldRate += value;
                    break;
                case ActorStatType.CritHit:
                    critHit += value;
                    break;
                case ActorStatType.Body:
                    body += value;
                    break;
                case ActorStatType.Spirit:
                    spirit += value;
                    break;
                case ActorStatType.Finesse:
                    finesse += value;
                    break;
            }
        }

        public float Get(ActorStatType type)
        {
            return type switch
            {
                ActorStatType.Atk => atk,
                ActorStatType.MoveSpeed => moveSpeed,
                ActorStatType.AtkSpeed => atkSpeed,
                ActorStatType.DmgReduce => dmgReduce,
                ActorStatType.Def => def,
                ActorStatType.Mental => mental,
                ActorStatType.MaxHp => maxHp,
                ActorStatType.CritProb => critProb,
                ActorStatType.CritDmg => critDmg,
                ActorStatType.CDReduction => cdReduction,
                ActorStatType.GoldRate => goldRate,
                ActorStatType.ExtraDmg => extraDmg,
                ActorStatType.HealRate => healRate,
                ActorStatType.ShieldRate => shieldRate,
                ActorStatType.CritHit => critHit,
                ActorStatType.Body => body,
                ActorStatType.Spirit => spirit,
                ActorStatType.Finesse => finesse,
                _ => 0
            };
        }

        public void Set(ActorStatType type, float value)
        {
            switch (type)
            {
                case ActorStatType.MaxHp:
                    maxHp = value;
                    break;
                case ActorStatType.Atk:
                    atk = value;
                    break;
                case ActorStatType.Def:
                    def = value;
                    break;
                case ActorStatType.MoveSpeed:
                    moveSpeed = value;
                    break;
                case ActorStatType.AtkSpeed:
                    atkSpeed = value;
                    break;
                case ActorStatType.DmgReduce:
                    dmgReduce = value;
                    break;
                case ActorStatType.Mental:
                    mental = value;
                    break;
                case ActorStatType.CritProb:
                    critProb = value;
                    break;
                case ActorStatType.CritDmg:
                    critDmg = value;
                    break;
                case ActorStatType.CDReduction:
                    cdReduction = value;
                    break;
                case ActorStatType.GoldRate:
                    goldRate = value;
                    break;
                case ActorStatType.ExtraDmg:
                    extraDmg = value;
                    break;
                case ActorStatType.HealRate:
                    healRate = value;
                    break;
                case ActorStatType.ShieldRate:
                    shieldRate = value;
                    break;
                case ActorStatType.CritHit:
                    critHit = value;
                    break;
                case ActorStatType.Body:
                    body = value;
                    break;
                case ActorStatType.Spirit:
                    spirit = value;
                    break;
                case ActorStatType.Finesse:
                    finesse = value;
                    break;
            }
        }
    }
}