using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using UnityEngine;
using UnityEngine.Serialization;

public class BlackHole : MonoBehaviour
{
    public LayerMask targetLayer;
    
    List<Rigidbody2D> targets = new List<Rigidbody2D>();
    public float pullForce;
    public float maxSpeed;
    public Vector2 offset;
    
    private void Awake()
    {
        targets ??= new();
    }

    private void FixedUpdate()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null) 
            {
                targets.RemoveAt(i);
                continue;
            }
            
            Vector2 position = (Vector2)transform.position + offset;
            Vector2 direction = position - (Vector2)targets[i].transform.position;
            direction.Normalize();
            if (Vector2.Distance(position, transform.position) > 0.5f)
            {
                targets[i].linearVelocity = Vector2.Lerp(targets[i].linearVelocity, direction * maxSpeed,
                    Time.fixedDeltaTime * pullForce);
            }
        }
    }
    public bool CheckLayer(GameObject obj)
    {
        return (targetLayer.value & (1 << obj.layer)) > 0;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CheckLayer(other.gameObject) && other.TryGetComponent(out Rigidbody2D rb))
        {
            targets.Add(rb);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (CheckLayer(other.gameObject) && other.TryGetComponent(out Rigidbody2D rb))
        {
            targets.Remove(rb);
        }
    }

    private void OnDisable()
    {
        targets.Clear();
    }
}
