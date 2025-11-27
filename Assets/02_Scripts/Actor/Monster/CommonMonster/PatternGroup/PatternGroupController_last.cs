// using System.Collections.Generic;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace Apis
// {
//     
//
//     public class PatternGroupController2 : MonoBehaviour
//     {
//         [SerializeField] private List<PatternGroup> patternGroupsList;
//         private Dictionary<int, PatternGroup> _patternGroups;
//
//         private Pattern _curPattern;
//         private CommonMonster2.CommonMonster2 _monster;
//
//         // 현재 패턴그룹을 실행중인가? (delay 포함)
//         private bool isPlayingPG = false;
//
//         // 현재 패턴 애니메이션을 재생중인가? (exept-ready,end)
//         private bool isPlayingPatternInPG = false;
//         [HideInInspector] public PatternGroup curPatternGroup;
//         private int curPattern = 0;
//         private int curAttack = 0;
//         private int curMovement = 0;
//
//         private List<int> checkedPG;
//         private List<PatternGroup> availablePG;
//
//         private bool isCheckCliffWhenGetRandomPatternGroup;
//
//         private void Awake()
//         {
//             _monster = GetComponent<CommonMonster2.CommonMonster2>();
//             _patternGroups = new();
//             isCheckCliffWhenGetRandomPatternGroup = false;
//             foreach (var pG in patternGroupsList)
//             {
//                 _patternGroups.Add(pG.id, pG);
//                 pG.Init(_monster);
//                 pG.CoolReset();
//                 if (!pG.patterns[0].isCliffRun)
//                 {
//                     isCheckCliffWhenGetRandomPatternGroup = true;
//                 }
//             }
//
//             availablePG = new List<PatternGroup>();
//         }
//
//
//         public bool PlayPatternGroup(int patternGroupId)
//         {
//             if (isPlayingPG)
//             {
//                 Debug.Log("is playing PG");
//                 return false;
//             }
//
//             if (_patternGroups.TryGetValue(patternGroupId, out var value))
//             {
//                 isPlayingPG = true;
//                 // 해당 패턴 그룹 실행
//                 curPatternGroup = value;
//                 curPattern = -1;
//                 curAttack = -1;
//                 curMovement = -1;
//
//                 curPatternGroup.UsePatternGroup();
//
//                 return true;
//             }
//
//             return false;
//         }
//
//         public void ResetWhenStart()
//         {
//             foreach (var value in _patternGroups)
//             {
//                 if (value.Value.isCoolResetAt)
//                 {
//                     value.Value.CoolReset();
//                 }
//             }
//         }
//
//
//         public int SelectRandomPatternGroup(float playerDist)
//         {
//             availablePG.Clear();
//             checkedPG = _monster.GetCheckedPatternGroupsWithCondition(playerDist);
//             if (checkedPG.Count == 0) return -1;
//             bool isCliff = false;
//             if (isCheckCliffWhenGetRandomPatternGroup)
//             {
//                 isCliff = _monster.ActorMovement.CheckCliff();
//             }
//             // TODO: actor movement 추가 요청
//             // TODO: cliff 여부 체크하고 
//             // bool isCliff = _monster.actorMovement.IsCliff();
//
//             foreach (var pGId in checkedPG)
//             {
//                 if (_patternGroups.TryGetValue(pGId, out var value))
//                 {
//                     if (value.IsNotCoolTime())
//                     {
//                         // cliff가 아니거나 해당 패턴그룹의 첫번째 패턴그룹이 cliff일때도 시행하면 가능한 패턴그룹으로 추가하기
//                         if (!isCliff || value.patterns[0].isCliffRun)
//                         {
//                             availablePG.Add(value);
//                         }
//                     }
//                     else
//                     {
//                     }
//                 }
//             }
//
//             if (availablePG.Count == 0) return -1;
//             availablePG = Default.Utils.GetChanceList<PatternGroup>(availablePG);
//             int randId = Random.Range(0, availablePG.Count);
//
//             return availablePG[randId].id;
//         }
//
//         // 
//         // 공격 후, 움직임 후, 패턴 애니메이션 전 후 체크
//         // attack 시작에 넣어줘야 함. <= attack update 판별해야 하는데 attack_start는 빼주기 위함
//
//         /*
//          public bool CheckPattern()
//         {
//             // 해당 패턴의 마지막 공격 and 마지막 움직임 인가?
//             if (curPattern == -1)
//             {
//                 curPattern = 0;
//                 _curPattern = curPatternGroup.patterns[curPattern];
//                 curAttack = 0;
//                 curMovement = 0;
//                 return true;
//             }
//
//             if (_curPattern.attacks.Count == curAttack && _curPattern.movements.Count == curMovement)
//             {
//                 curAttack = 0;
//                 curMovement = 0;
//                 curPattern += 1;
//
//                 // 마지막 패턴인가?
//                 if (curPatternGroup.patterns.Count <= curPattern)
//                 {
//                     curPattern = -1;
//                     isPlayingPG = false;
//                     _curPattern = null;
//                     return false;
//                 }
//                 else
//                 {
//                     _curPattern = curPatternGroup.patterns[curPattern];
//                     if (!_curPattern.isCliffRun)
//                     {
//                         if (_monster.actorMovement.CheckCliff())
//                         {
//                             // TODO: 해당 패턴그룹 시행을 멈추고 delay로 돌아가기
//                         }
//                     }
//
//                     if (_monster.CheckPlayerRL())
//                     {
//                         _monster.TurnWithoutDelay();
//                     }
//                 }
//             }
//
//             return true;
//         }
//         */
//
//
//         public void PatternStarted()
//         {
//             Debug.Log("Pattern started");
//             isPlayingPatternInPG = true;
//             curPattern += 1;
//             _curPattern = curPatternGroup.patterns[curPattern];
//             curAttack = -1;
//             curMovement = -1;
//         }
//
//         public void PatternEnded()
//         {
//             int nextPatternInd = curPattern + 1;
//             if (nextPatternInd < curPatternGroup.patterns.Count)
//             {
//                 if (curPatternGroup.patterns[nextPatternInd].isCliffRun)
//                 {
//                     if (_monster.ActorMovement.CheckCliff())
//                     {
//                         // TODO: 해당 패턴그룹 시행을 멈추고 delay로 돌아가기
//                     }
//                 }
//                 else if (!_monster.CheckPlayerRL())
//                 {
//                     Debug.Log("패턴 사이에 회전하기");
//                     _monster.TurnWithoutDelay();
//                 }
//                 else
//                 {
//                     PatternStarted();
//                 }
//             }
//             else
//             {
//                 ResetPGController();
//             }
//         }
//
//         public void ResetPGController()
//         {
//             isPlayingPatternInPG = false;
//             curPattern = -1;
//             _curPattern = null;
//             curAttack = -1;
//             curMovement = -1;
//             isPlayingPG = false;
//         }
//
//
//         // TODO: Update라 나중에 최적화 해야함.
//         private void Update()
//         {
//             if (curPattern != -1 && isPlayingPatternInPG)
//             {
//                 if (_curPattern.attacks.Count <= curAttack || curAttack < 0) return;
//                 _curPattern.attacks?[curAttack].Update();
//             }
//         }
//
//         public void Attack()
//         {
//             curAttack += 1;
//             _curPattern.attacks?[curAttack].Action(_monster);
//         }
//
//         public void Movement()
//         {
//             curMovement += 1;
//             _curPattern.movements?[curMovement].Action(_monster);
//         }
//
//     }
// }

