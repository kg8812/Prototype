using Apis;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Apis
{
    public class BeamEffect : Projectile
    {
        private Transform child;

        [TabGroup("빔 설정")]
        [LabelText("발사 시간")] public float fireTime;
        [TabGroup("빔 설정")]
        [LabelText("사정거리")] public float distance;
        [TabGroup("빔 설정")]
        [LabelText("크기")] public float size;

        [TabGroup("빔 설정")] [LabelText("발사 Ease")]
        public Ease ease = Ease.Linear;

        [TabGroup("빔 설정")] [LabelText("발사 방향")]
        public FireDir fireDir = FireDir.Horizontal;

        [System.Serializable]
        public struct BeamInfo
        {
            public BeamInfo(float _fireTime, float _distance, float _size, Ease _ease, FireDir dir)
            {
                fireTime = _fireTime;
                distance = _distance;
                size = _size;
                ease = _ease;
                fireDir = dir;
            }

            public BeamInfo(BeamInfo other)
            {
                fireTime = other.fireTime;
                distance = other.distance;
                size = other.size;
                ease = other.ease;
                fireDir = other.fireDir;
            }
            [LabelText("발사 시간")] public float fireTime;
            [LabelText("사정거리")] public float distance;
            [LabelText("크기")] public float size;
            [LabelText("발사 Ease")] public Ease ease;
            [LabelText("발사 방향")] public FireDir fireDir;
        }

        private Vector2 pos;

        private bool isFire;
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isFire)
            {
                transform.localPosition = pos;
            }
        }

        public enum FireDir
        {
            Horizontal,Vertical
        }
        public override void Destroy()
        {
            base.Destroy();
            child.DOKill();
            isFire = false;
        }

        public void Init(BeamInfo info)
        {
            fireTime = info.fireTime;
            distance = info.distance;
            size = info.size;
            ease = info.ease;
            fireDir = info.fireDir;
        }

        public override void Init(AttackObjectInfo atkObjectInfo)
        {
            base.Init(atkObjectInfo);
            pos = Vector2.zero;
            isFire = false;
        }

        public override void Fire(bool rotateWithPlayerX = true)
        {
            Collider.enabled = true;
            child = transform.GetChild(0);
            pos = transform.localPosition;
            child.localPosition = Vector2.zero;
            isFire = true;

            switch (fireDir)
            {
                case FireDir.Horizontal:
                    child.localScale = new Vector2(0, size);
                    child.DOLocalMoveX(distance / 2f * (_direction != null ?(int)_direction.Direction : 1), fireTime).SetEase(ease);
                    child.DOScaleX(distance * (_direction != null ?(int)_direction.Direction : 1), fireTime).SetEase(ease);
                    break;
                case FireDir.Vertical:
                    child.localScale = new Vector2(size, 0);
                    child.DOLocalMoveY(distance / 2f, fireTime).SetEase(ease);
                    child.DOScaleY(distance , fireTime).SetEase(ease);
                    break;
            }
        }

        
        public override void LookAtTarget(IOnHit target, bool setTarget = true)
        {
            base.LookAtTarget(target, setTarget);
            Vector2 dir = target.Position - transform.position;
            dir.Normalize();
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (target.Position.x < transform.position.x)
            {
                angle += 180;
            }
            
            ThisTrans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}