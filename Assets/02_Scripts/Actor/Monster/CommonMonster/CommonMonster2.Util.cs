using Apis.Managers;
using UnityEngine;

namespace Apis.CommonMonster2
{
    public partial class CommonMonster2
    {
        [SerializeField] private bool isHitAnim;

        private readonly int _hitTriggerProperty = Animator.StringToHash("hit");


        protected override void OnHitReaction(EventParameters eventParameters)
        {
            if(isHitAnim)
                animator.SetTrigger(_hitTriggerProperty);
            base.OnHitReaction(eventParameters);
        }

        private bool CheckWallAndCliff()
        {
            return ActorMovement.CheckWall2() || ActorMovement.CheckCliff();
        }
        
        public bool IsPlayerRight()
        {
            if (!ReferenceEquals(_playerTrans, null))
            {
                return _playerTrans.position.x > transform.position.x;
            }
            return false;
            // 플레이어가 없음. 그냥 왼쪽이라고 대충 알려주기
        }
        
        /// <returns>플레이어 바라보고 있으면 true, 아니면 else</returns>
        public bool CheckPlayerRl()
        {
            EActorDirection playerRL = IsPlayerRight() ? EActorDirection.Right : EActorDirection.Left;
            return Direction == playerRL;
        }
        
        public void TurnToPlayerWithoutDelay()
        {
            if(!CheckPlayerRl() && ableMove)
                TurnWithoutDelay();
        }
        
        
        /// <summary>
        /// 자신(몬스터)와 플레이어 간의 거리를 반환하는 함수입니다.
        /// </summary>
        /// <returns>-1 = 벽에 부딪히거나 오류</returns>
        public float ShotRayToPlayer()
        {
            Vector3 myRayPoint = Position;
            Vector2 dir = GameManager.instance.ControllingEntity.Position - myRayPoint;
            RaycastHit2D hit = Physics2D.Raycast(myRayPoint, dir, Mathf.Infinity, LayerMasks.Player | LayerMasks.GroundWall);
#if UNITY_EDITOR
            Debug.DrawRay(myRayPoint, dir,Color.yellow);
#endif
            if (hit && hit.collider.gameObject.CompareTag("Player"))
            {
                return hit.distance;
            }
            return -1;
        }
        
        
        private void SceneLoadBegin(SceneData _)
        {
            if (gameObject.activeSelf)
            {
                CloseHpBar();
            }
        }
    }
}