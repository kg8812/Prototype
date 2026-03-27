using System;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Apis
{
    // Projectile이 충돌했을때 타입
    public enum ProjectileConflictType
    {
        None, // 아무일도 없음
        Destroy, // 해당 투사체 파괴
        Reflect, // 닿은 면에 대해서 투사체 반사
        Penetrate, // 관통, 최대 횟수 및 데미지 감소량 필요할 때
        Stop, // 멈춤
    }
    public class Projectile : AttackObject , IDirection
    {
        #region Inspector
        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("중력 크기")]
        public float gravityScale;

        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("가속도")] [Tooltip("[unit/s^2]")]
        public float acceleration;

        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("최대 이동거리")] [Tooltip("parabolic 투사체에선 적용되지 않습니다.")]
        public float maxDistance;

        [FormerlySerializedAs("mapConflictType")] [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("벽 충돌타입")]
        public ProjectileConflictType wallConflictType;

        [TabGroup("InfoGroup/공격설정", "투사체 설정")] [LabelText("바닥 충돌타입")]
        public ProjectileConflictType groundConflictType;
        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("타겟 충돌타입")]
        public ProjectileConflictType targetConflictType;

        [TabGroup("InfoGroup/공격설정","투사체 설정")][LabelText("보스 충돌타입 사용여부")] public bool useBossConflict;
        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("보스 충돌타입")][ShowIf("useBossConflict")]
        public ProjectileConflictType bossConflictType;

        [ShowIf("targetConflictType",ProjectileConflictType.Penetrate)]
        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("관통 최대 횟수")]
        public int penetrationMax;
        [ShowIf("targetConflictType",ProjectileConflictType.Penetrate)]
        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("관통당 데미지 증가량")][Tooltip("감소 시 마이너스로 기입")]
        public int penetrationDmg;
        [ShowIf("targetConflictType",ProjectileConflictType.Penetrate)]
        [TabGroup("InfoGroup/공격설정", "투사체 설정")] [LabelText("관통당 크기 증가량(%)")] [Tooltip("감소 시 마이너스로 기입")]
        public float penetrationRadius;
        
        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("초기 속도")] [Tooltip("초기 방향이 달라진다면 발사 속력으로 간주")]
        public Vector2 firstVelocity;

        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("방향 = 속도vector 여부")] [SerializeField]
        private bool isRotateToVelocity = true;
        [ShowIf("isRotateToVelocity")]
        [TabGroup("InfoGroup/공격설정", "투사체 설정")] [LabelText("투사체가 바라볼 방향")] [SerializeField]
        private Dir projDirection = Dir.Right;
        [TabGroup("InfoGroup/공격설정","투사체 설정")] [LabelText("좌우 방향에 따른 y flip 여부")] [SerializeField]
        private bool isYFlipByDirection;

        [TabGroup("InfoGroup/공격설정", "투사체 설정")] [LabelText("속도 0일시 Destroy 여부")] [SerializeField]
        private bool destroyWhenZero;
        
        #endregion
        [HideInInspector] public Vector2 realInitVelocity;
        protected Transform ThisTrans;

        protected Vector2 perPos;
        protected float curDist;
        protected bool fired;
        public bool Fired => fired;
        protected bool isReturn;
        protected int penetrationCount;

        [HideInInspector] public Action<IOnHit> OnSetTarget;
        [HideInInspector] public Action OnFire;

        public enum Dir // 날아가는 방향을 바라볼 시, 앞으로 설정할 방향 (up이면 위쪽이 날아가는 방향을 바라봄)
        {
            Up = -90, Right = 0,Down = 90,Left = 180
        }
        protected virtual bool IsCheckMaxDist => true;

        protected override RigidbodyType2D BodyType => RigidbodyType2D.Dynamic;

        public enum Extensions
        {
            [LabelText("타겟이 투사체에 붙음")] StickTargets,
            [LabelText("유도기능")] Guide,
            [LabelText("파괴 시 방사체 생성")] Radial,
            [LabelText("점점 커지거나 작아짐")] Wave,
            [LabelText("파괴 시 장판 생성")] CreatePlate,
        }

        private List<ProjectileExtension> extensions;
        [Button("기능 추가",30)]
        public void AddExtension(Extensions type)
        {
            switch (type)
            {
                case Extensions.StickTargets:
                    gameObject.GetOrAddComponent<StickTargets>(false);
                    break;
                case Extensions.Guide:
                    gameObject.GetOrAddComponent<GuideExtension>(false);
                    break;
                case Extensions.Radial:
                    gameObject.GetOrAddComponent<RadialFunction>(false);
                    break;
                case Extensions.Wave:
                    gameObject.GetOrAddComponent<SoundWaveExtension>(false);
                    break;
                case Extensions.CreatePlate:
                    gameObject.GetOrAddComponent<CreatePlateExtension>(false);
                    break;
                default:
                    return;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            ThisTrans = transform;
            isDestroyed = false;
            extensions = GetComponents<ProjectileExtension>().ToList();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            isDestroyed = false;
        }

        public override void Init(ProjectileInfo info)
        {
            base.Init(info);

            if (info == null) return;
            
            gravityScale = info.gravityScale;
            acceleration = info.acceleration;
            maxDistance = info.maxDistance;
            wallConflictType = info.wallConflictType;
            groundConflictType = info.groundConflictType;
            targetConflictType = info.targetConflictType;
            penetrationMax = info.penetrationMax;
            penetrationDmg = info.penetrationDmg;
            penetrationRadius = info.penetrationRadius;
            firstVelocity = info.firstVelocity;
            isYFlipByDirection = info.isYFlipByDirection;
            isRotateToVelocity = info.isRotateToVelocity;
            destroyWhenZero = info.destroyWhenZero;
            projDirection = info.projDirection;
            useBossConflict = info.useBossConflict;
            bossConflictType = info.bossConflictType;
            extensions?.ForEach(x =>
            {
                x.Init(info);
            });
        }
        
        public override void Init(AttackObjectInfo atkObjectInfo)
        {
            base.Init(atkObjectInfo);
            curDist = 0;
            fired = false;
            rigid.linearVelocity = Vector2.zero;
            rigid.gravityScale = 0;
            perPos = ThisTrans.position;
            realInitVelocity = firstVelocity;
            Collider.enabled = false;

            isDestroyed = false;

        }

        protected Vector2 GetAccelerationVector()
        {
            Vector2 normalVel = rigid.linearVelocity.normalized;
            return normalVel * (acceleration * Time.fixedDeltaTime);
        }

        protected virtual void Update()
        {
            if (!fired && !isReturn) return;
            if (isRotateToVelocity)
            {
                float angle = Mathf.Atan2(rigid.linearVelocity.y, rigid.linearVelocity.x) * Mathf.Rad2Deg;
                ThisTrans.rotation = Quaternion.AngleAxis(angle + (int)projDirection,
                    Vector3.forward);
            }

            if (isYFlipByDirection)
            {
                Vector3 scale = ThisTrans.localScale;
                scale.y = Mathf.Abs(scale.y);

                ThisTrans.localScale = rigid.linearVelocity.x >= 0 ? scale : new Vector3(scale.x, -scale.y, scale.z);
            }
        }
        private bool isDestroyed;
        protected virtual void FixedUpdate()
        {
            if (!fired) return;
            
            SetDirection(rigid.linearVelocity.x > 0 ? EActorDirection.Right : EActorDirection.Left);
            rigid.linearVelocity += GetAccelerationVector();
            if (IsCheckMaxDist && !Mathf.Approximately(maxDistance,0))
            {
                var position = ThisTrans.position;
                float distanceThisFrame = Vector3.Distance(perPos, position);
                curDist += distanceThisFrame;
                perPos = position;
                
                if (curDist >= maxDistance)
                {
                    Destroy();
                    fired = false;
                }
            }

            if (destroyWhenZero)
            {
                if (Mathf.Approximately(Vector2.SqrMagnitude(rigid.linearVelocity), 0))
                {
                    Destroy();
                    fired = false;
                }
            }
        }

        private void FireInit()
        {
            fired = true;
            perPos = ThisTrans.position;
            curDist = 0;
            rigid.gravityScale = gravityScale;
            penetrationCount = 0;
        }
        
        
        private void SetFirstVelocity()
        {
            realInitVelocity = firstVelocity;
        }

        void SetVelocityToActorDirection()
        {
            rigid.linearVelocity = new Vector2(Mathf.Abs(rigid.linearVelocity.x) * (_direction != null ?(int)_direction.Direction : 1), rigid.linearVelocity.y);
        }

        /// <summary>
        /// 투사체가 특정 방향을 바라봅니다. 발사하기전에 호출해야 합니다.
        /// </summary>
        /// <param name="dir">방향벡터</param>
        public void LookAt(Vector2 dir)
        {
            firstVelocity = dir.normalized * firstVelocity.magnitude;
        }
        /// 투사체가 특정 타겟을 바라봅니다 , 발사하기전에 호출해야 합니다.
        public virtual void LookAtTarget(IOnHit target, bool setTarget = true)
        {
            Vector2 dir = target.Position - transform.position;
            dir.Normalize();
            firstVelocity = dir * firstVelocity.magnitude;
            
            OnSetTarget?.Invoke(target);
        }
        
        /// 투사체를 angle만큼 회전시킵니다. Fire(발사) 이후에 호출해야 작동합니다
        /// 방향 = 속도vector 여부를 키면 작동하지 않습니다.
        public void Rotate(float angle)
        {
            rigid.linearVelocity = Quaternion.AngleAxis(angle, Vector3.forward) * rigid.linearVelocity;
        }

        public void RotateBeforeFire(float angle)
        {
            transform.Rotate(new Vector3(0, 0, angle));
            float rad = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            firstVelocity = dir.normalized * firstVelocity.magnitude;
        }
        public virtual void Fire(bool rotateWithPlayerX = true)
        {
           
            SetFirstVelocity();
            FireInit();
            rigid.linearVelocity = realInitVelocity;
            if (rotateWithPlayerX)
            {
                SetVelocityToActorDirection();
            }
            
            Collider.enabled = true;
            OnFire?.Invoke();
        }

        protected override void AttackInvoke(EventParameters parameters)
        {
            base.AttackInvoke(parameters);

            if (useBossConflict && parameters?.target is BossMonster)
            {
                OnObjectConflicted(bossConflictType,targetLayer);
            }
            else
            {
                OnObjectConflicted(targetConflictType, targetLayer);
            }
        }
        
        private const float reflectionCheckDist = 1f;

        protected virtual void OnObjectConflicted(ProjectileConflictType conflictType, LayerMask layerMasks)
        {
            switch (conflictType)
            {
                case ProjectileConflictType.Destroy:
                    Destroy();
                    break;
                case ProjectileConflictType.Reflect:
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, rigid.linearVelocity, reflectionCheckDist,
                        layerMasks);
                    if (hit.collider != null)
                    {
                        Vector2 normal = hit.normal;
                        float dotProduct = Vector2.Dot(rigid.linearVelocity, normal);
                        rigid.linearVelocity -= 2f * dotProduct * normal;
                    }
                    break;
                case ProjectileConflictType.Penetrate:
                    if (penetrationCount >= penetrationMax)
                    {
                        Destroy();
                        return;
                    }
                    penetrationCount++;
                    _atkStrategy.DmgRatio += penetrationDmg;
                    transform.localScale *= (1 + penetrationRadius / 100f);
                    break;
                case ProjectileConflictType.Stop:
                    rigid.linearVelocity = Vector2.zero;
                    break;
            }
        }


        public override void OnTriggerEnter2D(Collider2D col)
        {
            base.OnTriggerEnter2D(col);
            if (col.gameObject.layer == Layers.Wall)
            {
                OnObjectConflicted(wallConflictType, LayerMasks.Wall);
            }
            else if (col.gameObject.layer == Layers.Ground)
            {
                OnObjectConflicted(groundConflictType, LayerMasks.Ground);
            }
        }

        public override void Destroy()
        {
            if (isDestroyed) return;
            base.Destroy();
            rigid.linearVelocity = Vector2.zero;
            //transform.localPosition = Vector2.zero;
            isDestroyed = true;
            extensions.ForEach(x =>
            {
                x?.Destroy();
            });
        }
        
        public EActorDirection Direction { get; set; }

        public void SetDirection(EActorDirection dir)
        {
            Direction = dir;
        }

        public int DirectionScale => (int)Direction;
    }
}