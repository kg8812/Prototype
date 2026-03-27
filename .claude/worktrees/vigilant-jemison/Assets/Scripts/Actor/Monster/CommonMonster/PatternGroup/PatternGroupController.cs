using System;
using System.Collections.Generic;
using Apis.Util;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Apis.CommonMonster2
{
    [Serializable]
    [ShowInInspector]
    public class PatternGroup : HasChance
    {
        public int id;

        public int pgChance;
        public float coolTime;
        public bool isLoop;

        public bool isCoolResetAt;
        public List<Pattern> patterns;
        private CommonMonster2 _monster;


        private float startTime;

        private float CoolTime
        {
            get
            {
                if (coolTime == 0) return 0;

                return coolTime;
            }
        }

        public int chance
        {
            get => pgChance;
            set => pgChance = value;
        }

        public void Init(CommonMonster2 monster)

        {
            _monster = monster;
        }

        public void UsePatternGroup()
        {
            startTime = Time.time;
        }

        public void CoolReset()
        {
            // 절대 cool 판단될 일 없음
            startTime = -(CoolTime + 1);
        }

        public bool IsNotCoolTime()
        {
            return startTime + CoolTime <= Time.time;
        }
    }

    public class PatternGroupController : MonoBehaviour
    {
        [ShowInInspector] public List<PatternGroup> _patternGroups;

        // 현재 상태
        [HideInInspector] public int curPGId;

        // 몬스터 액션에서 여러번 하는것들 있는데 그거 몇번했는지 체크하기 위함
        [HideInInspector] public int patternReturnCount;
        private List<PatternGroup> _availablePg;


        // for coding
        private List<int> _checkedPg;

        private bool _hasCheckCliff;

        private bool _isCliff;

        private bool _isInit;
        private bool _isOppositeDir;

        private CommonMonster2 _monster;
        private MonsterAction curAttack;
        private int curAttackId;
        private MonsterAction curMovement;
        private int curMovementId;
        private Pattern curPattern;
        private int curPatternId;
        private PatternGroup curPG;
        private bool isPlayingPattern;

        private bool isPlayingPG;


        private void Update()
        {
            if (isPlayingPattern && curPatternId != -1)
            {
                if (curPattern.attacks != null && curPattern.attacks.Count > curAttackId && curAttackId >= 0)
                    curPattern.attacks[curAttackId].Update();

                if (curPattern.movements != null && curPattern.movements.Count > curMovementId && curMovementId >= 0)
                    curPattern.movements[curMovementId].Update();
            }
        }

        // TODO: 체크 방식 수동적으로 바꾸기
        private void FixedUpdate()
        {
            if (isPlayingPattern && curPatternId != -1)
            {
                if (curPattern.attacks != null && curPattern.attacks.Count > curAttackId && curAttackId >= 0)
                    curPattern.attacks[curAttackId].FixedUpdate();

                if (curPattern.movements != null && curPattern.movements.Count > curMovementId && curMovementId >= 0)
                    curPattern.movements[curMovementId].FixedUpdate();
            }
        }

        public void InitCheck()
        {
            if (_isInit) return;
            Init();
        }

        private void Init()
        {
            _isInit = true;
            _monster = GetComponent<CommonMonster2>();
            _hasCheckCliff = false;
            foreach (var pG in _patternGroups)
            {
                pG.Init(_monster);
                pG.CoolReset();
                if (!pG.patterns[0].isCliffRun) _hasCheckCliff = true;
            }

            _availablePg = new List<PatternGroup>();
        }


        public bool PlayPatternGroup(int pGId)
        {
            if (isPlayingPG) return false;
            if (_patternGroups.Count >= pGId)
            {
                curPGId = pGId;
                curPG = _patternGroups[pGId - 1];
                curPatternId = -1;
                curPattern = null;
                curAttackId = -1;
                curMovementId = -1;
                patternReturnCount = 0;
                isPlayingPG = true;

                curPG.UsePatternGroup();

                return true;
            }

            return false;
        }

        public int SelectRandomPg(float playerDist)
        {
            _availablePg.Clear();
            _checkedPg = _monster.GetCheckedPatternGroupsWithCondition(playerDist);
            if (_checkedPg.Count == 0) return -1;
            _isCliff = false;
            if (_hasCheckCliff) _isCliff = _monster.ActorMovement.CheckCliff();

            _isOppositeDir = !_monster.CheckPlayerRl();

            foreach (var pGId in _checkedPg)
            {
                if (_patternGroups.Count < pGId) continue;
                var pg = _patternGroups[pGId - 1];
                if (!pg.IsNotCoolTime()) continue;
                if (_isCliff && !pg.patterns[0].isCliffRun) continue;
                if (!_monster.ableMove && ((_isOppositeDir && !pg.patterns[0].canAtkWithoutTurn) ||
                                           pg.patterns[0].haveMoving))
                    continue;

                _availablePg.Add(pg);
            }

            if (_availablePg.Count == 0) return -1;
            _availablePg = Utils.GetChanceList(_availablePg);
            var randId = Random.Range(0, _availablePg.Count);

            return _availablePg[randId].id;
        }


        public void PatternStarted(int fixedPattern = -1)
        {
            if (fixedPattern != -1)
                curPatternId = fixedPattern;
            else
                curPatternId += 1;


            if (curPG.patterns.Count <= curPatternId)
                Debug.LogError($"뭔가 벗어납니다. {curPG.id} {curPG.patterns.Count} {curPatternId}");

            curPattern = curPG.patterns[curPatternId];
            curAttackId = -1;
            curMovementId = -1;
            isPlayingPattern = true;
        }

        public void PatternEnded()
        {
            if (curAttackId > -1) curPattern.attacks?[curAttackId].OnEnd();
            if (curMovementId > -1) curPattern.movements?[curMovementId].OnEnd();
            var nextPatternInd = curPatternId + 1;
            if (nextPatternInd >= curPG.patterns.Count && curPG.isLoop) nextPatternInd = 0;

            if (nextPatternInd < curPG.patterns.Count)
            {
                if (!curPG.patterns[nextPatternInd].isCliffRun && _monster.ActorMovement.CheckCliff())
                {
                    _monster.animator.SetTrigger("forceDelay");
                }
                else
                {
                    if (!_monster.CheckPlayerRl()) _monster.TurnWithoutDelay();

                    PatternStarted(nextPatternInd);
                }
            }
            else
            {
                isPlayingPattern = false;
            }
        }

        public void ForceCancel()
        {
            if (isPlayingPattern) curPattern?.CancelPattern();

            ResetPGController();
        }

        public void ResetPGController()
        {
            isPlayingPattern = false;
            RepeatPatternGroup();
        }

        public void RepeatPatternGroup()
        {
            curPatternId = -1;
            curPattern = null;
            curAttackId = -1;
            curMovementId = -1;
        }

        // when delay ended
        public void EndPlayingPG()
        {
            isPlayingPG = false;
        }

        public void Attack()
        {
            if (curAttackId > -1) curPattern.attacks?[curAttackId].OnEnd();
            curAttackId += 1;
            if (curAttackId >= curPattern.attacks.Count)
                Debug.LogError(
                    $"{_monster.gameObject.name}: pattern의 attack count[{curPattern.attacks.Count}]를 넘어 [{curAttackId}]번째 attack을 시행하려고 함.");
            curPattern.attacks?[curAttackId].Action(_monster);
        }

        public void Movement()
        {
            if (curMovementId > -1) curPattern.movements?[curMovementId].OnEnd();
            curMovementId += 1;
            if (curMovementId >= curPattern.movements.Count)
                Debug.LogError(
                    $"{_monster.gameObject.name}: pattern의 movement count[{curPattern.movements.Count}]를 넘어 [{curMovementId}]번째 movement을 시행하려고 함.");
            curPattern.movements?[curMovementId].Action(_monster);
        }
    }
}