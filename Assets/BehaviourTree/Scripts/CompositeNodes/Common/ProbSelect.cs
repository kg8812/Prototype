using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class ProbSelect : CommonCompositeNode
    {
        public List<int> probs = new();
        private int index;

        public override void OnStart()
        {
            base.OnStart();
            index = -1;
            var list = probs.Select((_, index) => index).Where(x =>
            {
                if (x >= children.Count) return false;

                if (children[x] is DecoratorNode dec) return dec.Check();
                return true;
            }).ToList();

            List<int> probList = new();

            list.ForEach(x =>
            {
                for (var i = 0; i < probs[x]; i++) probList.Add(x);
            });

            if (probList.Count > 0) index = probList[Random.Range(0, probList.Count)];
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (index >= 0)
                switch (children[index].Update())
                {
                    case State.Success:
                        return State.Success;

                    case State.Failure:
                        return State.Failure;

                    case State.Running:
                        return State.Running;
                }

            return State.Failure;
        }
    }
}