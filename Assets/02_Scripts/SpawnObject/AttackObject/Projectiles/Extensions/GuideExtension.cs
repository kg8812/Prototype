using Default;
using UnityEngine;

namespace Apis
{
    public enum GuideTargetType
    {
        FoundUntilFirst,
        FoundNearestAlways,
        FoundUntilFirstInRange
    }
    public class GuideExtension : ProjectileExtension
    {
        [HideInInspector] public float followPower;
        float followRange;
        GuideTargetType guideTargetType = GuideTargetType.FoundUntilFirst;
        float targetFoundAngle = 0;
        
        public float FollowRange
        {
            get => followRange;
            set
            {
                followRange = value;
                followRangeSqr = value * value;
            }
        }

        protected IOnHit target;

        protected Vector2 TargetPos => target.Position;

        private const int MaxTargetSearch = 15;
        private float _tempMaxDist, _tempDist, _angle;
        private Collider2D[] _tempCols = new Collider2D[MaxTargetSearch];
        private int _foundedCols, _i, _exceptedCols, _targetInd;
        private Vector2 _position, _dir;
        private Vector2 _followVec, _curVec, _toVec;

        private float followRangeSqr;

        private void Awake()
        {
            projectile.OnSetTarget += LookAtTarget;
        }

        public override void Init(ProjectileInfo info)
        {
            base.Init(info);
            
            if (info == null) return;

            target = null;

            followPower = info.followPower;
            followRange = info.followRange;
            guideTargetType = info.guideTargetType;
            targetFoundAngle = info.targetFoundAngle;
        }

        public void LookAtTarget(IOnHit _target)
        {
            if (_target is { IsDead: false })
            {
                target = _target;
            }
        }

        private void FollowTarget()
        {
            if (ReferenceEquals(target, null)) return;
            _followVec = TargetPos - (Vector2)projectile.transform.position;
            _curVec = projectile.rigid.velocity;
            _toVec =
                Vector2.Lerp(_curVec.normalized, _followVec.normalized, followPower * Time.fixedDeltaTime).normalized *
                projectile.rigid.velocity.magnitude;

            projectile.rigid.velocity = _toVec;
        }

        private void FindTarget()
        {
            _foundedCols = Physics2D.OverlapCircleNonAlloc(projectile.transform.position, followRange, _tempCols, projectile.targetLayer);
            if (_foundedCols > 0)
            {
                // velocity가 0이라면 각도 판별 못함
                if (targetFoundAngle != 0 && projectile.rigid.velocity != Vector2.zero)
                {
                    _exceptedCols = 0;
                    // 각도에 안맞는 애들은 제외
                    for (_i = 0; _i < _foundedCols; _i++)
                    {
                        _position = _tempCols[_i].offset + (Vector2)_tempCols[_i].transform.position;
                        _dir = _position - (Vector2)projectile.transform.position;

                        if (!Utils.CheckAngle(projectile.rigid.velocity, _dir, targetFoundAngle))
                        {
                            _exceptedCols++;
                            _tempCols[_i] = null;
                        }
                    }

                    // 원뿔 내 개체가 존재하지 않음
                    if (_exceptedCols == _foundedCols)
                    {
                        target = null;
                        return;
                    }
                }

                // 가장 가까운 대상 탐색
                _tempMaxDist = 10000;
                for (_i = 0; _i < _foundedCols; _i++)
                {
                    if (ReferenceEquals(_tempCols[_i], null)) continue;
                    _position = _tempCols[_i].offset + (Vector2)_tempCols[_i].transform.position;
                    _dir = _position - (Vector2)projectile.transform.position;
                    _tempDist = _dir.sqrMagnitude;
                    if (_tempDist < _tempMaxDist)
                    {
                        _targetInd = _i;
                        _tempMaxDist = _tempDist;
                    }
                }

                target = _tempCols[_targetInd].gameObject.GetComponent<IOnHit>();
                if (target is { IsDead: true })
                {
                    target = null;
                }
            }
            else
            {
                target = null;
            }
        }

        protected bool CheckTargetInRange()
        {
            _dir = TargetPos - (Vector2)projectile.transform.position;
            return followRangeSqr >= _dir.sqrMagnitude && Utils.CheckAngle(projectile.rigid.velocity, _dir, targetFoundAngle);
        }

        void FixedUpdate()
        {
            if (!projectile.Fired) return;
            if (target is { IsDead : true}) target = null;
            
            switch (guideTargetType)
            {
                case GuideTargetType.FoundUntilFirst:
                    if (!ReferenceEquals(target, null))
                    {
                        FollowTarget();
                    }
                    else
                    {
                        FindTarget();
                    }

                    break;

                case GuideTargetType.FoundNearestAlways:
                    FindTarget();
                    FollowTarget();
                    break;

                case GuideTargetType.FoundUntilFirstInRange:
                    if (!ReferenceEquals(target, null))
                    {
                        if (CheckTargetInRange())
                        {
                            FollowTarget();
                        }
                        else
                        {
                            target = null;
                        }
                    }
                    else
                    {
                        FindTarget();
                    }

                    break;
            }
        }
    }
}