using Apis;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable/Datas/UnitData")]
public class UnitData : SerializedScriptableObject
{
    //캐릭터 데이터
    [FoldoutGroup("기획쪽 수정 변수들")] [TabGroup("기획쪽 수정 변수들/group1", "기본 스탯")] [HideLabel]
    public BaseStat baseStat;

    [FoldoutGroup("기획쪽 수정 변수들")] [TabGroup("기획쪽 수정 변수들/group1", "플레이어 스탯")]
    public PlayerStat playerStat;

    [LabelText("액티브 스킬")] public PlayerActiveSkill activeSkill;
    [LabelText("패시브 스킬")] public PlayerPassiveSkill passiveSkill;
}