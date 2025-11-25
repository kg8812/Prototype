using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMIdle: ICommonMonsterState<CommonMonster2>
    {
        private CommonMonster2 _cM;
        
        public void OnEnter(CommonMonster2 m)
        {
            _cM = m;
            // idle 들어오기전 체크
        }

        public void Update()
        {
            _cM.CheckRecognition();
        }

        public void FixedUpdate()
        {
            
        }

        public void OnExit()
        {
            
        }

        public void OnCancel()
        {
            
        }
    }
}