using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class TeleportToPos : BossActionNode
    {
        private bool isFinished;
        private Sequence seq;
        [LabelText("텔레포트 위치(오브젝트 이름)")] public string objName;

        [LabelText("텔레포트 시간")] [InfoBox("해당 시간동안 사라졌다가 나타납니다.")]
        public float duration;
        
        public override void OnStart()
        {     
            base.OnStart();
            isFinished = false;
            seq = boss.Teleport(objName, duration);
            seq.onKill += () =>
            {
                isFinished = true;
            };
        }
    
        public override void OnStop()
        {
        }
    
        public override State OnUpdate()
        {
            if(isFinished)
                return State.Success;

            return State.Running;
        }
    }
}