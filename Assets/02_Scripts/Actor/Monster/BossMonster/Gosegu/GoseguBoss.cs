using DG.Tweening;
using Default;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using Apis;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Apis
{
    public class GoseguBoss : BossMonster
    {

        [HideInInspector]public SkeletonMecanim skeleton;
        [TabGroup("기획쪽 수정 변수들/group1", "보스 패턴")]
        [SerializeField] PatternData patternData;

        [Serializable]
        [HideLabel]
        public class PatternData
        {
            [Title("공격패턴 1")]
            [LabelText("투척 개수 최소")] public int flaskCount1Min;
            [LabelText("투척 개수 최대")] public int flaskCount1Max;
            [LabelText("투척 거리")]
            public float flaskDistance;
            [LabelText("투척 높이")] public float flaskHeight1;
            [LabelText("투척 시간")]
            public float flaskTime;
            [LabelText("투척 거리간격 최소")] public float flaskPadding1Min;
            [LabelText("투척 거리간격 최대")] public float flaskPadding1Max;
            [LabelText("투척 시간간격")] public float flaskPaddingTime1;
            [LabelText("Ease")] public Ease ease;

            [Title("공격패턴 2")]
            [LabelText("이동 시간")] public float moveTime2;
            [LabelText("이동 거리")] public float distance2;
            [LabelText("점프 높이")] public float jumpHeight2;
            [LabelText("점프 Ease")] public Ease ease2;

            [Title("공격패턴 3")]
            [ListDrawerSettings(IsReadOnly = true)]
            [LabelText("이동 시간")] public float[] moveTime3 = new float[3];
            [ListDrawerSettings(IsReadOnly = true)]
            [LabelText("이동 거리")] public float[] distance3 = new float[3];
            [ListDrawerSettings(IsReadOnly = true)]
            [LabelText("이동 Ease")] public Ease[] ease3 = new Ease[3];

            [Title("공격패턴 4")]
            [LabelText("이동 시간")] public float moveTime4;
            [LabelText("이동 거리")] public float distance4;
            [LabelText("점프 높이")] public float jumpHeight4;
            [LabelText("이동 Ease")] public Ease ease4;
            [LabelText("투사체 개수 최소")] public int flaskCount4Min;
            [LabelText("투사체 개수 최대")] public int flaskCount4Max;
            [LabelText("투사체 거리간격 최소")] public float padding4Min;
            [LabelText("투사체 거리간격 최대")] public float padding4Max;
            [LabelText("투사체 투척 시간")] public float flaskMoveTime4;
            [LabelText("투척 시간간격 최소")] public float flaskPaddingTime4Min;
            [LabelText("투척 시간간격 최대")] public float flaskPaddingTime4Max;
            [LabelText("투사체 Ease")] public Ease ease4_2;

            [TitleGroup("공격패턴 5")]
            [TabGroup("공격패턴 5/group", "첫번째 점프")]
            [LabelText("이동 시간")] public float moveTime5_1;
            [TabGroup("공격패턴 5/group", "첫번째 점프")]
            [LabelText("이동 높이")] [Tooltip("플레이어 위쪽 이동 높이")]
            public float moveHeight5_1;
            [TabGroup("공격패턴 5/group", "첫번째 점프")]
            [LabelText("포물선 높이")][Tooltip("점프시 포물선 높이")]
            public float jumpHeight5_1;
            [TabGroup("공격패턴 5/group", "첫번째 점프")]
            [LabelText("이동 Ease")] public Ease ease5_1;
            [TabGroup("공격패턴 5/group", "첫번째 점프")]
            [Tooltip("최대 높이에서 다음 점프까지 대기시간")]
            [LabelText("대기시간")] public float waitTime5;

            [TabGroup("공격패턴 5/group", "두번째 점프")]
            [InfoBox("두번째 점프는 순간적으로 날라가게 만들어야 해서 power 및 중력으로 조절합니다.")]

            [LabelText("떨어지는 속도")] public float gravityScale5;
            [TabGroup("공격패턴 5/group", "두번째 점프")]
            [LabelText("X방향 힘")] public float xForce;
            [TabGroup("공격패턴 5/group", "두번째 점프")]
            [LabelText("y방향 힘")] public float yForce;

            [Title("공격패턴 6")]
            [LabelText("투사체 소환 높이")] public float height6;
            [LabelText("투사체 생성 구역 크기")] public Vector2 box;
            [LabelText("투사체 생성 횟수")] public int spawnCount6;
            [LabelText("투사체 생성 주기")] public float spawnInterval6;
            [LabelText("플라스크 소환 개수")] public int flaskCount6;
            [LabelText("바위 소환 개수")] public int rockCount6;

            [Title("실험관")] [LabelText("소환 위치")] public List<float> spawnPoints;
            [LabelText("낙하 시간")] public float spawnTime;
            

        } //패턴 관련 변수

        public Bone leftHand;
        public Bone rightHand;

        Actor player => GameManager.instance.ControllingEntity;

        public AttackObject atkObject;

        public void InitAtk()
        {
            atkObject.Init(this,new AtkBase(this));
        }

        protected override void SetAttackPatterns()
        {
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.red;
            Vector2 center = new Vector2(0, patternData.height6 + patternData.box.y / 2);
            Gizmos.DrawWireCube(center, patternData.box);
        }

        
        protected override void Awake()
        {
            base.Awake();
            skeleton = Utils.GetComponentInParentAndChild<SkeletonMecanim>(gameObject);
            SetState(BossState.Wait);
            OnTransformEnd.AddListener(() =>
            {
                CurHp = MaxHp;
            });
            phase = BossPhase.Phase2;
        }
        protected override void Start()
        {
            base.Start();
            OnTransformEnd.AddListener(() =>
            {
                if ((int)phase == 2)
                {
                    GameManager.Sound.Play(Define.BGMList.GoseguPhase2Intro, Define.Sound.BGM);
                    GameManager.Sound.ReserveBGM(Define.BGMList.GoseguPhase2Loop);
                }
            });

            leftHand = skeleton.skeleton.FindBone("handL");
            rightHand = skeleton.skeleton.FindBone("handR");
        }
        
        DG.Tweening.Sequence seq;

        [Button]
        public void AttackTestTrigger(int index)
        {
            animator.SetInteger("Attack", index);
        }
        
        public void Attack1()
        {
            int count = Random.Range(patternData.flaskCount1Min, patternData.flaskCount1Max + 1);
            float dist = Random.Range(patternData.flaskPadding1Min, patternData.flaskPadding1Max);

            for (int i = 0; i < count; i++)
            {
                float timePadding = patternData.flaskPaddingTime1;
                
                GoseguFlask flask = CreateFlask(leftHand);
                flask.Init(this,new AtkBase(this));
                flask.Throw(patternData.flaskTime + i * timePadding,
                    patternData.flaskDistance + i * dist, patternData.flaskHeight1).SetEase(patternData.ease);
            }
        }

        public void Attack2()
        {
            Rb.DOKill();
            
            animator.SetFloat("Atk2Speed",1 / patternData.moveTime2);
            
            var values = ActorMovement.DoJumpTween(patternData.moveTime2, patternData.jumpHeight2,
                patternData.distance2, false);
            xTween = values.Item1;
            yTween = values.Item2;
            
            xTween.SetEase(patternData.ease2);
            
            yTween = values.Item2;
            animator.ResetTrigger("AttackEnd");
            yTween.onKill += () =>
            {
                Rb.gravityScale = ActorMovement.GravityScale;
                animator.SetTrigger("AttackEnd");
            };
        }
        public void Attack3Move(int index)
        {
            FlipToPlayer();

            Vector2 dir = Direction == EActorDirection.Left ? Vector2.left : Vector2.right;

            if (Physics2D.Raycast(Position, dir, 1, LayerMasks.Wall)) return;
            
            Rb.DOMoveX(Rb.position.x + (int)Direction * patternData.distance3[index - 1], patternData.moveTime3[index - 1]).
                SetEase(patternData.ease3[index - 1]);
        }

        private Tween xTween;
        private Tween yTween;
        
        public void Attack4()
        {
            bool isThrow = false;
            float y = transform.position.y + patternData.jumpHeight4;
            Rb.DOKill();
            var values = ActorMovement.DoJumpTween(patternData.moveTime4, patternData.jumpHeight4,
                patternData.distance4,false);
            xTween = values.Item1;
            yTween = values.Item2;
            
            xTween.onUpdate += () =>
            {
                var dir = new Vector2((int)Direction, 0);
                Debug.DrawRay(Position, dir, Color.red);
                if (Physics2D.Raycast(Position, dir, 1f, LayerMasks.Wall))
                {
                    xTween.Kill();
                }
            };
            yTween.onUpdate += () =>
            {
                if (!isThrow && Math.Abs(transform.position.y - y) < 0.5f)
                {
                    isThrow = true;
                    int count = Random.Range(patternData.flaskCount4Min, patternData.flaskCount4Max + 1);
                    ThrowToPlayerDir(count);
                }
                
                Rb.gravityScale = 0;
            };
            yTween.onKill += () =>
            {
                animator.SetTrigger("AttackEnd");
                Rb.gravityScale = ActorMovement.GravityScale;
            };
            
            yTween.onComplete += () =>
                {
                    Rb.gravityScale = ActorMovement.GravityScale;
                    animator.SetTrigger("AttackEnd");
                };
            
            xTween.SetEase(patternData.ease4);
        }

        public void Attack5Jump1()
        {
            Rb.DOKill();
            Vector2 pos = player.Position + Vector3.up * patternData.moveHeight5_1;
            Rb.gravityScale = 0;

            Rb.DOJump(pos, patternData.jumpHeight5_1, 1, patternData.moveTime5_1).SetEase(patternData.ease5_1).SetUpdate(UpdateType.Fixed).
                AppendCallback(() =>
                {
                    animator.SetTrigger("AttackEnd");
                }).AppendInterval(patternData.waitTime5).
               AppendCallback(Attack5Jump2);
        }

        void Attack5Jump2()
        {
            Rb.gravityScale = patternData.gravityScale5;
            Rb.DOKill();

            Rb.AddForce(new Vector2((int)Direction * patternData.xForce, patternData.yForce), ForceMode2D.Impulse);

            seq = DOTween.Sequence().SetAutoKill(true);

            seq.SetDelay(5).OnKill(() =>
            {
                animator.SetTrigger("AttackEnd");
                Rb.gravityScale = ActorMovement.GravityScale;
            });
            seq.SetUpdate(UpdateType.Fixed);
            seq.onUpdate += () =>
            {
                if (Physics2D.Raycast(transform.position, Vector2.down,1,LayerMasks.GroundAndPlatform))
                {
                    seq.Kill();
                    seq = null;
                }
            };
        }
        public void ThrowToPlayerDir(int num)
        {
            if (player == null) return;
            float padding = Random.Range(patternData.padding4Min, patternData.padding4Max);
            float startX = player.Position.x - (num - 1) / 2f * padding;
            for (int i = 0; i < num; i++)
            {
                float timePadding = Random.Range(patternData.flaskPaddingTime4Min, patternData.flaskPaddingTime4Max);
                
                Vector2 pos = new(startX + padding * i, player.Position.y);
                GoseguFlask flask = CreateFlask(rightHand);
                flask.Init(this,new AtkBase(this));
                flask.ThrowToTarget(patternData.flaskMoveTime4 + timePadding, pos, 3).SetEase(patternData.ease4_2);
            }
        }
        public GoseguFlask CreateFlask(Bone hand)
        {
            hand.UpdateWorldTransform();
            Vector2 pos = hand.GetWorldPosition(skeleton.transform);
            GameObject obj = GameManager.Factory.Get(FactoryManager.FactoryType.AttackObject, "GoseguFlask", pos);
            GoseguFlask flask = obj.GetComponent<GoseguFlask>();
            
            return flask;
        }

        public GoseguFlask CreateFlask(Vector2 pos)
        {
            GameObject obj = GameManager.Factory.Get(FactoryManager.FactoryType.AttackObject, "GoseguFlask", pos);
            GoseguFlask flask = obj.GetComponent<GoseguFlask>();

            return flask;
        }
        public GoseguRock CreateRock(Vector2 pos)
        {
            GameObject obj = GameManager.Factory.Get(FactoryManager.FactoryType.AttackObject, "GoseguRock", pos);
            GoseguRock rock = obj.GetComponent<GoseguRock>();

            return rock;
        }

        public void Attack6()
        {
            animator.ResetTrigger("AttackEnd");
            var loop = DOTween.Sequence();

            float x = patternData.box.x / 2;

            loop.AppendInterval(patternData.spawnInterval6);
            loop.AppendCallback(() =>
            {
                for (int i = 0; i < patternData.flaskCount6; i++)
                {
                    float randx = Random.Range(-x, patternData.height6);
                    float randy = Random.Range(x, patternData.height6 + patternData.box.y);
                    Vector2 pos = new(randx, transform.position.y + randy);

                    GoseguFlask flask = CreateFlask(pos);
                    flask.rb.AddTorque(-100 * (int)Direction);
                    flask.CollideOn();
                }

                for (int i = 0; i < patternData.rockCount6; i++)
                {
                    float randx = Random.Range(-x, patternData.height6);
                    float randy = Random.Range(x, patternData.height6 + patternData.box.y);
                    Vector2 pos = new(randx, randy);

                    GoseguRock rock = CreateRock(pos);
                    rock.rb.AddTorque(-100 * (int)Direction);
                }
            });

            loop.SetLoops(patternData.spawnCount6);
            loop.onKill += () =>
            {
                animator.SetTrigger("AttackEnd");
            };
            loop.onComplete += () =>
            {
                animator.SetTrigger("AttackEnd");
            };
        }

        public GameObject[] walls;
        
        public void SpawnWalls()
        {
            for (int i = 0; i < patternData.spawnPoints.Count; i++)
            {
                walls[i].transform.position = new Vector2(patternData.spawnPoints[i], walls[i].transform.position.y);
                walls[i].transform.DOMoveY(0, patternData.spawnTime);
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMasks.Wall)
            {
                Rb.velocity = Vector2.zero;
                xTween?.Kill();
                if (seq != null && seq.IsActive())
                {
                    seq?.Kill();
                    seq = null;
                }
            }
        }

        public override void Die()
        {
            if (phase == BossPhase.Phase1)
            {
                SetState(BossState.Transform);
                return;
            }
            base.Die();
        }

        public bool isJumped = false;

        public void SetJumped(bool b)
        {
            isJumped = b;
        }

        private Vector2 pointPos;

        public void SetPointPos()
        {
            Bone point = skeleton.skeleton.FindBone("point");
            skeleton.skeleton.UpdateWorldTransform();
            pointPos = point.GetWorldPosition(transform);
        }
        public void AfterJump()
        {
            if (isJumped)
            {
                Flip();
                SetPositionToRootPoint();
                isJumped = false;
            }
        }
        public void SetPositionToRootPoint()
        {
            Rb.position = pointPos;
        }
    }
}