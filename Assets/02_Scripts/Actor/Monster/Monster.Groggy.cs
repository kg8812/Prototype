using System.Collections;
using UnityEngine;

namespace Apis
{
    public partial class Monster
    {
        // 참조 dragon
        public bool IsGroggy { get; set; }
        public virtual float CurGroggyGauge
        {
            get => _curGroggyGauge;
            set
            {
                _curGroggyGauge = Mathf.Min(value, MonsterData.groggyGauge);
                CurGroggyGaugeChanged?.Invoke(_curGroggyGauge / MonsterData.groggyGauge);
            }
        }
        public OnFloatValueChanged CurGroggyGaugeChanged;
        
        
        // privates
        private float _curGroggyGauge;
        private Coroutine recoverGroggyCoroutine;
        
        
        
        protected virtual void Groggyed()
        {
            float duration = 0;
            CurGroggyGauge = 0;
            switch (MonsterData.monsterType)
            {
                case MonsterType.Common :
                    duration = FormulaConfig.commonGroggyDuration;
                    break;
                case MonsterType.Elite:
                    duration = FormulaConfig.eliteGroggyDuration;
                    break;
                case MonsterType.Boss:
                    duration = FormulaConfig.bossGroggyDuration;
                    break;
            }
            StartStun(GameManager.instance.Player,duration);
        }
        
        
        // animation clip behavior에서 call event로 함수 실행
        public void GroggyLoopStart()
        {
            float duration = MonsterData.monsterType switch
            {
                MonsterType.Common => FormulaConfig.commonGroggyDuration,
                MonsterType.Elite => FormulaConfig.eliteGroggyDuration,
                MonsterType.Boss => FormulaConfig.bossGroggyDuration,
                _ => 0
            };
            SubBuffManager.AddCC(GameManager.instance.Player,SubBuffType.Debuff_Stun,duration);
        }
        
        private IEnumerator RecoverGroggyGauge()
        {
            yield return new WaitForSeconds(FormulaConfig.groggyRecoverDelay);
            float decreaseCntPerSec = 20;
            while (CurGroggyGauge > 0)
            {
                CurGroggyGauge -= FormulaConfig.groggyRecoverDelay / decreaseCntPerSec;
                if (CurGroggyGauge < 0) CurGroggyGauge = 0;
                yield return new WaitForSeconds(1/decreaseCntPerSec);
            }
        }
        
        // 플레이어, 무기에서 호출해줘야 함.
        public void AddGroggyGauge(float addedValue)
        {
            if (IsGroggy) return;
            
            CurGroggyGauge += addedValue;
            
            if (CurGroggyGauge >= MonsterData.groggyGauge)
            {
                Groggyed();
            }
        }
        
        
        private void LastHit(EventParameters parameters)
        {
            if (IsGroggy) return;
            if (recoverGroggyCoroutine != null)
            {
                StopCoroutine(recoverGroggyCoroutine);
            }

            recoverGroggyCoroutine = StartCoroutine(RecoverGroggyGauge());
        }
    }
}