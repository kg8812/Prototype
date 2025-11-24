using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "TagData",menuName = "Scriptable/Datas/TagData")]
public class TagData : ScriptableObject
{
    [LabelText("깃털 공격력 증가량")][SerializeField] float _featherIncrement;
    [LabelText("주문 공격력 증가량")][SerializeField] float _spellIncrement;
    [LabelText("결속력 증가 수치")] [SerializeField] private float _ineCdIncrement;
    
    [LabelText("묘사 공격력 증가량")][SerializeField] float _depictionIncrement;
    [LabelText("채색 공격력 증가량")][SerializeField] float _coloringIncrement;
    [LabelText("장총 공격력 증가량")][SerializeField] float _rifleIncrement;
    [LabelText("사냥 회복 증가량")][SerializeField] float _huntIncrement;
    [LabelText("사냥 공격력 증가량")][SerializeField] float _huntDmgIncrement;
    
    [LabelText("혼령 공격력 증가량")][SerializeField] float _ghostIncrement;
    [LabelText("병사 공격력 증가량")][SerializeField] float _soldierIncrement;
    [LabelText("드론 공격력 증가량")][SerializeField] float _droneIncrement;
    [LabelText("메카 체력 증가량")][SerializeField] float _mechaIncrement1;
    [LabelText("메카 공격력 증가량")][SerializeField] float _mechaIncrement2;
    [LabelText("방패 수용량 증가량")][SerializeField] float _shieldIncrement;
    [LabelText("야수 공격력 증가량")][SerializeField] float _beastIncrement;
    
     public float FeatherIncrement =>  _featherIncrement;
     public float SpellIncrement =>  _spellIncrement;

     public float IneCdIncrement => _ineCdIncrement;
     public float DepictionIncrement =>  _depictionIncrement;
     public float ColoringIncrement =>  _coloringIncrement;
     public float RifleIncrement =>  _rifleIncrement;
     public float HuntIncrement =>  _huntIncrement;
     public float HuntDmgIncrement =>  _huntDmgIncrement;
     public float GhostIncrement =>  _ghostIncrement;
     public float SoldierIncrement =>  _soldierIncrement;
     public float DroneIncrement =>  _droneIncrement;
     public float MechaHpIncrement =>  _mechaIncrement1;
     public float MechaDmgIncrement =>  _mechaIncrement2;
     public float ShieldIncrement =>  _shieldIncrement;
     public float BeastIncrement =>  _beastIncrement;
}