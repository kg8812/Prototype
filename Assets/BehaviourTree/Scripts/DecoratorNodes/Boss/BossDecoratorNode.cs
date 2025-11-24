using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public abstract class BossDecoratorNode : DecoratorNode
    {
        [HideInInspector] public BossMonster boss;

        public override void SetActor(Actor actor)
        {
            base.SetActor(actor);

            boss = actor as BossMonster;
        }
    }
}