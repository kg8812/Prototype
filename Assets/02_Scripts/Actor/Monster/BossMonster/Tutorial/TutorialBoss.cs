using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Apis
{
    public class TutorialBoss : BossMonster
    {
        [TabGroup("기획쪽 수정 변수들/group1", "패턴 관련")]
        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")]
        [LabelText("데미지")] public float dmg;

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2")]

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("데미지")] public float dmg2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("점프 높이")] public float height2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("점프 시간")] public float duration2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("점프 근접 거리")] public float meleeDistance2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("점프 최소 거리")] public float minDistance2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴2/패턴", "1")] [LabelText("점프 최대 거리")] public float maxDistance2;
        
        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "1")] [LabelText("투사체 정보")] public ProjectileInfo projInfo3;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "1")] [LabelText("투사체 개수")] public int count3;
        
        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("내려찍기 데미지")] public float dmg4;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("1단점프 높이")] public float height4_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("1단점프 시간")] public float duration4_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("1단점프 근접 거리")] public float meleeDistance4_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("1단점프 최소 거리")] public float minDistance4_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("1단점프 최대 거리")] public float maxDistance4_1; 
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("2단점프 높이")] public float height4_2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("2단점프 시간")] public float duration4_2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("2단점프 근접 거리")] public float meleeDistance4_2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("2단점프 최소 거리")] public float minDistance4_2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "1")] [LabelText("2단점프 최대 거리")] public float maxDistance4_2;

        protected override void Awake()
        {
            base.Awake();
            SetState(BossState.Move);
        }

        protected override void Update()
        {
            base.Update();
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

        protected override void WhenRecognized()
        {
            animator.SetTrigger("Recognized");
        }
        
        class Attack1 : IBossAttackPattern
        {
            private TutorialBoss _boss;
            public Attack1(TutorialBoss boss)
            {
                _boss = boss;
            }
            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
            }

            public void SetCollider(int index)
            {
                float dmg = _boss.dmg;
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
        
        class Attack2 : IBossAttackPattern
        {
            private TutorialBoss _boss;
            
            public Attack2(TutorialBoss boss)
            {
                _boss = boss;
            }
            public void OnPatternEnter()
            {
                _boss.Rb.DOKill();
                _boss.ActorMovement.SetGravityToZero();
            }

            public void Attack(int index)
            {
                var tweens = _boss.JumpToPlayer(_boss.meleeDistance2, _boss.minDistance2, _boss.maxDistance2, _boss.height2,
                    _boss.duration2);
                    tweens.Item2.onKill += () =>
                {
                    _boss.animator.SetTrigger("AttackEnd");
                };
            }

            public void SetCollider(int index)
            {
                float dmg = _boss.dmg2;
                _boss.colliderList.ForEach(x =>
                {
                    x.Init(_boss, new AtkBase(_boss, dmg));
                });
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
        class Attack3 : IBossAttackPattern
        {
            private TutorialBoss _boss;
            public Attack3(TutorialBoss boss)
            {
                _boss = boss;
            }
            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                var projectiles = GameManager.Factory.SpawnProjectilesInCircle(Define.BossObjects.TutorialBossProjectile, _boss.Position,
                    _boss.count3, 1);

                foreach (var projectile in projectiles)
                {
                    projectile.Init(_boss, new AtkBase(_boss, _boss.projInfo3.dmg));
                    projectile.Init(_boss.projInfo3);
                    projectile.LookAt(projectile.Position - _boss.Position);
                    projectile.Fire(false);
                }
            }

            public void SetCollider(int index)
            {
            }

            public void End()
            {
            }

            public void Cancel()
            {
            }
        }
        class Attack4 : IBossAttackPattern
        {
            private TutorialBoss _boss;
            private (Tween,Tween) jumpTween;
            public Attack4(TutorialBoss boss)
            {
                _boss = boss;
            }
            public void OnPatternEnter()
            {
                _boss.Rb.DOKill();
                _boss.ActorMovement.SetGravityToZero();
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        Jump1();
                        break;
                    case 2:
                        Jump2();
                        break;
                }
            }

            void Jump1()
            {
                jumpTween.Item1?.Kill();
                jumpTween.Item2?.Kill();
                _boss.animator.ResetTrigger("AttackEnd");
                jumpTween = _boss.JumpToPlayer(_boss.meleeDistance4_1,_boss.minDistance4_1,_boss.maxDistance4_1,0.5f,_boss.height4_1,_boss.duration4_1 );
                jumpTween.Item2.onKill += () =>
                {
                    _boss.animator.SetTrigger("AttackEnd");
                };
            }

            void Jump2()
            {
                _boss.ActorMovement.TryGetFloorPos(out var floorPos);
                float y = floorPos.y - _boss.transform.position.y;
                jumpTween.Item1?.Kill();
                jumpTween.Item2?.Kill();
                _boss.animator.ResetTrigger("AttackEnd");
                jumpTween = _boss.JumpToPlayer(_boss.meleeDistance4_2,_boss.minDistance4_2,_boss.maxDistance4_2,_boss.height4_2,y,_boss.duration4_2 );
                jumpTween.Item2.onKill += () =>
                {
                    _boss.animator.SetTrigger("AttackEnd");
                };
            }
            public void SetCollider(int index)
            {
                float dmg = _boss.dmg4;
                _boss.colliderList.ForEach(x =>
                {
                    x.Init(_boss, new AtkBase(_boss, dmg));
                });
            }

            public void End()
            {
                jumpTween.Item1?.Kill();
                jumpTween.Item2?.Kill();
                _boss.ActorMovement.ResetGravity();
            }

            public void Cancel()
            {
                jumpTween.Item1?.Kill();
                jumpTween.Item2?.Kill();
                _boss.ActorMovement.ResetGravity();
            }
        }
    }
}