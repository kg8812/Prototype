using Spine;
using Spine.Unity;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New MAS_FollowingPlayer", menuName = "Scriptable/Monster/Attack/MAS_FollowingPlayer")]
    [System.Serializable]
    public class MAS_FollowingPlayer: MonsterAction
    {
        public string boneName = "target";
        [Tooltip("부채꼴의 각도")] public float maxAngle = 60f;
        [Tooltip("유지 시간")] public float time = 2f;
        public string animTriggerNameWhenFollowed;
        
        
        
        private Bone bone;
        private CommonMonster2 monster;
        private Vector2 playerPos;

        private bool activated = false;


        private float _startTime;

        public override void Action(CommonMonster2 monster)
        {
            base.Action(monster);
            this.monster = monster;
            bone = monster.Mecanim.Skeleton.FindBone(boneName);
            _startTime = Time.time;
            activated = true;
        }

        public override void Update()
        {
            if (!activated) return;
            if (!ReferenceEquals(bone, null))
            {
                if (!ReferenceEquals(GameManager.instance.ControllingEntity, null))
                {
                    playerPos = GameManager.instance.ControllingEntity.Position;
                    CheckRotation(playerPos);
                    Vector3 skeletonSpacePoint = monster.Mecanim.transform.InverseTransformPoint(playerPos);
                    skeletonSpacePoint.x *= monster.Mecanim.Skeleton.ScaleX;
                    skeletonSpacePoint.y *= monster.Mecanim.Skeleton.ScaleY;
                    bone.SetLocalPosition(skeletonSpacePoint);
                }
            }

            if (Time.time >= _startTime + time)
            {
                activated = false;
                monster.animator.SetTrigger(animTriggerNameWhenFollowed);
            }
        }

        private void CheckRotation(Vector2 pos)
        {
            if (pos.x > monster.transform.position.x != (monster.Direction == EActorDirection.Right))
            {
                monster.TurnWithoutDelay();
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