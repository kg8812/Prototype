using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    enum Direction
    {
        ClockWise = 1,AntiClockWise = -1
    }

    [LabelText("이동방향")][SerializeField] private Direction direction;
    [LabelText("이동속도 (m/s)")][SerializeField] private float speed;
    [SerializeField] public LayerMask layers;
    private List<Transform> moves = new();

    private void Awake()
    {
        moves ??= new();
    }

    private void FixedUpdate()
    {
        if (moves.Count != 0)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                moves[i].Translate(transform.right * (speed * Time.fixedDeltaTime * (int)direction));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0)
        {
            Transform target = other.transform.parent != null ? other.transform.parent : other.transform;
            
            if (!moves.Contains(target))
            {
                moves.Add(target);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0)
        {
            Transform target = other.transform.parent != null ? other.transform.parent : other.transform;
            
            if (moves.Contains(target))
            {
                moves.Remove(target);
            }
        }
    }
}
