using System;
using Apis.BehaviourTreeTool;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine.Serialization;
using static Apis.BehaviourTreeTool.BlackBoard.JururuBoss;

namespace Apis
{
    public partial class JururuBoss : BossMonster
    {

        #region 수정전 패턴

        // [TitleGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5")]
        // [Tooltip("백분율로 설정 (100 = 공격력 100%)")]
        // [LabelText("투사체 공격 계수(백분율)")]
        // public float dmgRatio5_2;
        //
        // [LabelText("낫 공격 계수")] [SerializeField]
        // float dmgRatio5;
        //
        // [TitleGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5")] [LabelText("투사체 개수")] [SerializeField]
        // int count5;
        //
        // [TitleGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5")] [LabelText("투사체 소환 반경")] [SerializeField]
        // float radius5;
        //
        // [TitleGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5")] [LabelText("투사체 설정")] [SerializeField]
        // ProjectileInfo projectileInfo5;
        //
        // [TitleGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5")]
        // [Tooltip("패턴 종료 후 바닥으로 내려오는 시간")]
        // [LabelText("내려오는 시간")]
        // [SerializeField]
        // float moveTime5;
        //
        // [TitleGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5")] [LabelText("내려오는 Ease")] [SerializeField]
        // Ease moveEase5;

        #endregion

        #region Inspectors

        #region 패턴 1

        [TabGroup("기획쪽 수정 변수들/group1", "패턴 관련")]
        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")]
        [Tooltip("백분율로 설정 (100 = 공격력 100%)")]
        [SerializeField]
        [LabelText("공격 계수")]
        float dmgRatio1;
        
        
        [FormerlySerializedAs("minMove1")] [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [LabelText("근접 요구거리")] [SerializeField]
        private float meleeMinMove1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [LabelText("최소 이동거리")] [SerializeField]
        private float minMove1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [LabelText("최대 이동거리")] [SerializeField]
        private float maxMove1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [SerializeField] [LabelText("이동시간")]
        public float moveTime1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1")] [SerializeField] [LabelText("이동Ease")]
        public Ease moveEase1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("1타 데미지")]
        public float firstDmg1_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("2타 데미지")]
        public float secondDmg1_1;
        
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동거리 1")]
        public float firstMoveDistance1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동시간 1")]
        public float firstMoveTime1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동Ease 1")]
        public Ease firstMoveEase1_1;

        [FormerlySerializedAs("secondMinMove1_1")] [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("근접 요구거리 2")]
        public float secondMeleeMinMove1_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [LabelText("최소 이동거리2")] [SerializeField]
        private float secondMinMove1_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("최대 이동거리 2")]
        public float secondMaxMove1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동시간 2")]
        public float secondMoveTime1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동Ease 2")]
        public Ease secondMoveEase1_1;

        [FormerlySerializedAs("thirdMinMove1_1")] [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("근접 요구거리 3")]
        public float thirdMeleeMinMove1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("최소 이동거리 3")]
        public float thirdMinMove1_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("최대 이동거리 3")]
        public float thirdMaxMove1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동시간 3")]
        public float thirdMoveTime1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동Ease 3")]
        public Ease thirdMoveEase1_1;

        [FormerlySerializedAs("fourthMinMove1_1")] [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("근접 요구거리 4")]
        public float fourthMeleeMinMove1_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("최소 이동거리 4")]
        public float fourthMinMove1_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("최대 이동거리 4")]
        public float fourthMaxMove1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동시간 4")]
        public float fourthMoveTime1_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴1/패턴", "1_1")] [SerializeField] [LabelText("이동Ease 4")]
        public Ease fourthMoveEase1_1;

        #endregion

        #region 패턴 3

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "3")]
        [LabelText("공격 계수 (백분율)")]
        public float dmgRatio3;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "3")]
        [Tooltip("보스 발기준 후방 nM")]
        [LabelText("투사체 생성 x위치")]
        [SerializeField]
        float spawnPosX;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "3")]
        [Tooltip("보스 발기준 높이 nM")]
        [LabelText("투사체 생성 높이")]
        [SerializeField]
        float spawnPosY;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "3")]
        [Tooltip("생성될 때 투사체간의 Y 거리")]
        [LabelText("투사체간 거리")]
        [SerializeField]
        float padding;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "3")] [LabelText("투사체 발사 방향 (속도)")]
        public List<Vector2> projectiles = new();

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴3/패턴", "3")] [LabelText("투사체 설정")]
        public ProjectileInfo projectileInfo;

        #endregion

        #region 패턴 4

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")]
        [Tooltip("1회당 양쪽에 1개씩 생성")]
        [LabelText("FlameWall 생성횟수")]
        public int count4;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")]
        [Tooltip("중앙으로부터 xM")]
        [LabelText("생성 시작거리")]
        [SerializeField]
        float spawnPosX4;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")]
        [Tooltip("FlameWall간의 사이 거리")]
        [LabelText("생성 간격거리")]
        [SerializeField]
        float padding4;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")]
        [Tooltip("FlameWall의 1회 생성당 딜레이 시간")]
        [LabelText("생성 간격시간")]
        [SerializeField]
        float spawnTime4;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")]
        [Tooltip("백분율로 설정 (100 = 공격력 100%)")]
        [LabelText("공격 계수")]
        [SerializeField]
        float dmgRatio4;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")]
        [Tooltip("백분율로 설정 (100 = 공격력 100%)")]
        [LabelText("중앙 공격 계수")]
        [SerializeField]
        float dmgRatio4_2;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")]
        [Tooltip("중앙 FlameWall의 크기 계수, 1.5 = 기본 FlameWall 기준 1.5배 크기")]
        [LabelText("중앙 스케일")]
        [SerializeField]
        float scale4;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")] [LabelText("불기둥 지속시간")] [SerializeField]
        float duration4;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴4/패턴", "4")] [LabelText("불기둥 준비시간")] [SerializeField]
        float ready4;

        #endregion

        #region 패턴 5

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")]
        [LabelText("순간이동 시간")]
        public float teleportTime5;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("태양 반경")]
        public float sunRadius;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("태양 생성시간")]
        public float sunCreateTime;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("투사체 생성반경")]
        public float projCreateRadius5;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("투사체 생성 도형 각 개수")]
        public int projDiagramCount5;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("주기당 투사체 생성 개수")]
        public int projCreateCount5PerTime;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("투사체 생성 주기")]
        public float projCreateFrequency5;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("투사체 생성 횟수")]
        public float projCreateCount5;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("투사체 설정")]
        public ProjectileInfo projInfo5;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("태양 이동속도")]
        public float sunSpeed;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("태양 폭발 크기")]
        public Vector2 sunExplodeSize;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴5/패턴", "5")] [LabelText("태양 폭발 데미지")]
        public float sunExplodeDmg;

        #endregion

        #region 패턴 6

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("첫번째 후방점프 Vector")] [SerializeField] 
        private Vector2 pattern6BackDash1; 
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("첫번째 후방점프 높이")] [SerializeField] 
        private float pattern6BackJumpPower1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("첫번째 후방점프 시간")] [SerializeField]
        private float pattern6DashTime1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("첫번째 후방점프Ease")] [SerializeField]
        private Ease pattern6BackDash1Ease;
        
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("두번째 후방점프 거리")] [SerializeField]
        private float pattern6BackDash2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")][LabelText("두번째 후방점프 높이")] [SerializeField] 
        private float pattern6BackJumpPower2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("두번째 후방점프 시간")] [SerializeField]
        private float pattern6DashTime2;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("두번째 후방점프Ease")] [SerializeField]
        private Ease pattern6BackDash2Ease;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("투사체 발사 방향 (속도)")]
        public List<Vector2> pattern6Projectiles = new();
        
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6")] [LabelText("투사체 설정")]
        public ProjectileInfo pattern6ProjectileInfo;
        
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("첫번째 후방점프 Vector")] [SerializeField]
        private Vector2 pattern6_2BackDash1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("첫번째 후방점프 높이")] [SerializeField] 
        private float pattern6_2BackJumpPower1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("첫번째 후방점프 시간")] [SerializeField]
        private float pattern6_2DashTime1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("첫번째 후방점프Ease")] [SerializeField]
        private Ease pattern6_2BackDash1Ease;
        
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("두번째 후방점프 거리")] [SerializeField]
        private float pattern6_2BackDash2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("두번째 후방점프 높이")] [SerializeField] 
        private float pattern6_2BackJumpPower2;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("두번째 후방점프 시간")] [SerializeField]
        private float pattern6_2DashTime2;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("두번째 후방점프Ease")] [SerializeField]
        private Ease pattern6_2BackDash2Ease;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("투사체 발사 방향 (속도)")]
        public List<Vector2> pattern6_2Projectiles = new();

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴6/패턴", "6_2")] [LabelText("투사체 설정")]
        public ProjectileInfo pattern6_2ProjectileInfo;
        #endregion

        #region 패턴 7

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("돌진 데미지")]
        public float rushDmg7;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("올려치기 데미지")]
        public float slashDmg7;
        [FormerlySerializedAs("minMove7")] [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("근접 최소 거리")]
        public float minMeleeMove7;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("이동 최소 거리")]
        public float minMove7;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("이동 최대 거리")]
        public float maxMove7;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("이동 속도")]
        public float moveSpeed7;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("이동 Ease")]
        public Ease moveEase7;
        
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("준비 이동 거리")]
        public float readyMove7Distance;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("준비 이동 시간")]
        public float readyMove7Time;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("준비 이동 Ease")]
        public Ease readyMove7Ease;
        
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("공격 근접 최소 거리")]
        public float secondMeleeMinMove7;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("공격 이동 최소 거리")]
        public float secondMinMove7;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("공격 이동 최대 거리")]
        public float secondMaxMove7;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("공격 이동 시간")]
        public float secondMoveSpeed7;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7")] [LabelText("공격 이동 Ease")]
        public Ease secondMoveEase7;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7_1")] [LabelText("공격 근접 최소 거리")]
        public float meleeMinMove7_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7_1")] [LabelText("공격 이동 최소 거리")]
        public float minMove7_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7_1")] [LabelText("공격 이동 최대 거리")]
        public float maxMove7_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7_1")] [LabelText("공격 이동 시간")]
        public float moveSpeed7_1;

        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7_1")] [LabelText("공격 이동 Ease")]
        public Ease moveEase7_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7_1")] [LabelText("회전 데미지")]
        public float firstDmg7_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴7/패턴", "7_1")] [LabelText("찍기 데미지")]
        public float secondDmg7_1;
        
        #endregion

        #region 패턴 8

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8/패턴", "8")] [LabelText("불기둥 생성 개수")] public int flameWallCount8; 
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8/패턴", "8")] [LabelText("불기둥 첫 생성 거리")] public float flameWallCreateDistance8; 
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8/패턴", "8")] [LabelText("불기둥 생성거리 간격")] public float flameWallPaddingDistance8; 
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8/패턴", "8")] [LabelText("불기둥 생성시간 간격")] public float flameWallPaddingTime8; 
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8/패턴", "8")] [LabelText("불기둥 준비시간")] public float flameWallReady8;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8/패턴", "8")] [LabelText("불기둥 지속시간")] public float flameWallDuration8;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8/패턴", "8")] [LabelText("불기둥 첫 데미지")] public float flameWallDmg;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴8/패턴", "8")] [LabelText("불기둥 틱 데미지")] public float flameWallDmg2;
        
        #endregion
        #region 패턴 9

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9/패턴", "9")] [LabelText("데미지")] public float dmg9;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9/패턴", "9")] [LabelText("텔레포트 근접거리")] public float teleportMinDist9;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9/패턴", "9")] [LabelText("텔레포트 시간")] public float teleportTime9;
        [FormerlySerializedAs("minMove9")] [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9/패턴", "9")] [LabelText("근접 최소 거리")] public float meleeMove9;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9/패턴", "9")] [LabelText("이동 최소 거리")] public float minMove9;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9/패턴", "9")] [LabelText("이동 최대 거리")] public float maxMove9;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9/패턴", "9")] [LabelText("이동 시간")] public float moveTime9;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴9/패턴", "9")] [LabelText("이동 Ease")] public Ease moveEase9;
        
        #endregion

        #region 패턴 10

        [FoldoutGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10")]
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10")] [LabelText("조준 시간")] public float aimTime10;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10")] [LabelText("화살 크기")] public float arrowScale10;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10")] [LabelText("두번째 조준 시간")] public float secondAimTime10;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10")] [LabelText("화살 투사체 설정")] public ProjectileInfo arrowInfo;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("텔레포트 x거리")] public float teleportXDistance10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("텔레포트 y거리")] public float teleportYDistance10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("화살 발사 개수")] public float arrowCount10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("화살 발사 각도차")] public float arrowAngle10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("화살 발사후 뒷점프 높이")] public float jumpHeight10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("화살 발사후 뒷점프 거리")] public float jumpDistance10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("화살 발사후 착지까지 시간")] public float jumpTime10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("불바다 크기")] public float flameGroundSize10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("불바다 공격설정")] public ProjectileInfo flameGroundInfo10_1;
        [TabGroup("기획쪽 수정 변수들/group1/패턴 관련/공격패턴10/패턴", "10_1")] [LabelText("불바다 지속시간")] public float flameGroundDuration10_1;
        
        #endregion
        #endregion

        #region States

        class JururuSummon1State : IState<BossMonster>
        {
            JururuBoss jururu;
            private Guid guid;

            public void FixedUpdate()
            {
            }

            public void OnEnter(BossMonster t)
            {
                t.CurGroggyGauge = 0;
                jururu = t as JururuBoss;
                if (jururu == null) return;

                jururu.Rb.DOKill();
                guid = jururu.AddInvincibility();
                jururu.ActorMovement.SetGravityToZero();
                jururu.EffectSpawner.SpawnAndDestroyAfterDuration(
                    Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuShieldAppear), "center",
                    () =>
                    {
                        jururu.EffectSpawner.Spawn(
                            Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuShieldLoop), "center",false);
                    },false);
            }

            public void OnExit()
            {
                if (jururu != null)
                {
                    jururu.RemoveInvincibility(guid);
                    jururu.ActorMovement.ResetGravity();
                    jururu.EffectSpawner.Spawn(
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuShieldDis), "center",false);
                    jururu.EffectSpawner.Remove(
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuShieldLoop));
                }
            }

            public void Update()
            {
            }
        }

        class JururuSummon2State : IState<BossMonster>
        {
            JururuBoss jururu;
            private Guid guid;

            public void FixedUpdate()
            {
            }

            public void OnEnter(BossMonster t)
            {
                t.CurGroggyGauge = 0;
                jururu = t as JururuBoss;
                if (jururu == null) return;

                jururu.Rb.DOKill();
                guid = jururu.AddInvincibility();
                jururu.ActorMovement.SetGravityToZero();
                jururu.EffectSpawner.SpawnAndDestroyAfterDuration(
                    Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuShieldAppear), "center",
                    () =>
                    {
                        jururu.EffectSpawner.Spawn(
                            Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuShieldLoop), "center",false);
                    },false);
            }

            public void OnExit()
            {
                if (jururu != null)
                {
                    jururu.RemoveInvincibility(guid);
                    jururu.ActorMovement.ResetGravity();
                    jururu.EffectSpawner.Spawn(
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuShieldDis), "center",false);
                    jururu.EffectSpawner.Remove(
                        Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuShieldLoop));
                }
            }

            public void Update()
            {
            }
        }

        #endregion

        int summon;

        public TextMeshPro stateText;

        protected override void Awake()
        {
            base.Awake();
            stateDict.Add(BossState.Summon1, new JururuSummon1State());
            stateDict.Add(BossState.Summon2, new JururuSummon2State());
            summon = 0;
            phase = BossPhase.Phase1;
            treeRunner.tree.blackboard.jururuBoss.pattern = Patterns.None;
            
            OnTeleportStart += () =>
                SpawnEffectInPositionButNoParent(Define.JururuBossEffect.EffectType.JururuAttack5Teleport);
            OnTeleportEnd += () =>
                SpawnEffectInPositionButNoParent(Define.JururuBossEffect.EffectType.JururuAttack5Teleport);
        }

        protected override void Start()
        {
            base.Start();

            colliderList.ForEach(x => { x.gameObject.SetActive(false); });
            
            OnTransformStart.AddListener(() =>
            {
                StopBGM(1f);
            });
            OnTransformEnd.AddListener(PlayPhase2BGM);
            OnTransformEnd.AddListener(() =>
            {
                ChangeSkin(BossPhase.Phase2);
                ForceRemoveInvincibility();
            });
        }

        protected override void SetAttackPatterns()
        {
            attackPatterns = new()
            {
                { 1, new AttackPattern1(this) },
                { 101, new AttackPattern1_1(this) },
                { 2, new AttackPattern2(this) },
                { 3, new AttackPattern3(this) },
                { 4, new AttackPattern4(this) },
                { 5, new AttackPattern5(this) },
                { 6, new AttackPattern6(this) },
                { 602, new AttackPattern6_2(this) },
                { 7, new AttackPattern7(this) },
                { 701, new AttackPattern7_1(this) },
                { 8, new AttackPattern8(this) },
                { 9, new AttackPattern9(this) },
                { 10, new AttackPattern10(this) },
                { 1001, new AttackPattern10_1(this) },
            };
        }

        public void StopBGM(float time)
        {
            GameManager.Sound.PauseArenaBGM(time);
        }

        public GameObject SpawnEffectInBone(Define.JururuBossEffect.EffectType type)
        {
            return EffectSpawner.Spawn(Define.JururuBossEffect.Get(type), "center",true).gameObject;
        }

        public GameObject SpawnEffectInPosition(Define.JururuBossEffect.EffectType type)
        {
            var effect = EffectSpawner.Spawn(Define.JururuBossEffect.Get(type), Position,true);
            return effect.gameObject;
        }

        public GameObject SpawnEffectInPositionButNoParent(Define.JururuBossEffect.EffectType type)
        {
            return EffectSpawner.Spawn(Define.JururuBossEffect.Get(type), Position,false).gameObject;
        }
        public void ReturnEffect(Define.JururuBossEffect.EffectType type)
        {
            EffectSpawner.Remove(Define.JururuBossEffect.Get(type));
        }

        public void PlayPhase2BGM()
        {
            GameManager.Sound.ChangeArenaClip(new List<string>()
            {
                Define.BGMList.JururuPhase2Intro,Define.BGMList.JururuPhase2Loop
            });
            GameManager.Sound.ResumeArenaBGM(1);
        }

        public void ChangeSkin(BossPhase p)
        {
            string skin = p == BossPhase.Phase1 ? "boss_jururu_phase1" : "boss_jururu_phase2";
            Mecanim.skeleton.SetSkin(skin);
        }

        JururuFlameBall SpawnProjectile(Vector2 position)
        {
            JururuFlameBall flameBall = GameManager.Factory.Get<JururuFlameBall>(
                FactoryManager.FactoryType.AttackObject,
                phase == BossPhase.Phase1
                    ? Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack3Throw)
                    : Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack3ThrowBlue)
                , position);

            string appearAddress = phase == BossPhase.Phase1
                ? Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack3Appear)
                : Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack3AppearBlue);

            EffectSpawner.Spawn(appearAddress, flameBall.Position,false);

            flameBall.transform.SetParent(BossAttacks.transform);

            return flameBall;
        }

        private GameObject _ep;

        private GameObject ep
        {
            get
            {
                if (_ep == null)
                {
                    _ep = new GameObject();
                    _ep.transform.SetParent(BossAttacks.transform);
                }

                return _ep;
            }
        }

        public override void CancelAttack()
        {
            base.CancelAttack();
            colliderList.ForEach(x => x.gameObject.SetActive(false));
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (state == BossState.Move && CurHp / MaxHp <= 0.5f && summon < 2)
            {
                SetState(BossState.Summon2);
                summon = 2;
            }
            else if (state == BossState.Move && CurHp / MaxHp <= 0.75f && summon == 0)
            {
                SetState(BossState.Summon1);
                summon = 1;
            }

            if (stateText != null)
            {
                stateText.text = GetComponent<BehaviourTreeRunner>().tree.blackboard.currentNodeName;
                stateText.transform.position = Position + Vector3.up * 1.5f;
            }
        }

        public void PullWhip() // 애니메이션에서 사용
        {
            GetComponentInChildren<JururuAttack2>().Pull();
        }

        public override void Die()
        {
            base.Die();

            SetState(BossState.Down);
        }
    }
}