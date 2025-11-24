using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : ActorController
{
    private float dashAngle;

    public enum InputDirection
    {
        Up,Right,Down,Left,
        RightUp,LeftUp,RightDown,LeftDown
    }

    private InputDirection inputDir;
    public InputDirection InputDir => inputDir;
    
    public override void KeyControl()
    {
        base.KeyControl();
        
        
        #region 입력 방향 설정

        if (Input.GetKey(GetKeyCode(Define.GameKey.RightMove)) && Input.GetKey(GetKeyCode(Define.GameKey.Up)))
        {
            inputDir = InputDirection.RightUp;
        }
        else if (Input.GetKey(GetKeyCode(Define.GameKey.RightMove)) && Input.GetKey(GetKeyCode(Define.GameKey.Down)))
        {
            inputDir = InputDirection.RightDown;
        }
        else if (Input.GetKey(GetKeyCode(Define.GameKey.LeftMove)) && Input.GetKey(GetKeyCode(Define.GameKey.Up)))
        {
            inputDir = InputDirection.LeftUp;
        }
        else if (Input.GetKey(GetKeyCode(Define.GameKey.LeftMove)) && Input.GetKey(GetKeyCode(Define.GameKey.Down)))
        {
            inputDir = InputDirection.LeftDown;
        }
        else if (Input.GetKey(GetKeyCode(Define.GameKey.RightMove)))
        {
            inputDir = InputDirection.Right;
        }
        else if (Input.GetKey(GetKeyCode(Define.GameKey.LeftMove)))
        {
            inputDir = InputDirection.Left;
        }
        else if (Input.GetKey(GetKeyCode(Define.GameKey.Up)))
        {
            inputDir = InputDirection.Up;
        }
        else if (Input.GetKey(GetKeyCode(Define.GameKey.Down)))
        {
            inputDir = InputDirection.Down;
        }

        #endregion
    }

    public override void GamePadControl()
    {
        base.GamePadControl();
        
        #region 입력 방향 설정

        Vector2 moveInput = Gamepad.current.leftStick.ReadValue();

        float x = moveInput.x;
        float y = moveInput.y;

        if (x > 0.5f && y > 0.5f)
        {
            inputDir = InputDirection.RightUp;
        }
        else if (x < -0.5f && y < -0.5f)
        {
            inputDir = InputDirection.LeftDown;
        }
        else if (x > 0.5f && y < -0.5f)
        {
            inputDir = InputDirection.RightDown;
        }
        else if (x < -0.5f && y > 0.5f)
        {
            inputDir = InputDirection.LeftUp;
        }
        else if (x > 0.5f)
        {
            inputDir = InputDirection.Right;
        }
        else if (x < -0.5f)
        {
            inputDir = InputDirection.Left;
        }
        else if (y > 0.5f)
        {
            inputDir = InputDirection.Up;
        }
        else if (y < -0.5f)
        {
            inputDir = InputDirection.Down;
        }
        #endregion
    }
}
