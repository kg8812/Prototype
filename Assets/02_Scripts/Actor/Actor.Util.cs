using System;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Actor
{
    private Guid _guid;
    [Button("무적 On/Off")]
    public void Invincibility(bool on)
    {
        if (on) _guid = AddInvincibility();
        else RemoveInvincibility(_guid);
    }
    
    public void MoveToFloor()
    {
        if (Utils.GetLowestPointByRay(Position, LayerMasks.GroundAndPlatform, out var value))
        {
            transform.position = value;
        }
    }
}