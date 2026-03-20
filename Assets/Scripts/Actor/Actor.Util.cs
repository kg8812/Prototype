using System;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public partial class Actor : IImmunity
{
    private Guid _guid;
    private UnityEvent _onAppear;
    private UnityEvent _onHide;

    public UnityEvent OnHide => _onHide ??= new UnityEvent();
    public UnityEvent OnAppear => _onAppear ??= new UnityEvent();

    private ActorImmunity _actorImmunity;
    public ActorImmunity ActorImmunity => _actorImmunity ??= new ActorImmunity();

    public ImmunityController ImmunityController => ActorImmunity.Controller;
    public bool HitImmune => ActorImmunity.IsHitImmune;

    public bool IsInvincible => ActorImmunity.IsInvincible;

    public Guid AddHitImmunity()
    {
        return ActorImmunity.AddHitImmunity();
    }

    public void RemoveHitImmunity(Guid guid)
    {
        ActorImmunity.RemoveHitImmunity(guid);
    }

    public Guid AddInvincibility()
    {
        return ActorImmunity.AddInvincible();
    }

    public void RemoveInvincibility(Guid guid)
    {
        ActorImmunity.RemoveInvincible(guid);
    }

    public void ForceRemoveHitImmunity()
    {
        ActorImmunity.ClearHitImmunity();
    }

    [Button("무적 On/Off")]
    public void Invincibility(bool on)
    {
        if (on) _guid = AddInvincibility();
        else RemoveInvincibility(_guid);
    }

    public void MoveToFloor()
    {
        if (Utils.GetLowestPointByRay(Position, LayerMasks.GroundAndPlatform, out var value))
            transform.position = value;
    }

    public void Hide()
    {
        actorRenderer.Hide();

        OnHide.Invoke();
    }

    public void Appear()
    {
        actorRenderer.Appear();
        OnAppear.Invoke();
    }
}