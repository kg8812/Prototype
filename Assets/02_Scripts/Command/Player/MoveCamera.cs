using Managers;
using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "MoveCam", menuName = "ActorCommand/Player/MoveCam")]
    public class MoveCamera : PlayerCommand
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public bool Move;
        public Direction direction;

        protected override void Invoke(Player go)
        {
            switch (direction)
            {
                case Direction.Left:
                    CameraManager.instance.playerCamX = Move ? -1 : 0;
                    break;
                case Direction.Right:
                    CameraManager.instance.playerCamX = Move ? 1 : 0;
                    break;
                case Direction.Up:
                    CameraManager.instance.playerCamY = Move ? 1 : 0;
                    break;
                case Direction.Down:
                    CameraManager.instance.playerCamY = Move ? -1 : 0;
                    break;
            }
        }

        public override bool InvokeCondition(Player go)
        {
            return true;
        }
    }
}