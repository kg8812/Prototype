using Apis;
using UnityEngine;

public abstract class CollideItem : DropItem
{
    private Collider2D _collider;
    private Collider2D Collider => _collider ??= GetComponent<Collider2D>();

    protected override void OnEnable()
    {
        base.OnEnable();
        Collider.enabled = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isInteractable && other.gameObject.CompareTag("Player"))
        {
            InvokeInteraction();
            isInteractable = false;
            GameManager.Factory.Return(gameObject);
        }
    }

    public override void Dropping()
    {
        base.Dropping();
        Collider.enabled = false;
    }

    protected override void Droped()
    {
        base.Droped();
        isInteractable = true;
        Collider.enabled = true;
    }
}