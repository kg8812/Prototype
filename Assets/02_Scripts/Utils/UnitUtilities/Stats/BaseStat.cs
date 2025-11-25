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
        
        [LabelText("방어력")] [SerializeField] float def; // 방어력
        
        [LabelText("크리티컬 확률")] [MinValue(0)] [Tooltip("1 = 1%")] [SerializeField]
        float critProb; // 크리티컬 확률

        [LabelText("크리티컬 데미지")] [MinValue(0)] [Tooltip("1대1 대응, 125 = 125% 추가 데미지")] [SerializeField]
        float critDmg; // 크리티컬 데미지
        
        public BaseStat()
        {
            maxHp = 0;
            atk = 0;
            def = 0;
            moveSpeed = 0;
            atkSpeed = 0;
            critProb = 0;
            critDmg = 0;
        }
        public BaseStat(BaseStat other)
        {
            if (other == null) return;
            
            maxHp = other.maxHp;
            atk = other.atk;
            def = other.def;
            moveSpeed = other.moveSpeed;
            atkSpeed = other.atkSpeed;
            critProb = other.critProb;
            critDmg = other.critDmg;
        }
        public static BaseStat operator +(BaseStat a, BaseStat b)
        {
            if (a == null) return b;
            if (b == null) return a;
            BaseStat c = new();
            c.maxHp += a.maxHp + b.maxHp;
            c.atk += a.atk + b.atk;
            c.def += a.def + b.def;
            c.moveSpeed += a.moveSpeed + b.moveSpeed;
            c.atkSpeed += a.atkSpeed + b.atkSpeed;
            c.critProb += a.critProb + b.critProb;
            c.critDmg += a.critDmg + b.critDmg;
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
            }
        }

        public float Get(ActorStatType type)
        {
            return type switch
            {
                ActorStatType.Atk => atk,
                ActorStatType.MoveSpeed => moveSpeed,
                ActorStatType.AtkSpeed => atkSpeed,
                ActorStatType.Def => def,
                ActorStatType.MaxHp => maxHp,
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
            }
        }
    }
}