using UnityEngine;

public class CircleAround
{
    private IMonoBehaviour user;
    public float Degree;
    public readonly float radius;
    public readonly float speed;

    private readonly Transform transform;

    public enum Direction
    {
        ClockWise = 1 // 시계방향
        ,AntiClockWise = -1 // 반시계방향
    }

    private Direction dir;
    public bool lookCenter;
    
    public CircleAround(IMonoBehaviour center, Transform transform, float radius, float speed,Direction dir = Direction.ClockWise)
    {
        user = center;
        this.transform = transform;
        this.radius = radius;
        this.speed = speed;
        this.dir = dir;
        lookCenter = true;
    }

    public void Update()
    {
        Degree += Time.deltaTime * speed * (int)dir;

        if (Degree > 360) Degree = 0;

        var rad = Mathf.Deg2Rad * Degree;
        var x = radius * Mathf.Sin(rad);
        var y = radius * Mathf.Cos(rad);
        transform.position = user.Position + new Vector3(x, y);
        if (lookCenter)
        {
            transform.rotation = Quaternion.Euler(0, 0, Degree * -1 * (int)dir); //가운데를 바라보게 각도 조절
        }
    }
}