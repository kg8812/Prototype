using System;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New CheckPlayer", menuName = "Scriptable/Monster/Attack/checkPlayer")]
    [Serializable]
    public class MAS_CheckPlayer : MonsterAction
    {
        public float lx, rx;
        public float dy, uy;
        public string triggerName;
        public float maxTime = 4f;
        public string failTriggerName = "attackFail";
        public float moveRatio = 1.5f;
        private float _startTime;
        private bool isCheck;
        private bool lastMoveStop;


        private Vector2 monsterPos, playerPos;
        private Transform monsterTrans;

        private Transform playerTrans;

        public override void Action(CommonMonster2 monster)
        {
            base.Action(monster);
            lastMoveStop = false;
            _startTime = Time.time;
            isCheck = true;
            // 공격 개체 pool 만들고 연결
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
            if (!isCheck) return;
            if (!playerTrans || !monsterTrans) InitTrans(_cM);
            var isRight = _cM.Direction == EActorDirection.Right;

            monsterPos = monsterTrans.position;
            playerPos = playerTrans.position;
            if (monsterPos.x + (isRight ? lx : -rx) <= playerPos.x &&
                playerPos.x <= monsterPos.x + (isRight ? rx : -lx))
                // Debug.Log("update2");
                if (monsterPos.y + dy <= playerPos.y &&
                    playerPos.y <= monsterPos.y + uy)
                {
                    // Debug.Log("updat3e");
                    // 플레이어가 범위 내에 존재.
                    // Position이 아닌 position으로 하기 때문에 신장 차이로 인한 차이가 있을수 있지만 무시함.
                    _cM.PgController.PatternEnded();
                    _cM.animator.SetTrigger(triggerName);
                    return;
                }

            if (_cM.MonsterMove(false, moveRatio))
            {
                if (lastMoveStop)
                {
                    lastMoveStop = false;
                    _cM.animator.SetBool("moveStop", false);
                }
            }
            else
            {
                if (!lastMoveStop)
                {
                    lastMoveStop = true;
                    _cM.animator.SetBool("moveStop", true);
                }
            }

            if (Time.time > _startTime + maxTime)
            {
                isCheck = false;
                _cM.PgController.PatternEnded();
                _cM.animator.SetTrigger(failTriggerName);
            }
        }

        public override void OnCancel()
        {
            _cM.animator.SetBool("moveStop", false);
        }


        private void InitTrans(Monster monster)
        {
            playerTrans = GameManager.instance.PlayerTrans;
            monsterTrans = monster.transform;
        }
    }
}