using System.Collections.Generic;

namespace Apis.BehaviourTreeTool
{
    public class IfBossState : BossDecoratorNode
    {
        public List<BossMonster.BossState> stateList = new();

        public override bool Check()
        {
            foreach (var bossState in stateList)
                if (bossState == boss.State)
                    return CheckChild;

            return false;
        }

        public override void OnStart()
        {
            foreach (var bossState in stateList)
                if (bossState == boss.State)
                    BehaviourTree.Traverse(this, node => { node.isStarted = false; });
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            foreach (var bossState in stateList)
                if (bossState == boss.State)
                {
                    if (child != null) return child.Update();

                    return State.Failure;
                }

            if (child != null) child.state = State.Failure;

            return State.Failure;
        }
    }
}