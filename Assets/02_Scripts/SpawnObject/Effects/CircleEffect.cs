using UnityEngine;

public class CircleEffect : MonoBehaviour
{
    public CircleAround move;

    private void Update()
    {
        move.Update();
    }

    public void Init(IMonoBehaviour actor, float radius, float speed)
    {
        move = new CircleAround(actor, transform, radius, speed);
    }
}