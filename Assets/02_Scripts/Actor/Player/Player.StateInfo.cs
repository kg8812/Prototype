using System.Collections.Generic;
using UnityEngine;

public partial class Player : Actor
{
    public IDictionary<EPlayerState, StateInfo> StateInfoDict
    {
        get; private set;
    } = new Dictionary<EPlayerState, StateInfo>();

    public void AddInfo(EPlayerState state, StateInfo info)
    {
        if(StateInfoDict.ContainsKey(state))
        {
            StateInfoDict[state] = info;
            return;
        }
        StateInfoDict.Add(state, info);
    }

    public void RemoveInfo(EPlayerState state)
    {
        if(!StateInfoDict.ContainsKey(state)) return;

        StateInfoDict.Remove(state);
    }

    public StateInfo GetInfo(EPlayerState state)
    {
        if(!StateInfoDict.ContainsKey(state)) return null;

        return StateInfoDict[state];
    }
}

public class StateInfo
{
    public int CutSceneID;
    public EventParameters eventParameters;
    public StateInfo()
    {

    }
}

public class CutsceneInfo : StateInfo
{
    public PlayerTimelineDummy Dummy;
}

public class ClimbInfo : StateInfo
{
    public Vector2 endPos;
    public EClimbType type;
    public Collider2D hit;
}
