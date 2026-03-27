using Sirenix.OdinInspector;
using UnityEngine;

public class RotatingEffect : MonoBehaviour
{
    [LabelText("회전속도 (angle/s)")] public float speed;
    [LabelText("회전 줄어드는 속도 (angle/s)")] public float decreaseSpeed;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
        if (speed > 0)
            speed -= decreaseSpeed * Time.deltaTime;
        else
            speed = 0;
    }
}