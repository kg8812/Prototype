using DG.Tweening;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New MoveToPlayer", menuName = "Scriptable/Monster/Movement/MoveToPlayer")]
    [System.Serializable]
    public class MMS_MoveToPlayer: MonsterAction
    {
        public Vector2 playerPosOffset;
        public float time;
        
        public Ease moveEase = Ease.OutCubic; // DOTween의 Ease 객체 사용
        
        
        private float elapsedTime = 0f; // 경과 시간
        private float duration; // 이동 시간
        private bool isMoving = false;

        
        public override void Action(CommonMonster2 monster)
        {
            base.Action(monster);
            duration = time;
            elapsedTime = 0f;
            isMoving = true;
        }
        
        
        public override void Update()
        {
            if (!isMoving) return;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // 진행 비율 (0~1)

            // DOTween의 Ease.Evaluate로 보간 비율 계산
            float easedT = DOVirtual.EasedValue(0f, 1f, t, moveEase);

            // 현재 위치를 기준으로 목표 위치로 이동
            Vector2 currentPos = _cM.Position;
            Vector2 newPos = Vector2.Lerp(currentPos, (Vector2)GameManager.instance.ControllingEntity.Position + playerPosOffset, easedT);

            _cM.Position = newPos;

            // 이동 완료 처리
            if (t >= 1f)
            {
                isMoving = false;
            }
        }

        public override void FixedUpdate()
        {
        }

        public override void OnCancel()
        {
            isMoving = false;
        }
    }
}