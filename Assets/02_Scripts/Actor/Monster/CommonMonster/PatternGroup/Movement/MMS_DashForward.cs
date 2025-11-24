using DG.Tweening;
using UnityEngine;

namespace Apis.CommonMonster2
{
    
    
    [CreateAssetMenu(fileName = "New DashForward", menuName = "Scriptable/Monster/Movement/dashForward")]
    [System.Serializable]
    public class MMS_DashForward : MonsterAction
    {
        public float time;
        public float distance;
        public bool isBackDash;
        public bool isResisOff;

        private Tweener tweener;
        public override void Action(CommonMonster2 monster)
        {
            base.Action(monster);
            tweener = _cM.ActorMovement.DashTemp(time, distance, isBackDash);
            // _cM.rb.AddForce((int)_cM.direction * dashForce, ForceMode2D.Impulse);
            _cM.ExecuteEvent(EventType.OnDash, new EventParameters(_cM));
            if (isResisOff)
            {
                _cM.IsResist = false;
            }
        }

        public override void Update()
        {
            
        }
        
        public override void FixedUpdate()
        {
            
        }
        public override void OnCancel()
        {
            if (isResisOff)
            {
                _cM.IsResist = true;
            }
            tweener.Kill();
        }

        public override void OnEnd()
        {
            if (isResisOff)
            {
                _cM.IsResist = true;
            }
            tweener.Kill();
        }
    }
}