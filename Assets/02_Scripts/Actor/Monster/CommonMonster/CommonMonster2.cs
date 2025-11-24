using System;
using System.Collections.Generic;
using Apis;
using Apis.DataType;
using Apis.Interface;
using Apis.Managers;
using NewMonster;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using Default;

namespace Apis.CommonMonster2
{
    public partial class CommonMonster2: Monster, IVisible, IActivation
    {
        // const, readonly
        private readonly int _moveStopBool = Animator.StringToHash("moveStop");
        
        // inspector 용
        [TabGroup("기획쪽 수정 변수들/group1", "몬스터 설정")][LabelText("패트롤 이동속도 비율")][SerializeField][Tooltip("1 = 기본 이동속도")]
        private float patrolCoef = 0.3f;
        [TabGroup("기획쪽 수정 변수들/group1", "몬스터 설정")][LabelText("회전 시간")] public float turnTime = 1f;
        
        public List<AttackObject> colliderAttack;
        
        
        // 참조용
        public PatternGroupChecker PgChecker { get; private set; }
        public PatternGroupController PgController { get; private set; }
        public List<Projectile> Projectiles { get; private set; }
        
        public bool IsTurning { get; set; }
        // 0: non-forced, 1: right forced, -1: left forced
        public int ForcedPatrolRotation { get; set; }

        
        // privates
        private bool _ableTurn;
        private bool _lastMoveStop;
        private bool CanMoveStop { get; set; } = true;

        private Transform _playerTrans;
        private UI_SemiHpBar _hpBar;


        #region Init

        protected override void Awake()
        {
            base.Awake();
            animator = Utils.GetComponentInParentAndChild<Animator>(gameObject, false);
            
            _playerTrans = GameManager.instance.PlayerTrans;
            PgChecker = GetComponent<PatternGroupChecker>();
            PgController = GetComponent<PatternGroupController>();
            PgController.InitCheck();
            Projectiles = new();
            
            AwakeState();
            
            // TODO: 씬 전환 시스템이 바뀜에 따라 어떻게 할지 다시 작성해야함.
            GameManager.Scene.WhenSceneLoadBegin.AddListener(SceneLoadBegin);
            EventManager.AddEvent(EventType.OnDestroy, _ =>
            {
                GameManager.Scene.WhenSceneLoadBegin.RemoveListener(SceneLoadBegin);
            });
        }

        public override void Init(MonsterDataType monsterDataType)
        {
            base.Init(monsterDataType);
            GetHpBar();
            foreach (var ca in colliderAttack)
            {
                ca.Init(this, new AtkBase(this, ca.projectileInfo.dmg));
            }

            
        }

        #endregion

        #region Factory

        public override void OnReturn()
        {
            ChangeMonsterState(MonsterState.None);
            base.OnReturn();
            CloseHpBar();
        }

        #endregion

        #region onoff

        public override void AnimPauseOn()
        {
            CanMoveStop = false;
            base.AnimPauseOn();
        }

        public override void AnimPauseOff()
        {
            CanMoveStop = true;
            base.AnimPauseOff();
        }

        #endregion

        
        
        public virtual List<int> GetCheckedPatternGroupsWithCondition(float playerDist)
        {
            return PgChecker.GetCheckedPatternGroups(playerDist);
        }


        #region moveUtil

        // 나중에 몬스터마다 달라질수 있지만 일단 통일
        public bool MonsterMove(bool isPatrol, float ratio = 1)
        {
            
            if (!MonsterData.isMove || !ableMove || CheckWallAndCliff())
            {
                if (!_lastMoveStop && CanMoveStop)
                {
                    _lastMoveStop = true;
                    animator.SetBool(_moveStopBool, true);
                    MoveComponent.Stop();
                }

                return false;
            }
            else
            {
                if (_lastMoveStop)
                {
                    _lastMoveStop = false;
                    animator.SetBool(_moveStopBool, false);
                }

                ActorMovement.Move(Direction, (isPatrol ? patrolCoef : 1) * ratio, MonsterData.isFlying);
                return true;
            }
        }
        
        public void TurnWithoutDelay()
        {
//            Debug.Log($"turn {Direction}");
            EActorDirection toDir = (EActorDirection)((int)Direction * -1);
            Direction = toDir;
            Vector3 localScale = transform.localScale;
            transform.localScale =
                new Vector3(
                    (toDir == EActorDirection.Right ? 1 : -1) * Mathf.Abs(localScale.x),
                    localScale.y, 
                    localScale.z
                );
        }

        #endregion
        
        

        #region HpBar

        public void GetHpBar()
        {
            if (_hpBar != null)
            {
                ResetHpBar();
                return;
            }
            _hpBar = GameManager.UI.CreateUI("UI_SemiHpBar", UIType.Ingame, withoutActivation:true) as UI_SemiHpBar;
            if (_hpBar == null)
            {
                Debug.LogError("알수 없는 오류: common monster hp bar 생성 실패");
                return;
            }
            _hpBar.InitActor(this);
            _hpBar.SetTrans(transform);
            _hpBar.TryActivated();

            ResetHpBar();
        }

        private void ResetHpBar()
        {
            _hpBar.ChangeGroggyBarImg(CurGroggyGauge);
        }

        public void CloseHpBar()
        {
            if (_hpBar == null) return;
            GameManager.UI.CloseUI(_hpBar);
            _hpBar = null;
        }

        #endregion
        

        // attack object on/off with code
        public void AttackColliderOn(int index) => colliderAttack[index]?.gameObject.SetActive(true);
        public void AttackColliderOff(int index) => colliderAttack[index]?.gameObject.SetActive(false);
        
        
        public override void Die()
        {
            base.Die();
            ChangeMonsterState(MonsterState.Death);
        }
    }
}