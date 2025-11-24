using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Default;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Apis
{
    public partial class JururuBoss
    {
        public class AttackPattern1 : IBossAttackPattern
        {
            private readonly JururuBoss _boss;

            public AttackPattern1(JururuBoss boss)
            {
                _boss = boss;
            }

            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                _boss.MoveToPlayer(_boss.meleeMinMove1,_boss.minMove1,_boss.maxMove1,_boss.moveTime1,_boss.moveEase1);
            }

            public void SetCollider(int index)
            {
                _boss.colliderList.ForEach(x =>
                {
                    x.Init(_boss, new AtkBase(_boss, _boss.dmgRatio1));
                    x.hitEffect = Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack1Hit);
                    if (_boss.phase == BossPhase.Phase2)
                    {
                        x.AddEventUntilInitOrDestroy(info =>
                        {
                            if (info?.target is Actor t)
                            {
                                t.AddSubBuff(_boss, SubBuffType.Debuff_Burn);
                            }
                        });
                    }
                });
            }

            public void End()
            {
            }

            public void Cancel()
            {
            }
        }

        public class AttackPattern1_1 : IBossAttackPattern
        {
            private JururuBoss _boss;

            private Tween _tween;
            public AttackPattern1_1(JururuBoss boss)
            {
                _boss = boss;
            }
            void FirstMove()
            {
                //_boss.FlipToPlayer();
                _boss.Rb.DOKill();
                _tween = _boss.Rb.DOMoveX(_boss.firstMoveDistance1_1 * _boss.DirectionScale, _boss.firstMoveTime1_1).SetRelative().SetEase(_boss.firstMoveEase1_1);
            }
            
            void SecondMove()
            {
                //_boss.FlipToPlayer();
                _tween = _boss.MoveToPlayer(_boss.secondMeleeMinMove1_1,_boss.secondMinMove1_1,_boss.secondMaxMove1_1,_boss.secondMoveTime1_1,_boss.secondMoveEase1_1);
            }

            void ThirdMove()
            {
                //_boss.FlipToPlayer();
                _tween = _boss.MoveToPlayer(_boss.thirdMeleeMinMove1_1,_boss.thirdMinMove1_1,_boss.thirdMaxMove1_1,_boss.thirdMoveTime1_1,_boss.thirdMoveEase1_1);
            }

            void FourthMove()
            {
                //_boss.FlipToPlayer();
                _tween = _boss.MoveToPlayer(_boss.fourthMeleeMinMove1_1,_boss.fourthMinMove1_1,_boss.fourthMaxMove1_1,_boss.fourthMoveTime1_1,_boss.fourthMoveEase1_1);
            }

            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        FirstMove();
                        break;
                    case 2:
                        SecondMove();
                        break;
                    case 3:
                        ThirdMove();
                        break;
                    case 4:
                        FourthMove();
                        break;
                }
            }

            public void SetCollider(int index)
            {
                float dmg = index == 1 ? _boss.firstDmg1_1 : _boss.secondDmg1_1;
                _boss.colliderList.ForEach(x =>
                {
                    x.Init(_boss,new AtkBase(_boss,dmg));
                    if (_boss.phase == BossPhase.Phase2)
                    {
                        x.AddEventUntilInitOrDestroy(info =>
                        {
                            if (info?.target is Actor t)
                            {
                                t.AddSubBuff(_boss, SubBuffType.Debuff_Burn);
                            }
                        });
                    }
                });
            }

            public void End()
            {
            }

            public void Cancel()
            {
                _tween?.Kill();
                _tween = null;
            }
        }

        public class AttackPattern2 : IBossAttackPattern
        {
            private JururuBoss _boss;

            public AttackPattern2(JururuBoss boss)
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
            }

            public void End()
            {
            }

            public void Cancel()
            {
                _boss.GetComponentInChildren<JururuAttack2>(true).gameObject.SetActive(false);
            }
        }

        public class AttackPattern3 : IBossAttackPattern
        {
            private JururuBoss _boss;

            public AttackPattern3(JururuBoss boss)
            {
                _boss = boss;
            }

            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                for (int i = 0; i < _boss.projectiles.Count; i++)
                {
                    Vector2 spawnPos = _boss.transform.position -
                                       new Vector3((int)_boss.Direction * _boss.spawnPosX, -_boss.spawnPosY);
                    JururuFlameBall flameBall = _boss.SpawnProjectile(spawnPos + i * _boss.padding * Vector2.up);
                    flameBall.Init(_boss, new AtkBase(_boss, _boss.dmgRatio3));
                    flameBall.Init(_boss.projectileInfo);
                    flameBall.firstVelocity = _boss.projectiles[i];
                    flameBall.Fire();
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

        public class AttackPattern4 : IBossAttackPattern
        {
            private JururuBoss _boss;

            private Vector3 midPos;
            private Coroutine coroutine;
            
            public AttackPattern4(JururuBoss boss)
            {
                _boss = boss;
            }

            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                midPos = GameObject.Find("Attack4Pos").transform.position;
                coroutine = _boss.StartCoroutine(Attack4Coroutine());
            }

            public void SetCollider(int index)
            {
            }

            void SpawnFlame(Vector3 spawnPos)
            {
                var temp = _boss.EffectSpawner.Spawn(
                    Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack4Ready),
                    spawnPos,false);
                temp.transform.SetParent(_boss.BossAttacks.transform);
                
                GameManager.Factory.Return(temp.gameObject, _boss.ready4, () =>
                {
                    var appear = _boss.EffectSpawner.Spawn(
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack4Appear), spawnPos,false);
                    AttackObject atk = appear.GetComponent<AttackObject>();
                    atk.Init(_boss,new AtkBase(_boss,atk.projectileInfo.dmg));
                    AfterEffectSpawner spawner = appear.GetComponent<AfterEffectSpawner>();
                    spawner.OnSpawn += loop =>
                    {
                        GameManager.Factory.Return(loop, _boss.duration4);
                        AttackObject a = loop.GetComponent<AttackObject>();
                        a.Init(_boss,new AtkBase(_boss,a.projectileInfo.dmg));
                    };
                });
            }

            void SpawnBigFlame(Vector3 spawnPos)
            {
                _boss.ep.transform.position = spawnPos;
                _boss.ep.transform.localScale = new Vector3(_boss.scale4, 1, 1);
                var temp =
                    _boss.EffectSpawner.Spawn(
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack4Ready),
                        spawnPos,false);

                temp.transform.SetParent(_boss.ep.transform, true);
                temp.transform.localPosition = Vector3.zero;
                temp.transform.localScale = Vector3.one;
                GameManager.Factory.Return(temp.gameObject, _boss.ready4, () =>
                {
                    var appear = _boss.EffectSpawner.Spawn(
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack4Appear), spawnPos,false);
                    appear.transform.SetParent(_boss.ep.transform);
                    appear.transform.localPosition = Vector3.zero;
                    appear.transform.localScale = Vector3.one;
                    AttackObject atk = appear.GetComponent<AttackObject>();
                    atk.Init(_boss,new AtkBase(_boss,atk.projectileInfo.dmg));
                    AfterEffectSpawner spawner = appear.GetComponent<AfterEffectSpawner>();
                    spawner.OnSpawn += loop =>
                    {
                        GameManager.Factory.Return(loop, _boss.duration4,AtkEnd);
                        AttackObject a = loop.GetComponent<AttackObject>();
                        a.Init(_boss,new AtkBase(_boss,a.projectileInfo.dmg));
                    };
                });
            }
            IEnumerator Attack4Coroutine()
            {
                for (int i = 0; i < _boss.count4; i++)
                {
                    SpawnFlame(midPos + new Vector3(_boss.spawnPosX4 - i * _boss.padding4, 0, 0));
                    SpawnFlame(midPos - new Vector3(_boss.spawnPosX4 - i * _boss.padding4, 0, 0));
                    
                    yield return new WaitForSeconds(_boss.spawnTime4);
                }
                
                yield return new WaitForSeconds(_boss.spawnTime4);

                SpawnBigFlame(midPos);
            }
            void AtkEnd()
            {
                _boss.animator.SetTrigger("AttackEnd");
            }
            public void End()
            {
                if (coroutine != null)
                {
                    _boss.StopCoroutine(coroutine);
                    coroutine = null;
                }
            }

            public void Cancel()
            {
                if (coroutine != null)
                {
                    _boss.StopCoroutine(coroutine);
                    coroutine = null;
                }
            }
        }

        public class AttackPattern5 : IBossAttackPattern
        {
            private JururuBoss _boss;

            public AttackPattern5(JururuBoss boss)
            {
                _boss = boss;
                atkInfo = ResourceUtil.Load<ProjectileInfo>("MonsterAtkInfo");
            }

            ~AttackPattern5()
            {
                ResourceUtil.Release(atkInfo);
            }
            private Coroutine _coroutine;
            public void OnPatternEnter()
            {
                _boss.ActorMovement.SetGravityToZero();
            }

            public void Attack(int index)
            {
                _coroutine = _boss.StartCoroutine(AttackCoroutine());
            }

            public void SetCollider(int index)
            {
            }

            private Guid guid;
            private ProjectileInfo atkInfo;
            private Projectile sun;
            IEnumerator AttackCoroutine()
            {
                yield return _boss.Teleport("Attack5Pos", _boss.teleportTime5).WaitForCompletion();
                AfterEffectSpawner appear = _boss.SpawnEffectInPosition(Define.JururuBossEffect.EffectType.JururuAttack5AuraAppear).GetComponent<AfterEffectSpawner>();
                appear.Init(_boss.EffectSpawner);
                
                sun = GameManager.Factory.Get<Projectile>(FactoryManager.FactoryType.Normal,
                    Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack5Sun), _boss.Position);
                sun.transform.localScale = Vector2.zero;
                sun.Init(_boss, new FixedAmount(0));

                yield return sun.transform.DOScale(Vector3.one * (_boss.sunRadius * 2), _boss.sunCreateTime).SetEase(Ease.Linear).WaitForCompletion();
                _boss.Hide();
                guid = _boss.AddInvincibility();
                float angle = 360f / _boss.projDiagramCount5;
                List<int> temp = new();
                for (int i = 0; i < _boss.projCreateCount5; i++)
                {
                    temp.Clear();
                    for (int j = 0; j < _boss.projDiagramCount5; j++)
                    {
                        temp.Add(j);
                    }

                    for (int k = 0; k < _boss.projCreateCount5PerTime; k++)
                    {
                        int rand = Random.Range(0, temp.Count);
                        int num = temp[rand];
                        temp.Remove(num);
                        float tempAngle = num * angle;
                        Vector2 spawnPos = _boss.Position + new Vector3(Mathf.Cos(tempAngle * Mathf.Deg2Rad),
                            Mathf.Sin(tempAngle * Mathf.Deg2Rad),0) * _boss.projCreateRadius5;

                        Projectile proj = _boss.SpawnProjectile(spawnPos);
                        proj.Init(_boss,new AtkBase(_boss,_boss.projInfo5.dmg));
                        proj.Init(_boss.projInfo5);
                        proj.LookAt(proj.transform.position - _boss.Position);
                        proj.Fire(false);
                    }

                    yield return new WaitForSeconds(_boss.projCreateFrequency5);
                }
                _boss.EffectSpawner.Remove(
                    Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack5Aura));

                sun.LookAt(GameManager.instance.ControllingEntity.Position - sun.Position);
                sun.AddEventUntilInitOrDestroy(_ =>
                {
                    AttackObject explosion = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.Effect,
                        Define.DummyEffects.Explosion, sun.transform.position);
                    explosion.transform.localScale = _boss.sunExplodeSize * 0.3f;
                    explosion.Init(_boss,new AtkBase(_boss,_boss.sunExplodeDmg),1);
                    explosion.Init(atkInfo);
                    _boss.animator.SetTrigger("AttackEnd");
                    _boss.RemoveInvincibility(guid);
                    _boss.Position = sun.Position;
                    _boss.MoveToFloor();
                    _boss.Appear();
                },EventType.OnDestroy);
                sun.Fire();
            }
            public void End()
            {
                _boss.ActorMovement.ResetGravity();
            }

            public void Cancel()
            {
                _boss.RemoveInvincibility(guid);
                _boss.EffectSpawner.Remove(
                    Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack5Aura));
                sun?.Destroy();
                _boss.ActorMovement.ResetGravity();

                if (_coroutine != null)
                {
                    _boss.StopCoroutine(_coroutine);
                    _coroutine = null;
                }
            }
        }

        public class OldAttackPattern5 : IBossAttackPattern
        {
            private JururuBoss _boss;
        
            public void Attack5Move()
            {
                // Transform pos = GameObject.Find("SummonEndPos").transform;
                // _boss.transform.DOMoveY(pos.position.y, _boss.moveTime5).SetEase(_boss.moveEase5)
                //     .OnComplete(() => _boss.animator.SetTrigger("AttackEnd"));
            }
        
            public OldAttackPattern5(JururuBoss boss)
            {
                _boss = boss;
            }
        
            public void OnPatternEnter()
            {
            }
        
            public void Attack(int index)
            {
                // for (int i = 1; i <= _boss.count5; i++)
                // {
                //     var angle = 90 + 180f / _boss.count5 * i;
                //     angle *= Mathf.Deg2Rad;
                //
                //     JururuFlameBall proj =
                //         _boss.SpawnProjectile(_boss.Position +
                //                               new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _boss.radius5);
                //     proj.transform.right = proj.transform.position - _boss.Position;
                //
                //     angle = 270 + 180f / _boss.count5 * i;
                //     angle *= Mathf.Deg2Rad;
                //
                //     JururuFlameBall proj2 =
                //         _boss.SpawnProjectile(_boss.Position +
                //                               new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _boss.radius5);
                //     proj2.transform.right = proj2.transform.position - _boss.Position;
                //
                //     proj.Init(_boss, new AtkBase(_boss, _boss.dmgRatio5_2));
                //     proj2.Init(_boss, new AtkBase(_boss, _boss.dmgRatio5_2));
                //
                //     proj.Init(_boss.projectileInfo5);
                //     proj2.Init(_boss.projectileInfo5);
                //
                //     proj.LookAt(proj.transform.right.normalized);
                //     proj2.LookAt(proj.transform.right.normalized);
                //
                //     proj.Fire(false);
                //     proj2.Fire(false);
                // }
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

        public class AttackPattern6 : IBossAttackPattern
        {
            private JururuBoss _boss;

            public AttackPattern6(JururuBoss boss)
            {
                _boss = boss;
            }

            private Coroutine _coroutine;
            public void OnPatternEnter()
            {
                _boss.ActorMovement.SetGravityToZero();
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        _coroutine = _boss.StartCoroutine(FirstJump());
                        break;
                    case 2:
                        if (_coroutine != null)
                        {
                            _boss.StopCoroutine(_coroutine);
                        }
                        _coroutine = _boss.StartCoroutine(SecondJump());
                        break;
                    case 3:
                        FireProjectiles();
                        break;
                }
            }

            public void SetCollider(int index)
            {
            }

            private Vector2 startPos;
            IEnumerator FirstJump()
            {
                startPos = _boss.transform.position;
                Vector2 endPos = _boss.transform.position + new Vector3(_boss.pattern6BackDash1.x * -(int)_boss.Direction,
                    _boss.pattern6BackDash1.y,0);
                (Tween x, Tween y) tweens = _boss.Rb.DOJumpUp(endPos, _boss.pattern6BackJumpPower1, _boss.pattern6DashTime1);
                tweens.x.SetEase(_boss.pattern6BackDash1Ease).KillWhenBoxCast(_boss, 1,
                    Vector2.left * _boss.DirectionScale, new Vector2(0.5f, 1.5f), LayerMasks.Wall);
                yield return tweens.y.WaitForCompletion();
                _boss.animator.SetTrigger("AttackStart");
            }

            IEnumerator SecondJump()
            {
                Vector2 endPos = new Vector2(_boss.transform.position.x + -(int)_boss.Direction * _boss.pattern6BackDash2,
                    startPos.y);
                (Tween x,Tween y) tweens = _boss.Rb.DOJumpDown(endPos, _boss.pattern6BackJumpPower2, _boss.pattern6DashTime2);
                tweens.x.KillWhenBoxCast(_boss, 1,
                    Vector2.left * _boss.DirectionScale, new Vector2(0.5f, 1.5f), LayerMasks.Wall);
                yield return tweens.y.WaitForCompletion();
                _boss.animator.SetTrigger("AttackEnd");
            }

            void FireProjectiles()
            {
                for (int i = 0; i < _boss.pattern6Projectiles.Count; i++)
                {
                    Vector2 spawnPos = _boss.transform.position -
                                       new Vector3((int)_boss.Direction * _boss.spawnPosX, -_boss.spawnPosY);
                    JururuFlameBall flameBall = _boss.SpawnProjectile(spawnPos + i * _boss.padding * Vector2.up);
                    flameBall.Init(_boss, new AtkBase(_boss, _boss.pattern6ProjectileInfo.dmg));
                    flameBall.Init(_boss.pattern6ProjectileInfo);
                    flameBall.firstVelocity = _boss.pattern6Projectiles[i].normalized * _boss.pattern6ProjectileInfo.firstVelocity.magnitude;
                    flameBall.Fire();
                }
            }
            
            public void End()
            {
                _boss.ActorMovement.ResetGravity();
            }

            public void Cancel()
            {
                if (_coroutine != null)
                {
                    _boss.StopCoroutine(_coroutine);
                }
                _boss.ActorMovement.ResetGravity();
            }
        }

        public class AttackPattern6_2 : IBossAttackPattern
        {
            private JururuBoss _boss;

            public AttackPattern6_2(JururuBoss boss)
            {
                _boss = boss;
            }

            private Coroutine _coroutine;
            private (Tween x, Tween y) tweens;
            public void OnPatternEnter()
            {
                _boss.ActorMovement.SetGravityToZero();
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        _coroutine = _boss.StartCoroutine(FirstJump());
                        break;
                    case 2:
                        if (_coroutine != null)
                        {
                            _boss.StopCoroutine(_coroutine);
                        }
                        _coroutine = _boss.StartCoroutine(SecondJump());
                        break;
                    case 3:
                        FireProjectiles();
                        break;
                }
            }

            public void SetCollider(int index)
            {
            }

            private Vector2 startPos;

            void CancelTweens()
            {
                tweens.x?.Kill();
                tweens.y?.Kill();
            }
            IEnumerator FirstJump()
            {
                CancelTweens();
                startPos = _boss.transform.position;
                Vector2 endPos = _boss.transform.position + new Vector3(_boss.pattern6_2BackDash1.x * -(int)_boss.Direction,
                    _boss.pattern6_2BackDash1.y);
                tweens = _boss.Rb.DOJumpUp(endPos, _boss.pattern6_2BackJumpPower1, _boss.pattern6_2DashTime1);
                tweens.x.SetEase(_boss.pattern6_2BackDash1Ease).KillWhenBoxCast(_boss, 1,
                    Vector2.left * _boss.DirectionScale, new Vector2(0.5f, 1.5f), LayerMasks.Wall);
                yield return tweens.y.WaitForCompletion();
                _boss.animator.SetTrigger("AttackStart");
            }

            IEnumerator SecondJump()
            {
                CancelTweens();
                Vector2 endPos = new Vector2(_boss.transform.position.x + -(int)_boss.Direction * _boss.pattern6_2BackDash2,
                    startPos.y);
                tweens = _boss.Rb.DOJumpDown(endPos, _boss.pattern6_2BackJumpPower2, _boss.pattern6_2DashTime2);
                tweens.x.KillWhenBoxCast(_boss, 1,
                    Vector2.left * _boss.DirectionScale, new Vector2(0.5f, 1.5f), LayerMasks.Wall);
                yield return tweens.y.WaitForCompletion();
                _boss.animator.SetTrigger("AttackEnd");
            }

            void FireProjectiles()
            {
                for (int i = 0; i < _boss.pattern6_2Projectiles.Count; i++)
                {
                    Vector2 spawnPos = _boss.transform.position -
                                       new Vector3((int)_boss.Direction * _boss.spawnPosX, -_boss.spawnPosY);
                    JururuFlameBall flameBall = _boss.SpawnProjectile(spawnPos + i * _boss.padding * Vector2.up);
                    flameBall.Init(_boss, new AtkBase(_boss, _boss.pattern6_2ProjectileInfo.dmg));
                    flameBall.Init(_boss.pattern6_2ProjectileInfo);
                    flameBall.firstVelocity = _boss.pattern6_2Projectiles[i].normalized * _boss.pattern6_2ProjectileInfo.firstVelocity.magnitude;
                    flameBall.Fire();
                }
            }

            public void End()
            {
                _boss.ActorMovement.ResetGravity();
            }

            public void Cancel()
            {
                if (_coroutine != null)
                {
                    _boss.StopCoroutine(_coroutine);
                }
                CancelTweens();
                _boss.ActorMovement.ResetGravity();
            }
        }

        public class AttackPattern7 : IBossAttackPattern
        {
            private JururuBoss _boss;
            private Tween _tween;
            public AttackPattern7(JururuBoss boss)
            {
                _boss = boss;
            }
            void FirstMove()
            {
                _tween = _boss.MoveToPlayer(_boss.minMeleeMove7, _boss.minMove7,_boss.maxMove7, _boss.moveSpeed7, _boss.moveEase7).SetSpeedBased();
                _tween.onKill +=
                    () =>
                    {
                        _boss.animator.SetTrigger("AttackStart");
                    };
            }

            void ReadyMove()
            {
                _boss.FlipToPlayer();
                _boss.Rb.DOMoveX(_boss.DirectionScale * _boss.readyMove7Distance, _boss.readyMove7Time).SetRelative()
                    .SetEase(_boss.readyMove7Ease);
            }
            void AttackMove()
            {
                _tween = _boss.MoveToPlayer(_boss.secondMeleeMinMove7, _boss.secondMinMove7, _boss.secondMaxMove7,
                    _boss.secondMoveSpeed7,
                    _boss.secondMoveEase7);
            }

            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        FirstMove();
                        break;
                    case 2:
                        ReadyMove();
                        break;
                    case 3:
                        AttackMove();
                        break;
                }
            }

            public void SetCollider(int index)
            {
                float dmg = index == 1 ? _boss.rushDmg7 : _boss.slashDmg7;
                _boss.colliderList.ForEach(x =>
                {
                    x.Init(_boss, new AtkBase(_boss, dmg));
                });
            }

            public void End()
            {
                _boss.ActorMovement.StopWithFall();
            }

            public void Cancel()
            {
                _tween?.Kill();
                _tween = null;
            }
        }

        public class AttackPattern7_1 : IBossAttackPattern
        {
            private readonly JururuBoss _boss;

            public AttackPattern7_1(JururuBoss boss)
            {
                _boss = boss;
            }

            private Tween _tween;

            void MoveToPlayer()
            {
                _boss.MoveToPlayerInRootMotion(_boss.meleeMinMove7_1, _boss.minMove7_1, _boss.maxMove7_1);
            }
            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                MoveToPlayer();
            }

            public void SetCollider(int index)
            {
                float dmg = index == 1 ? _boss.firstDmg7_1 : _boss.secondDmg7_1;

                _boss.colliderList.ForEach(x =>
                {
                    x.Init(_boss, new AtkBase(_boss, dmg));
                    if (_boss.phase == BossPhase.Phase2)
                    {
                        x.AddEventUntilInitOrDestroy(info =>
                        {
                            if (info?.target is Actor t)
                            {
                                t.AddSubBuff(_boss, SubBuffType.Debuff_Burn);
                            }
                        });
                    }
                });
            }

            public void End()
            {
            }

            public void Cancel()
            {
                _tween?.Kill();
                _tween = null;
            }
        }
        
        public class AttackPattern8 : IBossAttackPattern
        {
            private JururuBoss _boss;
            private Coroutine _coroutine;
            void SpawnFlame(Vector3 spawnPos)
            {
                var temp = _boss.EffectSpawner.Spawn(
                    Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack4Ready),
                    spawnPos,false);
                temp.transform.SetParent(_boss.BossAttacks.transform);
                GameManager.Factory.Return(temp.gameObject, _boss.flameWallReady8, () =>
                {
                    var appear = _boss.EffectSpawner.Spawn(
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack4Appear), spawnPos,false);
                    AfterEffectSpawner spawner = appear.GetComponent<AfterEffectSpawner>();
                    AttackObject atk = appear.GetComponent<AttackObject>();
                    atk.Init(_boss,new AtkBase(_boss,_boss.flameWallDmg));
                    spawner.OnSpawn += loop =>
                    {
                        GameManager.Factory.Return(loop, _boss.flameWallDuration8);
                        AttackObject a = loop.GetComponent<AttackObject>();
                        a.Init(_boss,new AtkBase(_boss,_boss.flameWallDmg2));
                    };
                });
            }
            
            public AttackPattern8(JururuBoss boss)
            {
                _boss = boss;
            }

            IEnumerator SpawnFlameWalls()
            {
                Vector2 startPos = _boss.transform.position;
                for (int i = 0; i < _boss.flameWallCount8; i++)
                {
                    SpawnFlame(startPos +  Vector2.right * (_boss.DirectionScale * ( _boss.flameWallCreateDistance8 + _boss.flameWallPaddingDistance8 * i)));
                    yield return new WaitForSeconds(_boss.flameWallPaddingTime8);
                }
                
                _boss.animator.SetTrigger("AttackEnd");
            }

            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        _coroutine = _boss.StartCoroutine(SpawnFlameWalls());
                        break;
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
                if (_coroutine != null)
                {
                    _boss.StopCoroutine(_coroutine);
                }
            }
        }
        
        public class AttackPattern9 : IBossAttackPattern
        {
            private readonly JururuBoss _boss;
            private Tween _tween;
            
            public AttackPattern9(JururuBoss boss)
            {
                _boss = boss;
            }

            void Teleport()
            {
                _boss.CancelAttack();
                _boss.StopAnimation();

                _boss.Teleport(_boss.teleportTime9, () =>
                {
                    Actor player = GameManager.instance.ControllingEntity;

                    var leftRay = Physics2D.Raycast(player.transform.position, Vector2.left, _boss.teleportMinDist9,
                        LayerMasks.Wall);
                    var rightRay = Physics2D.Raycast(player.transform.position, Vector2.right, _boss.teleportMinDist9,
                        LayerMasks.Wall);
                    Vector2 leftPos = player.transform.position + Vector3.left *
                        (leftRay.collider != null ? leftRay.distance : _boss.teleportMinDist9);
                    Vector2 rightPos = player.transform.position + Vector3.right *
                        (rightRay.collider != null ? rightRay.distance : _boss.teleportMinDist9);

                    float leftDist = Vector2.Distance(_boss.transform.position, leftPos);
                    float rightDist = Vector2.Distance(_boss.transform.position, rightPos);

                    Vector2 endPos = leftDist > rightDist ? rightPos : leftPos;
                    Utils.GetLowestPointByRay(endPos, LayerMasks.GroundAndPlatform, out endPos);
                    return endPos;
                }).onComplete += () =>
                {
                    _boss.animator.SetTrigger("AttackStart");
                    _boss.ResumeAnimation();
                };
            }

            void Move()
            {
               _tween =  _boss.MoveToPlayer(_boss.meleeMove9,_boss.minMove9,_boss.maxMove9,_boss.moveTime9,_boss.moveEase9);
            }

            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        Teleport();
                        break;
                    case 2:
                        Move();
                        break;
                }
            }

            public void SetCollider(int index)
            {
                _boss.colliderList.ForEach(x =>
                {
                    x.Init(_boss, new AtkBase(_boss, _boss.dmg9));
                    if (_boss.phase == BossPhase.Phase2)
                    {
                        x.AddEventUntilInitOrDestroy(info =>
                        {
                            if (info?.target is Actor t)
                            {
                                t.AddSubBuff(_boss, SubBuffType.Debuff_Burn);
                            }
                        });
                    }
                });
            }

            public void End()
            {
                _boss.ResumeAnimation();
                _boss.ActorMovement.ResetGravity();
            }

            public void Cancel()
            {
                _boss.ResumeAnimation();
                _tween?.Kill();
                _tween = null;
                _boss.ActorMovement.ResetGravity();
            }
        }
        
        public class AttackPattern10 : IBossAttackPattern
        {
            private readonly JururuBoss _boss;
            
            public AttackPattern10(JururuBoss boss)
            {
                _boss = boss;
            }

            private readonly Queue<Projectile> arrows = new();
            
            void CreateArrow()
            {
                Projectile arrow = GameManager.Factory.Get<Projectile>(FactoryManager.FactoryType.AttackObject,
                    Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuFireArrow),_boss.Position);
                arrow.transform.localScale = new Vector3(_boss.arrowScale10,_boss.arrowScale10,_boss.arrowScale10);
                arrow.SetDirection(_boss.Direction);
                arrow.Init(_boss,new AtkBase(_boss,_boss.arrowInfo.dmg),10);
                arrow.Init(_boss.arrowInfo);
                arrow.RotateBeforeFire(_boss.Direction == EActorDirection.Left ? 180 : 0);
                arrows.Enqueue(arrow);
            }
            void TeleportToEnd()
            {
                Vector2 leftPos = GameObject.Find("Attack10LeftPos").transform.position;
                Vector2 rightPos = GameObject.Find("Attack10RightPos").transform.position;
                Actor player = GameManager.instance.ControllingEntity;
                
                Vector2 pos = Vector2.Distance(leftPos,player.Position) > Vector2.Distance(rightPos,player.Position) ? leftPos : rightPos;
                var seq = _boss.Teleport(pos, 0.1f);
                seq.AppendCallback(_boss.FlipToPlayer);
                seq.AppendCallback(CreateArrow);
                seq.AppendInterval(_boss.aimTime10);
                seq.AppendCallback(() =>
                {
                    _boss.animator.SetTrigger("AttackStart");
                });
            }
            void FireArrow()
            {
                if (arrows.Count == 0) return;
                Projectile arrow = arrows.Dequeue();
                
               
                arrow.Fire(false);
            }

            IEnumerator SecondAim()
            {
                yield return new WaitForSeconds(_boss.secondAimTime10);
                _boss.animator.SetTrigger("AttackStart");
            }

            public void OnPatternEnter()
            {
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                         TeleportToEnd();
                         break;
                    case 2:
                        FireArrow();
                        break;
                    case 3:
                        _boss.StartCoroutine(SecondAim());
                        break;
                    case 4:
                        CreateArrow();
                        break;
                }
            }

            public void SetCollider(int index)
            {
            }

            public void End()
            {
                while (arrows.Count > 0)
                {
                    arrows.Dequeue().Destroy();
                }
            }

            public void Cancel()
            {
                while (arrows.Count > 0)
                {
                    arrows.Dequeue().Destroy();
                }
            }
        }
        
        public class AttackPattern10_1 : IBossAttackPattern
        {
            private JururuBoss _boss;

            private readonly Queue<Projectile> arrows = new();

            private float startY;
            private (Tween x ,Tween y) tweens;
            
            public AttackPattern10_1(JururuBoss boss)
            {
                _boss = boss;
            }

            void Teleport()
            {
                _boss.FlipToPlayer();
                startY = _boss.transform.position.y;
                _boss.Teleport(
                    _boss.transform.position + new Vector3(_boss.DirectionScale * _boss.teleportXDistance10_1,
                        _boss.teleportYDistance10_1, 0), Time.fixedDeltaTime).onComplete += () =>
                {
                    _boss.FlipToPlayer();
                    CreateArrows();
                };
            }

            void CreateArrows()
            {
                float angle = _boss.arrowAngle10_1;
                int half = Mathf.FloorToInt(_boss.arrowCount10_1 / 2);
                
                for (int i = 0; i < _boss.arrowCount10_1; i++)
                {
                    float currAngle = angle * (i - half);
                    Projectile arrow = GameManager.Factory.Get<Projectile>(FactoryManager.FactoryType.AttackObject,
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuFireArrow),
                        _boss.Position);

                    arrow.transform.localScale = new Vector3(_boss.arrowScale10,
                        _boss.arrowScale10, _boss.arrowScale10);
                    arrow.Init(_boss,new AtkBase(_boss,_boss.arrowInfo.dmg),10);
                    arrow.Init(_boss.arrowInfo);
                    arrow.RotateBeforeFire(_boss.DirectionScale * currAngle + (_boss.Direction == EActorDirection.Left ? 180 : 0) - 45 * _boss.DirectionScale);
                    
                    arrow.AddEventUntilInitOrDestroy( _ =>
                    {
                        Utils.GetLowestPointByRay(arrow.Position, LayerMasks.GroundAndPlatform, out Vector2 pos);
                        CreateFlameGround(pos);
                    },EventType.OnDestroy);
                    arrows.Enqueue(arrow);
                }
            }

            void CreateFlameGround(Vector2 pos)
            {
                var flameGround = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
                    Define.BossObjects.JururuFlameGround,pos);
                flameGround.Init(_boss,new AtkBase(_boss,_boss.flameGroundInfo10_1.dmg),_boss.flameGroundDuration10_1);
                flameGround.Init(_boss.flameGroundInfo10_1);
                flameGround.transform.localScale = new Vector3(_boss.flameGroundSize10_1, 1);
            }
            void JumpBackToStart()
            {
                tweens = _boss.Rb.DOJumpDown(new Vector2(_boss.Position.x + _boss.jumpDistance10_1 * -_boss.DirectionScale, startY),
                    _boss.jumpHeight10_1, _boss.jumpTime10_1);
                tweens.x.KillWhenBoxCast(_boss, 1, Vector2.left * _boss.DirectionScale, new Vector2(0.5f, 1),
                    LayerMasks.Wall);
                tweens.y.onComplete += () =>
                {
                    _boss.animator.SetTrigger("AttackEnd");
                };
            }
            void FireArrows()
            {
                while (arrows.Count > 0)
                {
                    Projectile arrow = arrows.Dequeue();
                    
                    arrow.Fire(false);
                }
            }

            public void OnPatternEnter()
            {
                _boss.ActorMovement.SetGravityToZero();
            }

            public void Attack(int index)
            {
                switch (index)
                {
                    case 1:
                        Teleport();
                        break;
                    case 2:
                        FireArrows();
                        break;
                    case 3:
                        JumpBackToStart();
                        break;
                }
            }

            public void SetCollider(int index)
            {
            }

            public void End()
            {
                while (arrows.Count > 0)
                {
                    arrows.Dequeue().Destroy();
                }
                tweens.x?.Kill();
                tweens.y?.Kill();
                tweens.x = null;
                tweens.y = null;
                _boss.ActorMovement.ResetGravity();
            }

            public void Cancel()
            {
                while (arrows.Count > 0)
                {
                    arrows.Dequeue().Destroy();
                }
                tweens.x?.Kill();
                tweens.y?.Kill();
                tweens.x = null;
                tweens.y = null;
                _boss.ActorMovement.ResetGravity();
            }
        }
    }
}