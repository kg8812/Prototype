using System.Collections;
using System.Collections.Generic;
using Apis;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData",menuName = "Scriptable/Datas/UnitData")]
public class UnitData : SerializedScriptableObject
{
    [FoldoutGroup("기획쪽 수정 변수들")]
    [TabGroup("기획쪽 수정 변수들/group1", "기본 스탯")]
    [HideLabel] public BaseStat baseStat;

    [FoldoutGroup("기획쪽 수정 변수들")] [TabGroup("기획쪽 수정 변수들/group1", "플레이어 스탯")]
    public PlayerStat playerStat;
    
    [LabelText("스파인 데이터")] public SkeletonDataAsset skeletonDataAsset;
    [LabelText("스파인 스킨이름")] public string initialSkinName;
    [LabelText("액티브 스킬")] public PlayerActiveSkill activeSkill;
    [LabelText("패시브 스킬")] public PlayerPassiveSkill passiveSkill;
    [LabelText("레벨당 영혼 증가량")] public float spirit;
    [LabelText("레벨당 신체 증가량")] public float body;
    [LabelText("레벨당 기량 증가량")] public float finesse;
}
