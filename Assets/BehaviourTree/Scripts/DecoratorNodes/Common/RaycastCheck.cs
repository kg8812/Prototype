using System.Collections.Generic;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class RaycastCheck : CommonDecoratorNode
    {
        public List<Vector2> fixedRayCast;
        public LayerMask _layerMask;

        public override void OnStart()
        {

        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (child.state == State.Running) return child.Update();

            foreach (var fVector2 in fixedRayCast)
            {
                Vector2 dir = new Vector2(fVector2.x * (int)_actor.Direction, fVector2.y);
                Debug.DrawRay(_actor.Position, dir, Color.red);
                RaycastHit2D hit = Physics2D.Raycast(_actor.Position, dir, fVector2.magnitude, _layerMask);
                if (hit)
                {
                    return child.Update();
                }
            }
            return State.Failure;
        }

        public override bool Check()
        {
            foreach (var fVector2 in fixedRayCast)
            {
                Vector2 dir = new Vector2(fVector2.x * (int)_actor.Direction, fVector2.y);
                Debug.DrawRay(_actor.Position, dir, Color.red);

                RaycastHit2D hit = Physics2D.Raycast(_actor.Position, dir, fVector2.magnitude, _layerMask);
                if (hit)
                {
                    return CheckChild;
                }
            }
            return false;
        }
    }
}