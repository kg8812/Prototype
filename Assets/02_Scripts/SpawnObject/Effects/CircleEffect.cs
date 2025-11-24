using UnityEngine;

public class CircleEffect : MonoBehaviour
{
    public CircleAround move;

    public void Init(IMonoBehaviour actor,float radius,float speed)
    {
        move = new(actor, transform, radius, speed);
    }

    private void Update()
    {
        move.Update();
    }
}
