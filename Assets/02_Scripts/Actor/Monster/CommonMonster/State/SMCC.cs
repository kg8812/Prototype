using UnityEngine;
using Default;

namespace Apis.CommonMonster2
{
    public class SMCC : ICommonMonsterState<CommonMonster2>
    {
        private readonly int _groggyInd = Animator.StringToHash("groggy");
        private readonly int _forceTurn = Animator.StringToHash("forceTurn");
        private CommonMonster2 _cM;

        public void OnEnter(CommonMonster2 m)
        {
            _cM = m;
            _cM.PgController.ForceCancel();
            _cM.animator.SetTrigger(_groggyInd);
            _cM.IsGroggy = true;
        }

        public void Update()
        {
            _cM.CheckRecognition();
        }

        public void FixedUpdate()
        {
        }

        public void OnCancel()
        {
        }

        public void OnExit()
        {
            _cM.animator.ResetTrigger(_groggyInd);
            // if (!_cM.IsRecognized)
            // {
            //     _cM.TryChangeMonsterState(MonsterState.Idle);
            // }
            // else if (_cM.ableMove && !_cM.CheckPlayerRl() && _cM.TryChangeMonsterState(MonsterState.Turn))
            // {
            //     _cM.PreState = MonsterState.Move;
            //     _cM.animator.SetTrigger(_forceTurn);
            // }
            // else
            // {
            //     _cM.animator.SetTrigger(_groggyEndInd);
            // }

            _cM.IsGroggy = false;
        }
    }
}