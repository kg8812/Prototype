using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using WaitForSeconds = UnityEngine.WaitForSeconds;

namespace Apis
{
    public class GiantSpiritBoss : BossMonster
    {
        #region 공격 패턴
        public class Attack1 : IBossAttackPattern
        {
            private GiantSpiritBoss _boss;
            public Attack1(GiantSpiritBoss boss)
            {
                _boss = boss;
            }
            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                Tween tween = _boss.Rb.DOMoveX(_boss.moveDistance1 * _boss.DirectionScale, _boss.moveSpeed1).SetRelative().SetEase(_boss.moveEase1);
                tween.KillWhenBoxCast(_boss, 0.5f, Vector2.right * _boss.DirectionScale, new Vector2(0.2f, 1), LayerMasks.Wall);
            }

            public void SetCollider(int index)
            {
                float dmg = _boss.dmg1;
                _boss.colliderList.ForEach(x =>
                {
                    x.Init(_boss, new AtkBase(_boss, dmg));
                });
            }

            public void End()
            {
            }

            public void Cancel()
            {
            }
        }

        public class Attack2 : IBossAttackPattern
        {
            private GiantSpiritBoss _boss;
            private Coroutine _atkCoroutine;
            public Attack2(GiantSpiritBoss boss)
            {
                _boss = boss;
            }
            public void OnPatternEnter()
            {
                _boss.animator.ResetTrigger("AttackEnd");
                CancelCoroutine();
            }

            public void Attack(int index)
            {
                _atkCoroutine = GameManager.instance.StartCoroutineWrapper(AtkCoroutine());
            }

            IEnumerator AtkCoroutine()
            {
                for (int i = 0; i < _boss.projCount2; i++)
                {
                    Projectile proj = GameManager.Factory.Get<Projectile>(FactoryManager.FactoryType.Normal,
                        Define.BossEffects.GiantSpiritProjectile, _boss.Position);
                    proj.Init(_boss,new AtkBase(_boss,_boss.projInfo2.dmg),5);
                    proj.Init(_boss.projInfo2);
                    proj.SetRadius(_boss.projRadius2);
                    Vector2 vel = new Vector2(Random.Range(_boss.fireVelocityX2.x, _boss.fireVelocityX2.y),
                        Random.Range(_boss.fireVelocityY2.x, _boss.fireVelocityY2.y));
                    proj.firstVelocity = vel;
                    proj.Fire(false);
                    yield return new WaitForSeconds(_boss.fireTerm2);
                }
                
                _boss.animator.SetTrigger("AttackEnd");
            }
            public void SetCollider(int index)
            {
            }

            public void End()
            {
                CancelCoroutine();
            }

            public void Cancel()
            {
                CancelCoroutine();
            }

            void CancelCoroutine()
            {
                if (_atkCoroutine != null)
                {
                    GameManager.instance.StopCoroutineWrapper(_atkCoroutine);
                }
                _atkCoroutine = null;
            }
        }
        public class Attack3 : IBossAttackPattern
        {
            private GiantSpiritBoss _boss;
            private Tween tween;
            
            public Attack3(GiantSpiritBoss boss)
            {
                _boss = boss;
            }
            public void OnPatternEnter()
            {
                _boss.Rb.DOKill();
                _boss.ActorMovement.SetGravityToZero();
                _boss.FlipToPlayer();
            }

            void Jump()
            {
                Vector2 endPos = new Vector2(GameManager.instance.ControllingEntity.Position.x, _boss.Position.y + _boss.height3);
                tween = _boss.Rb.DOJump(endPos, _boss.height3, 1, _boss.duration3);
                tween.onComplete += () =>
                {
                    _boss.animator.SetTrigger("AttackEnd");
                };
            }

            void Stamp()
            {
                RaycastHit2D hit = Physics2D.Raycast( _boss.Position, Vector2.down, Mathf.Infinity, LayerMasks.GroundAndPlatform);
                if (hit.collider != null)
                {
                    tween = _boss.Rb.DOMoveY(hit.point.y, _boss.stampSpeed3).SetUpdate(UpdateType.Fixed).SetSpeedBased().SetAutoKill(true).SetEase(Ease.Linear);
                   
                    tween.onComplete += () =>
                    {
                        AttackObject obj = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.Effect,
                            "GiantSpiritEffect", _boss.transform.position);
                        obj.Init(_boss,new AtkBase(_boss,_boss.dmg3),1);
                        obj.Init(AtkInfo);
                    };
                    tween.onComplete += () =>
                    {
                        _boss.animator.SetTrigger("AttackEnd");
                    };
                }
                else
                {
                    Cancel();
                }
            }
            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        Jump(); break;
                    case 2:
                        Stamp();
                        break;
                }
            }

            public void SetCollider(int index)
            {
            }

            public void End()
            {
                _boss.ActorMovement.ResetGravity();
            }

            public void Cancel()
            {
                _boss.ActorMovement.ResetGravity();
            }
        }
        public class Attack4 : IBossAttackPattern
        {
            private GiantSpiritBoss _boss;
            private BlackHole _blackHole;
            
            public Attack4(GiantSpiritBoss boss)
            {
                _boss = boss;
            }
            public void OnPatternEnter()
            {
                _boss.Rb.DOKill();
            }

            void SpawnBlackHole()
            {
                _blackHole = GameManager.Factory.Get<BlackHole>(FactoryManager.FactoryType.Normal,
                    Define.BossEffects.GiantSpiritBlackHole, _boss.Position);
                Vector2 offset = new Vector2(0, _boss.transform.position.y - _boss.Position.y);
                _blackHole.targetLayer = LayerMasks.Player;
                _blackHole.pullForce = _boss.grabForce4;
                _blackHole.maxSpeed = _boss.grabSpeed4;
                _blackHole.offset = offset;
                _blackHole.SetRadius(_boss.grabArea4);
            }

            void Explode()
            {
                RemoveBlackHole();
                var exp = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.Effect,
                    Define.DummyEffects.Explosion, _boss.Position);
                exp.Init(_boss, new AtkBase(_boss, _boss.dmg4), 1);
                exp.Init(AtkInfo);
                exp.SetRadius(_boss.radius4);
            }
            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        SpawnBlackHole();
                        break;
                    case 2:
                        Explode();
                        break;
                }
            }

            void RemoveBlackHole()
            {
                if (_blackHole != null)
                {
                    GameManager.Factory.Return(_blackHole.gameObject);
                    _blackHole = null;
                }
            }
            void OnCancel()
            {
                RemoveBlackHole();
            }
            public void SetCollider(int index)
            {
            }

            public void End()
            {
                OnCancel();
            }

            public void Cancel()
            {
                OnCancel();
            }
        }

        #endregion
        
        [TabGroup("기획쪽 수정 변수들/group1", "패턴 관련")]
        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [LabelText("데미지")] public float dmg1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [LabelText("이동거리")] public float moveDistance1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [LabelText("이동속도")] public float moveSpeed1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [LabelText("이동Ease")] public Ease moveEase1;
        
        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("투사체 설정")] public ProjectileInfo projInfo2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("투사체 반경")] public float projRadius2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("발사 개수")] public int projCount2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("발사 텀")] public float fireTerm2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("최소,최대 X 발사 힘")]
        [InfoBox("-부터 입력해주셔야 왼쪽으로도 날라갑니다. 예시) -5,5")]
        public Vector2 fireVelocityX2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("최소,최대 Y 발사 힘")][Tooltip("Y쪽으로 발사하는 힘입니다. 입력된 힘으로 발사됩니다.")] 
        public Vector2 fireVelocityY2;

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "1")] [LabelText("점프 높이")] public float height3;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "1")] [LabelText("점프 시간")] public float duration3;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "1")] [LabelText("찍기 속도")] public float stampSpeed3;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "1")] [LabelText("찍기 데미지")] public float dmg3;
        
        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("데미지")] public float dmg4;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("그랩 반경")] public float grabArea4;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("그랩 최대속도")] public float grabForce4;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("당기는 힘")] public float grabSpeed4;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("공격 반경")] public float radius4;

        protected override void Awake()
        {
            base.Awake();
        }
        
        protected override void SetAttackPatterns()
        {
            attackPatterns = new()
            {
                { 1, new Attack1(this) },
                { 2, new Attack2(this) },
                { 3, new Attack3(this) },
                { 4, new Attack4(this) },
            };
        }
    }
}