using System;
using UnityEngine;

namespace Apis.CommonMonster2
{
    public class SMDeath : ICommonMonsterState<CommonMonster2>
    {
        private readonly int _deadAnim = Animator.StringToHash("dead");
        private CommonMonster2 _cM;

        private Guid _invincibleGuid;

        public void OnEnter(CommonMonster2 monster)
        {
            _cM = monster;
            _cM.IsDead = true;
            _cM.PgController.ForceCancel();
            _cM.animator.SetTrigger(_deadAnim);
            _invincibleGuid = _cM.AddInvincibility();
            _cM.MoveComponent.Stop();
            _cM.Rb.bodyType = RigidbodyType2D.Static;
            _cM.Collider.enabled = false;
            _cM.HitCollider.enabled = false;

            for (var i = _cM.Projectiles.Count - 1; i >= 0; i--) _cM.Projectiles[i].Destroy();
            _cM.Projectiles.Clear();


            GameManager.instance.BattleStateClass.RemoveRecogMonster(_cM);
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void OnExit()
        {
            _cM.animator.ResetTrigger(_deadAnim);
            _cM.RemoveInvincibility(_invincibleGuid);
        }

        public void OnCancel()
        {
        }
    }
}