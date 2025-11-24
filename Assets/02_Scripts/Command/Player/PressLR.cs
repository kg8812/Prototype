using System;
using UnityEngine;
namespace Command
{
    [CreateAssetMenu(fileName = "PressLR", menuName = "ActorCommand/Player/PressLR", order = 1)]
    public class PressLR : PlayerCommand
    {
        [SerializeField] private EActorDirection direction; // 좌우 확인
        [SerializeField] private bool onKeyUp;

        protected override void Invoke(Player go)
        {
            go.PressLR(direction, !onKeyUp);
        }

        public override bool InvokeCondition(Player go)
        {
            return true;
        }
    }
}