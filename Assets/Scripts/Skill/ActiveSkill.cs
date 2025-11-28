using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public abstract class ActiveSkill : Skill
    {
        public enum ActiveEnums
        {
            Instant,
            Charge,
            Casting,
            Continuous,
            Toggle
        }

        [TabGroup("사용 관련/설정", "기본설정")] [LabelText("공격중 사용가능 여부")] [SerializeField]
        public bool usableWhenAttack;

        [TabGroup("사용 관련/설정", "기본설정")] [LabelText("플레이어 상태체크 여부")] [SerializeField]
        public bool usePlayerCheck = true;

        [HideInInspector] public float chargeDmgRatio;
        [HideInInspector] public float chargeRatio;
        private ActiveEnums baseActiveType;

        [ShowIfGroup("사용 관련/Casting", Condition = "_activeType", Value = ActiveEnums.Casting)]
        [TabGroup("사용 관련/Casting/설정", "캐스팅 설정")]
        [LabelText("캐스팅 시간")]
        public float CastTime;

        [ShowIfGroup("사용 관련/Casting", Condition = "_activeType", Value = ActiveEnums.Casting)]
        [TabGroup("사용 관련/Casting/설정", "캐스팅 설정")]
        [LabelText("캐스팅 모션타입")]
        [DetailedInfoBox("모션목록 확인", "모션 목록 \n 0 : staff_attack_5\n 1 : staff_attack_6")]
        public int CastType;

        [ShowIfGroup("사용 관련/Charge", Condition = "_activeType", Value = ActiveEnums.Charge)]
        [TabGroup("사용 관련/Charge/설정", "차징 설정")]
        [LabelText("차징 시간")]
        public float ChargeTime;

        [ShowIfGroup("사용 관련/Charge", Condition = "_activeType", Value = ActiveEnums.Charge)]
        [TabGroup("사용 관련/Charge/설정", "차징 설정")]
        [LabelText("차징 자동완료 여부")]
        public bool EndChargeAutomatic = true;

        [ShowIfGroup("사용 관련/Charge", Condition = "_activeType", Value = ActiveEnums.Charge)]
        [TabGroup("사용 관련/Charge/설정", "차징 설정")]
        [LabelText("이동속도 감소율")]
        public float chargeMoveDebuff;

        [ShowIfGroup("사용 관련/Charge", Condition = "_activeType", Value = ActiveEnums.Charge)]
        [TabGroup("사용 관련/Charge/설정", "차징 설정")]
        [LabelText("차징시간 및 데미지")]
        public List<ChargeInfo> chargeInfos = new();

        [ShowIfGroup("사용 관련/Charge", Condition = "_activeType", Value = ActiveEnums.Charge)]
        [TabGroup("사용 관련/Charge/설정", "차징 설정")]
        [LabelText("차징 모션타입")]
        [DetailedInfoBox("모션목록 확인", "모션 목록 \n 0 : rifle_attack_3\n 1 : punch_attack_9")]
        public int chargeType;

        [ShowIfGroup("사용 관련/toggle", Condition = "_activeType", Value = ActiveEnums.Toggle)]
        [TabGroup("사용 관련/toggle/설정", "토글 설정")]
        [LabelText("토글 변경스킬")]
        [SerializeField]
        private ActiveSkill skill2;

        [ShowIfGroup("사용 관련/toggle", Condition = "_activeType", Value = ActiveEnums.Toggle)]
        [LabelText("프레임 사용여부")]
        [InfoBox("True : 토글 시 아이콘의 프레임이 켜짐 \n False : 프레임이 켜지지 않음")]
        [TabGroup("사용 관련/toggle/설정", "토글 설정")]
        [SerializeField]
        private bool useFrame;

        [HideInInspector] public ActiveSkill Skill2;
        private Dictionary<ActiveEnums, ISkillActive> _actives;

        private UnityEvent _onChargeStart = new();
        private UnityEvent _onChargeEnd = new();
        private UnityEvent _whenCharging = new();

        [HideInInspector] public bool isCharging;
        private List<(float, UnityAction)> _list;

        private float originalDmg;
        private float original;

        private List<UnityAction> _actionList;

        protected abstract ActiveEnums _activeType { get; }
        protected virtual bool fixedActiveType => true;

        public override float Atk => ActiveStrategy.CalculateDmg(base.Atk);

        public virtual UI_AtkItemIcon Icon => Item?.Icon;
        public UnityEvent OnChargeStart => _onChargeStart ??= new UnityEvent();
        public UnityEvent OnChargeEnd => _onChargeEnd ??= new UnityEvent();
        public UnityEvent WhenCharging => _whenCharging ??= new UnityEvent();

        public ISkillActive ActiveStrategy
        {
            get
            {
                if (_actives == null) ResetActiveType();

                return fixedActiveType ? _actives[_activeType] : _actives[baseActiveType];
            }
        }

        public ActiveEnums ActiveType => _activeType;


        private List<(float, UnityAction)> list => _list ??= new List<(float, UnityAction)>();

        public IAttackItem Item { get; set; }
        public List<UnityAction> actionList => _actionList ??= new List<UnityAction>();

        public override bool TryUse()
        {
            return CDActive.CheckActive() && ActiveStrategy.CheckUsable;
        }

        public virtual void BeforeAttack()
        {
        }

        private void ResetActiveType()
        {
            _actives = new Dictionary<ActiveEnums, ISkillActive>
            {
                { ActiveEnums.Instant, new InstantSkill(this) },
                { ActiveEnums.Casting, new CastingSkill(this) },
                { ActiveEnums.Continuous, new ContinuousSkill(this) }
            };

            for (var i = 0; i < chargeInfos.Count; i++)
            {
                var temp = i;
                list.Add((chargeInfos[i].time, () => ChargeInvoke(temp)));
            }

            _actives.Add(ActiveEnums.Charge, new ChargeSkill(list, this));

            if (skill2 != null)
            {
                Skill2 = Instantiate(skill2);
                _actives.Add(ActiveEnums.Toggle, new ToggleSkill(this, Skill2, useFrame));
            }
        }

        public void ChangeToChargeSkill(List<ChargeInfo> infos)
        {
            chargeInfos = infos;
            baseActiveType = ActiveEnums.Charge;
            ChargeTime = chargeInfos[^1].time;
            if (_actives == null) ResetActiveType();
            list.Clear();
            for (var i = 0; i < chargeInfos.Count; i++)
            {
                var temp = i;

                list.Add((chargeInfos[i].time, () => ChargeInvoke(temp)));
                _actives[ActiveEnums.Charge] = new ChargeSkill(list, this);
            }
        }

        public void ChangeActiveType(ActiveEnums type)
        {
            baseActiveType = type;
        }

        protected virtual void ChargeInvoke(int idx)
        {
            chargeDmgRatio = chargeInfos[idx].dmg;
        }

        public override bool Use()
        {
            if (!base.Use()) return false;
            if (activeUser != null) activeUser.curSkill = this;

            chargeDmgRatio = 0;
            chargeRatio = 100;
            ActiveStrategy.Activate(this);

            return true;
        }

        public void DeActive()
        {
            ActiveStrategy.DeActivate(this);
        }

        protected override void StopDuration()
        {
            base.StopDuration();
            CDActive.SetIconCdType(Icon);
        }

        public override IEnumerator DurationCoroutine()
        {
            if (IsUse || Mathf.Approximately(Duration, 0)) yield break;

            Icon.ChangeType(new UI_AtkItemIcon.DurationUpdate(Icon));

            yield return base.DurationCoroutine();

            CDActive.SetIconCdType(Icon);
        }

        protected override void OnEquip(IMonoBehaviour owner) //착용시
        {
            base.OnEquip(owner);
            // Debug.Log($"is item null? {Item == null}");
            CDActive.SetIconCdType(Icon);
        }

        protected override void OnUnEquip()
        {
            base.OnUnEquip();
            ActiveStrategy.OnUnEquip(this);
        }

        public virtual void StartCharge()
        {
            isCharging = true;
            OnChargeStart.Invoke();
        }

        public override void Init()
        {
            base.Init();
            baseActiveType = ActiveEnums.Instant;
            chargeDmgRatio = 0;
            isCharging = false;
            if (_actives == null) ResetActiveType();
        }

        public virtual void EndMotion()
        {
            IsUse = false;
        }

        public override void Active()
        {
            base.Active();

            IsUse = true;
        }

        protected override void StartDuration()
        {
            base.StartDuration();
        }

        public override void Cancel()
        {
            base.Cancel();
            ActiveStrategy.OnCancel();
        }

        [Serializable]
        public struct ChargeInfo
        {
            [LabelText("필요 시간")] public float time;

            [InfoBox("입력한 수치값으로 조정됩니다. 200% 입력시 200% 증가가 아닌 200%로 조정")] [LabelText("데미지 설정값(%)")]
            public float dmg;

            public static ChargeInfo operator +(ChargeInfo a, ChargeInfo b)
            {
                ChargeInfo c = new()
                {
                    time = a.time + b.time,
                    dmg = a.dmg + b.dmg
                };

                return c;
            }

            public static ChargeInfo operator -(ChargeInfo a, ChargeInfo b)
            {
                ChargeInfo c = new()
                {
                    time = a.time - b.time,
                    dmg = a.dmg - b.dmg
                };

                return c;
            }
        }
    }
}