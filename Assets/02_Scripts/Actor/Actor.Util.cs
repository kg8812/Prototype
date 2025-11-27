using System;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public partial class Actor
{
    private Guid _guid;
    private UnityEvent _onAppear;

    private UnityEvent _onHide;
    public UnityEvent OnHide => _onHide ??= new UnityEvent();
    public UnityEvent OnAppear => _onAppear ??= new UnityEvent();
    public bool HitImmune => ImmunityController.IsImmune("HitImmunity");

    public Guid AddHitImmunity()
    {
        if (!ImmunityController.Contains("HitImmunity")) ImmunityController.MakeNewType("HitImmunity");
        return ImmunityController.AddCount("HitImmunity");
    }

    public void RemoveHitImmunity(Guid guid)
    {
        ImmunityController.MinusCount("HitImmunity", guid);
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

    public void ForceRemoveHitImmunity()
    {
        ImmunityController.MakeCountToZero("HitImmunity");
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