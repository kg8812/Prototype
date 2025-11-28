using System;
using Apis.DataType;
using Default;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Apis
{
    public partial class Buff
    {
        private IApplyType _applyStrategy;
        private BuffDataType _data;

        public Actor buffActor;

        public BuffDispellStrategy.IDispell dispell;
        public Actor subBuffActor;

        public Buff(BuffDataType data, IEventUser act)
        {
            _data = data;
            CreateDispell();
            // BuffDesc = LanguageManager.Str(data.buffDesc);
            switch (ApplyType)
            {
                case 0:
                    _applyStrategy = new NormalApply(this);
                    break;
                case 1:
                    _applyStrategy = new PermanentApply(this);
                    break;
                case 2:
                    _applyStrategy = new TempApply(this);
                    break;
            }

            Icon = ResourceUtil.Load<Sprite>(_data.buffIconPath);

            Array.Resize(ref data.buffPower, 5);
            buffActor = act as Actor;
        }

        public SubBuff ActivatedSubBuff { get; private set; }

        public void SetData(BuffDataType data)
        {
            _data = data;
            CreateDispell();
            // BuffDesc = LanguageManager.Str(data.buffDesc);
            switch (ApplyType)
            {
                case 0:
                    _applyStrategy = new NormalApply(this);
                    break;
                case 1:
                    _applyStrategy = new PermanentApply(this);
                    break;
                case 2:
                    _applyStrategy = new TempApply(this);
                    break;
            }

            Icon = ResourceUtil.Load<Sprite>(_data.buffIconPath);

            Array.Resize(ref data.buffPower, 5);
        }

        ~Buff()
        {
            if (Icon != null) Addressables.Release(Icon);
        }

        private void CreateDispell()
        {
            switch (BuffDispellType)
            {
                case 0:
                case 1:
                    dispell = new BuffDispellStrategy.Nothing();
                    break;
                case 2:
                    dispell = new BuffDispellStrategy.OnHit();
                    break;
                case 3:
                    dispell = new BuffDispellStrategy.OnAttackSuccess();
                    break;
                case 4:
                    dispell = new BuffDispellStrategy.OnDeath();
                    break;
                case 5:
                    dispell = new BuffDispellStrategy.OnMasterHit();
                    break;
                case 6:
                    dispell = new BuffDispellStrategy.OnAttackEnd();
                    break;
                case 7:
                    dispell = new BuffDispellStrategy.OnSubBuffRemove();
                    break;
            }
        }

        public void RemoveBuff(EventParameters _)
        {
            if (subBuffActor != null) subBuffActor.RemoveSubBuff(this);
        }

        public SubBuff AddSubBuff(Actor actor, EventParameters parameters)
        {
            subBuffActor = actor;
            ActivatedSubBuff = SubBuffResources.Get(this);

            if (ActivatedSubBuff == null)
            {
                subBuffActor = null;
                return null;
            }

            ActivatedSubBuff.buff = this;
            _applyStrategy.Apply(actor, parameters);
            return ActivatedSubBuff;
        }

        #region 파라미터 설명

        /* BuffMainType,SubType : 버프타입 인덱스 (생성자에서 자동 할당함)
         * BuffPower : 버프 수치 배열 (데미지 등)
         * BuffCategory : 버프의 분류 (0 : 버프 타입별 , 1 : 효과별로 나눠짐)
         * BuffDuration : 버프 지속시간
         * BuffDispelType : 0 - 해제되지 않음
                             1 - 시간이 지나면 사라짐
                             2 - 오브젝트가 피해 받았을때
                             3 - 오브젝트가 피해 주었을 때
                             4 - 오브젝트가 파괴될 때
                             5 - 주인이 피격됐을 때
                             6 - 기본 공격 모션 끝날 때
                             7 - 서브버프가 해제될 때

         *ApplyType : 0 - 지속시간 혹은 착용중에 적용
                      1 - 영구 적용 (해제 후에도 지속됨)
                      2 - 일시 적용 (공격 한 번 등)

         BuffMaxStack : 최대 스택
         StackDecrease : 스택 감소 방식 (0 : 한개씩, 1 : 한번에 전부)
         ValueType : 값 적용 방식 (Value : 고정값, Ratio : %)
         ShowIcon : 아이콘 표시 여부
         BuffIconPath : 아이콘 스프라이트 주소
         BuffName : 버프 이름 string 인덱스
         BuffDesc : 버프 설명 string 인덱스
         */

        #endregion

        #region 테이블 파라미터

        public int BuffIndex => _data.index;

        public int BuffName
        {
            get => _data.buffName;
            set => _data.buffName = value;
        }

        public int BuffDesc => _data.buffDesc;

        // [CanBeNull] public string BuffDesc;
        public int BuffMainType => _data.buffMainType;
        public int BuffSubType => _data.buffSubType;
        public float[] BuffPower => _data.buffPower;

        public float BuffDuration
        {
            get => _data.buffDuration;
            set => _data.buffDuration = value;
        }

        public int ApplyType => _data.applyType;

        public int BuffDispellType => _data.buffDispellType;

        public int BuffMaxStack => _data.buffMaxStack;

        public int BuffCategory => _data.buffCategory;
        public int StackDecrease => _data.stackDecrease;
        public ValueType ValueType => _data.valueType;
        public bool ShowIcon => _data.showIcon;
        public int ApplyStrategy => _data.applyStrategy;
        public Sprite Icon;

        #endregion
    }
}