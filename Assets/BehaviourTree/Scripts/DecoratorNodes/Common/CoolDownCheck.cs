using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class CoolDownCheck : CommonDecoratorNode
    {
        float cooldown;
        [InfoBox("쿨타임은 매번 최소, 최대중 랜덤으로 적용")]
        [PropertySpace(30)]
        [LabelText("최소 쿨타임")] public float cdMin;
        [LabelText("최대 쿨타임")] public float cdMax;

        float startTime = -1;
        [LabelText("시작 쿨타임 적용여부")] public bool startCD;
        
        public override bool Check()
        {
            if (startTime < 0 || startTime + cooldown < Time.time)
            {
                return CheckChild;
            }

            return false;
        }

        public override void OnStart()
        {
        }

        public override void Init()
        {
            base.Init();

            if (startCD)
            {
                startTime = Time.time;
            }

            cooldown = Random.Range(cdMin, cdMax);
        }
        public override void OnStop()
        {
            cooldown = Random.Range(cdMin, cdMax);
        }

        public override State OnUpdate()
        {
            
            if (startTime < 0 || startTime + cooldown < Time.time)
            {
                startTime = Time.time;
                return child.Update();
            }
            if (child.state == State.Running)
            {
                return child.Update();
            }

            return State.Failure;
        }
    }
}