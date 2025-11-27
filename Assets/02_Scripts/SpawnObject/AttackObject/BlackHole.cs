using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public LayerMask targetLayer;
    public float pullForce;
    public float maxSpeed;
    public Vector2 offset;

    private List<Rigidbody2D> targets = new();

    private void Awake()
    {
        targets ??= new List<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        for (var i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null)
            {
                targets.RemoveAt(i);
                continue;
            }

            var position = (Vector2)transform.position + offset;
            var direction = position - (Vector2)targets[i].transform.position;
            direction.Normalize();
            if (Vector2.Distance(position, transform.position) > 0.5f)
                targets[i].linearVelocity = Vector2.Lerp(targets[i].linearVelocity, direction * maxSpeed,
                    Time.fixedDeltaTime * pullForce);
        }
    }

    private void OnDisable()
    {
        targets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CheckLayer(other.gameObject) && other.TryGetComponent(out Rigidbody2D rb)) targets.Add(rb);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CheckLayer(other.gameObject) && other.TryGetComponent(out Rigidbody2D rb)) targets.Remove(rb);
    }

    public bool CheckLayer(GameObject obj)
    {
        return (targetLayer.value & (1 << obj.layer)) > 0;
    }
}