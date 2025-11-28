using System;
using Apis;
using Apis.StageObj;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scenes.Tutorial
{
    public class Elevator : MonoBehaviour, TriggeredObj, IOnInteract
    {
        [Tooltip("해당 엘리베이터가 두 개로 연동되어 작동하나?")] [SerializeField]
        private bool isMulti;

        [ShowIf("isMulti")] [SerializeField] private Elevator multiElevator;

        [Tooltip("해당 엘리베이터 안에서 작동시킬 수 있는 상태가 origin에 위치해있는가?(아니면 to)")] [ShowIf("isMulti")] [SerializeField]
        private bool isDefaultOrigin;

        [SerializeField] private Lever[] levers;

        [SerializeField] private Ease moveFunc = Ease.Linear;
        [SerializeField] private Collider2D interactionCol;

        [SerializeField] private Vector2 originPos;
        [SerializeField] private Vector2 toPos;
        [SerializeField] private Transform movingObj;
        [SerializeField] private float moveDuration;

        [Tooltip("레버의 오른쪽 활성화가 toPos로 가는 것인지(false : lever's right = origin")] [SerializeField]
        private bool leverRightIsTo;

        public GameObject Block;

        private float _allDist;
        private bool _isGoalTo;
        private Tween _tween;
        private bool isInteractable;

        private MovingObj moving;

        public bool IsInteractable
        {
            get => isInteractable;
            set
            {
                interactionCol.enabled = value;
                isInteractable = value;
            }
        }

        private void Awake()
        {
            _allDist = (toPos - originPos).magnitude;
            IsInteractable = true;
            moving = GetComponentInChildren<MovingObj>();
            InteractCheckEvent += Check;
        }

        public Func<bool> InteractCheckEvent { get; set; }

        public void OnInteract()
        {
            if (IsInteractable)
            {
                MoveTo(!_isGoalTo);
                if (levers == null) return;
                foreach (var lever in levers)
                    // 조금 복잡쓰
                    lever.MoveTo(_isGoalTo == leverRightIsTo);
            }
        }

        public void ChangeTrigger(int value)
        {
            if (value == 1)
                MoveTo(leverRightIsTo);
            else
                MoveTo(!leverRightIsTo);
        }

        private bool Check()
        {
            return IsInteractable;
        }

        public void ResetPos()
        {
            moving.RemoveAll();
            if (_tween != null && _tween.IsActive()) _tween.Kill();

            _isGoalTo = false;
            movingObj.transform.localPosition = originPos;
            IsInteractable = true;
            Block.SetActive(false);

            if (levers == null) return;
            foreach (var lever in levers)
                // 조금 복잡쓰
                lever.MoveTo(_isGoalTo == leverRightIsTo);
        }

        public void MoveTo(bool isTo, bool isForce = false)
        {
            IsInteractable = false;
            Block.SetActive(true);
            _isGoalTo = isTo;
            if (_tween != null && _tween.IsActive()) _tween.Kill();
            if (isForce)
            {
                movingObj.localPosition = isTo ? toPos : originPos;
                if (!isMulti || _isGoalTo != isDefaultOrigin) IsInteractable = true;
            }
            else
            {
                if (isMulti) multiElevator.MoveTo(_isGoalTo, true);
                // TODO: 나중에 추가하고 싶은 느낌
                // 엘베 끝날 때 덜컹 거리면서 움직임 멈춤 구현.
                // 나중에 톱니바퀴 움직임 animator도 연결해야함.


                // 현재 위치를 기반으로 위치 지점까지 갈 때 필요한 duration 계산
                var duration = moveDuration *
                               (((isTo ? toPos : originPos) - (Vector2)movingObj.localPosition).magnitude / _allDist);

                _tween = movingObj.DOLocalMove(isTo ? toPos : originPos, duration).SetEase(moveFunc).OnComplete(() =>
                {
                    if (!isMulti || _isGoalTo != isDefaultOrigin)
                    {
                        IsInteractable = true;
                        Block.SetActive(false);
                    }
                }).SetUpdate(UpdateType.Fixed);
            }
        }
    }
}