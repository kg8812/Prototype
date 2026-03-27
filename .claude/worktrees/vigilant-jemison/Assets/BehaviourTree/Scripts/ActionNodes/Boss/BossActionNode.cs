using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public abstract class BossActionNode : ActionNode
    {
        [HideInInspector] public BossMonster boss;

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void SetActor(Actor actor)
        {
            base.SetActor(actor);

            boss = actor as BossMonster;
        }
    }
}