using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New MAS_CheckHp", menuName = "Scriptable/Monster/Attack/MAS_CheckHp")]
    [System.Serializable]
    public class MAS_CheckHp : MonsterAction
    {
        public string boolName;
        public bool isUpdating = false;
        public bool isRatio;
        public bool isGreater;
        [Tooltip("100분율(100=100%) or 실제 hp")] public int value;
        
        
        public override void Action(CommonMonster2 monster)
        {
            base.Action(monster);
            isPlayed = false;
            if (!isUpdating)
            {
                CheckHp();
            }
        }

        protected virtual void CheckHp()
        {
            if (isRatio)
            {
                RealAction((_cM.CurHp / _cM.MaxHp > value * 0.01f) == isGreater);
            }
            else
            {
                RealAction((_cM.CurHp > value) == isGreater);
            }
        }

        protected virtual void RealAction(bool isTrue)
        {
            if (isTrue)
            {
                isPlayed = true;
            }
            if (_cM.PgController.patternReturnCount < 1)
            {
                if (isTrue)
                {
                    _cM.PgController.patternReturnCount++;
                }
                _cM.animator.SetBool(boolName, isTrue);
            }
            else
            {
                _cM.animator.SetBool(boolName, isTrue);
            }
        }

        public override void Update()
        {
            if (isUpdating && !isPlayed)
            {
                CheckHp();
            }
        }

        public override void FixedUpdate()
        {
        }

        public override void OnCancel()
        {
        }
    }
}