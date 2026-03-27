using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Defaut;
using UnityEngine;

namespace GameStateSpace
{
    public class BattleState : GameState
    {
        private readonly Dictionary<StateCond, Guid> _guids;

        public BattleState()
        {
            _guids = new Dictionary<StateCond, Guid>();
            _guids.Add(StateCond.MonsterRecog, Guid.Empty);
            _guids.Add(StateCond.Arena, Guid.Empty);
            _guids.Add(StateCond.PlayerHit, Guid.Empty);
            _guids.Add(StateCond.PlayerDeffued, Guid.Empty);

            InitMonsterRecog();
            InitPlayerHit();
        }


        public override void OnEnterState()
        {
        }

        public override void OnExitState()
        {
        }

        public override void KeyBoardControlling()
        {
            base.KeyBoardControlling();
            GameManager.PlayerController?.KeyControl();
            GameManager.DefaultController?.KeyControl();
        }

        public override void GamePadControlling()
        {
            base.GamePadControlling();
            GameManager.PlayerController?.GamePadControl();
            GameManager.DefaultController?.GamePadControl();
        }

        #region condition on off

        private enum StateCond
        {
            MonsterRecog,
            Arena,
            PlayerHit,
            PlayerDeffued
        }

        private void TryOnWithCondition(StateCond condition)
        {
            _guids[condition] = GameManager.instance.TryOnGameState(GameStateType.BattleState);
        }

        private void TryOffWithCondition(StateCond condition)
        {
            if (_guids[condition] == Guid.Empty) return;
            GameManager.instance.TryOffGameState(GameStateType.BattleState, _guids[condition]);
            _guids[condition] = Guid.Empty;
        }

        #endregion


        /**
         * 진입 조건 목록 (or)
         * 1. 몬스터 인식
         * 2. 아레나 진입 - awake에서 관리
         * 3. 플레이어 피격 - player onhit 기준
         * 4. 일부 도트데미지 - player에 Debuff_DotDmg 존재 여부
         */

        #region 개별 조건들

        #region 몬스터 인식

        private List<Monster> _recogMonsterList;

        private void InitMonsterRecog()
        {
            _recogMonsterList = new List<Monster>();
        }

        public void AddRecogMonster(Monster mon)
        {
            if (!_recogMonsterList.Contains(mon))
            {
                void ExitRecog(EventParameters _)
                {
                    RemoveRecogMonster(mon);
                    mon.RemoveEvent(EventType.OnRecognitionExit, ExitRecog);
                    mon.RemoveEvent(EventType.OnDisable, ExitRecog);
                }

                mon.AddEvent(EventType.OnRecognitionExit, ExitRecog);
                mon.AddEvent(EventType.OnDisable, ExitRecog);
                _recogMonsterList.Add(mon);
                TryOnWithCondition(StateCond.MonsterRecog);
            }
        }

        public void RemoveRecogMonster(Monster mon)
        {
            if (_recogMonsterList.Contains(mon))
            {
                _recogMonsterList.Remove(mon);
                TryOnWithCondition(StateCond.MonsterRecog);
            }
        }

        #endregion


        #region 플레이어 피격

        private bool _isPlayerHit;
        private Coroutine _battleStateHitCoroutine;


        private void InitPlayerHit()
        {
            _isPlayerHit = false;
            _battleStateHitCoroutine = null;

            GameManager.instance.InitWithPlayer(p => { p.AddEvent(EventType.OnAfterHit, LastHit); });
        }

        private void LastHit(EventParameters eventParameters)
        {
            if (!_isPlayerHit)
            {
                _isPlayerHit = true;
                TryOnWithCondition(StateCond.PlayerHit);
            }

            if (_battleStateHitCoroutine != null) GameManager.instance.StopCoroutineWrapper(_battleStateHitCoroutine);
            _battleStateHitCoroutine = GameManager.instance.StartCoroutineWrapper(DelayLastHitForBattleState());
        }

        private IEnumerator DelayLastHitForBattleState()
        {
            yield return new WaitForSeconds(Consts.BattleStateHitDelay);
            if (_isPlayerHit)
            {
                _isPlayerHit = false;
                TryOffWithCondition(StateCond.PlayerHit);
            }
        }

        #endregion

        #endregion
    }
}