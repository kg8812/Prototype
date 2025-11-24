using UnityEngine;

public class PetFollower : MonoBehaviour
{
    [HideInInspector] public Transform followTrans;
    
    [HideInInspector] public Actor master;
    [HideInInspector] public bool moveOn;

    private Vector3 offset;
    private Actor actor;

    private void Awake()
    {
        actor = GetComponent<Actor>();
    }

    public void Init(Transform _followTrans, Actor _master = null, Vector2? _offset = null)
    {
        followTrans = _followTrans;
        master = _master;
        transform.position = followTrans.position + offset;
        moveOn = true;
        offset = _offset ?? Vector2.zero;
    }

    private Vector2 velocity;

    public void SetPosition()
    {
        Vector3 os = new Vector3(offset.x * -master.DirectionScale, offset.y);
        transform.position = followTrans.position + os;
    }
    private void Update()
    {
        if (!moveOn) return;
        if (followTrans == null) return;
        
        float distance = Vector2.Distance(transform.position, followTrans.position);
        Vector3 os = new Vector3(offset.x * -master.DirectionScale, offset.y);
        if (distance > 0.02f && distance < 7)
        {
            transform.position = Vector2.SmoothDamp(transform.position,
                followTrans.position + os, ref velocity, 0.1f, 30);
        }
        else
        {
            SetPosition();
        }

        if (ReferenceEquals(master, null)) return;

        if ((object)actor != null)
        {
            actor.SetDirection(master.Direction);
        }
        else
        {
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3((int)master.Direction * Mathf.Abs(scale.x), scale.y, scale.z);
        }
    }
}
