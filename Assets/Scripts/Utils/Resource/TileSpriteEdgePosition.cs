using UnityEngine;

public class TileSpriteEdgePosition : MonoBehaviour
{
    public float maxDistance;
    private SpriteRenderer render;
    [HideInInspector] public IMonoBehaviour target;

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        var distance = Vector2.Distance(transform.position, target.Position);

        if (maxDistance > 0) distance = Mathf.Clamp(distance, 0, maxDistance);
        Vector2 dir = target.Position - transform.position;
        dir.Normalize();

        render.size = new Vector2(distance / transform.localScale.x, render.size.y);
        render.transform.right = -dir;
    }
}