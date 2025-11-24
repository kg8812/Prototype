using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.DataType
{
    [System.Serializable]
    [FoldoutGroup("기획쪽 수정 변수들")]
    [TabGroup("기획쪽 수정 변수들/group1","몬스터 설정")]
    [HideLabel]
    public class MonsterDataType
    {
        [HideInInspector] public int monsterId;

        [JsonConverter(typeof(StringEnumConverter))]
        [LabelText("몬스터 타입")] public MonsterType monsterType;

        // public string monsterName;
        [HideInInspector] public float maxHp;
        [HideInInspector] public float atkPower;
        [HideInInspector] public float moveSpeed;
        [LabelText("최대 그로기 게이지")] public float groggyGauge;
        [LabelText("초당 그로기 회복량")]public float groggyRecovery;
        [LabelText("delay state 지속시간")] [Tooltip("n, m random으로 결정")]public float[] delayDuration;
        [LabelText("patrol state 존재 여부")] public bool isPatrol;
        [LabelText("움직임 여부")] public bool isMove;
        [LabelText("비행 몬스터 여부")] public bool isFlying;
        [LabelText("patrol state 지속시간")] [Tooltip("n, m random으로 결정")] public float[] patrolDuration;
        [LabelText("드롭아이템 index")] public int dropIndex;
        public int dropExp;
        
        public MonsterDataType(int monsterId, 
            MonsterType monsterType, 
            string monsterName, 
            float maxHp,
            float atkPower,
            float moveSpeed,
            float groggyGauge,
            float groggyRecovery,
            float[] delayDuration,
            bool isPatrol,
            bool isMove,
            bool isFlying,
            float[] patrolDuration,
            int dropIndex,int dropExp)
        {
            this.monsterId = monsterId;
            this.monsterType = monsterType;
            // this.monsterName = monsterName;
            this.maxHp = maxHp;
            this.atkPower = atkPower;
            this.moveSpeed = moveSpeed;
            this.groggyGauge = groggyGauge;
            this.groggyRecovery = groggyRecovery;
            this.delayDuration = delayDuration;
            this.isPatrol = isPatrol;
            this.isMove = isMove;
            this.isFlying = isFlying;
            this.patrolDuration = patrolDuration;
            this.dropIndex = dropIndex;
            this.dropExp = dropExp;
        }
    }
}