using System.Collections;
using System.Collections.Generic;
using Apis.StageObj;
using Save.Schema;
using UnityEngine;

public class PermanentLever : Lever
{
    public int index;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        if (DataAccess.TaskData.IsDone(index))
        {
            MoveLever();
        }
    }

    public override void OnInteract()
    {
        base.OnInteract();
        DataAccess.TaskData.ActivateTask(index);
    }
}
