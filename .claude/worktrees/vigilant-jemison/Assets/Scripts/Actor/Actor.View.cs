using Apis;
using Default;
using UnityEngine;
using UnityEngine.Events;

public partial class Actor
{
    private UnityEvent _onAppear;
    private UnityEvent _onHide;

    public UnityEvent OnHide => _onHide ??= new UnityEvent();
    public UnityEvent OnAppear => _onAppear ??= new UnityEvent();
    
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
    
    public void MoveToFloor()
    {
        if (Utils.GetLowestPointByRay(Position, LayerMasks.GroundAndPlatform, out var value))
            transform.position = value;
    }

}
