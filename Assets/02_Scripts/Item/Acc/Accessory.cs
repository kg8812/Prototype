using System;
using System.Collections;
using Apis.DataType;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Apis
{
    public class Accessory : Item
    {
        // 악세사리 클래스                         
        // protected new string name;
        // protected string flavourText;
        // protected string description;

        public override int ItemId => dataId;
        // public override string Name => name;
        //
        // public override string FlavourText => flavourText;
        //
        // public override string Description => description;
        
        protected BonusStat _bonusStat;

        public virtual BonusStat BonusStat => _bonusStat ??= new();
        
        AccessoryDataType data;

        [LabelText("테이블 데이터")]
        [SerializeField] int dataId;
        
        public int Index => dataId;
        public AccessoryDataType Data => data;
        private int grade;
        public int Grade => grade;

        protected bool isCd;
        protected bool isDuration;

        protected Action OnDurationStart;
        protected Action OnDurationEnd;

        public override void Init()
        {
            base.Init();
            AccessoryData.DataLoad.TryGetData(Index, out data);

            // name = LanguageManager.Str(data.accName);
            // flavourText = LanguageManager.Str(data.accFlavorText);
            // description = LanguageManager.Str(data.accDesc);
            
            if (Image == null)
            {
                Image = ResourceUtil.Load<Sprite>(data.iconPath);
            }

            isCd = false;
            isDuration = false;
            grade = data.grade;
        }

        public override void Activate()
        {
        }

        public override void Return()
        {
            base.Return();
            GameManager.Item.Acc.Return(this);
        }

        private void OnDestroy()
        {
            if (Image != null)
            {
                Addressables.Release(Image);
            }
        }

        protected IEnumerator CDCoroutine(float cd)
        {
            if (isCd) yield break;
            isCd = true;
            yield return new WaitForSeconds(cd);
            isCd = false;
        }
        protected IEnumerator DurationCoroutine(float duration)
        {
            if (isDuration) yield break;
            OnDurationStart?.Invoke();
            isDuration = true;
            yield return new WaitForSeconds(duration);
            isDuration = false;
            OnDurationEnd?.Invoke();
        }
    }
}
