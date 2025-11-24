using System;
using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMNone: ICommonMonsterState<CommonMonster2>
    {
        private CommonMonster2 _cM;
        private Guid _invincible;
        public void OnEnter(CommonMonster2 m)
        {
            // Debug.Log("smnone enter to animator false");
            _cM = m;
            _cM.Mecanim.enabled = false;
            _cM.animator.enabled = false;
            _invincible = _cM.AddInvincibility();
        }

        public void Update()
        {
            _cM.CheckRecognition();
            if (_cM.IsRecognized)
            {
                _cM.TryChangeMonsterState(MonsterState.Move);
            }else if (_cM.IsActivated)
            {
                _cM.TryChangeMonsterState(MonsterState.Idle);
            }
        }

        public void FixedUpdate()
        {
        }

        public void OnExit()
        {
            // Debug.Log("smnone exit to animator true");
            _cM.Mecanim.enabled = true;
            _cM.animator.enabled = true;
            _cM.RemoveInvincibility(_invincible);
            _cM.Rb.bodyType = RigidbodyType2D.Dynamic;
            _cM.Collider.enabled = true;
            _cM.HitCollider.enabled = true;
        }

        public void OnCancel()
        {
        }
    }
}