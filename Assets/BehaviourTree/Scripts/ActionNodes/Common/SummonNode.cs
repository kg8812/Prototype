using System.Collections.Generic;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class SummonNode : CommonActionNode
    {
        [LabelText("소환 x위치 최소값")] public float xMin;
        [LabelText("소환 x위치 최대값")]public float xMax;
        public string address;

        private Dictionary<Actor, GameObject> chains = new();
        
        public override State OnUpdate()
        {
            GameObject obj = GameManager.Factory.Get(FactoryManager.FactoryType.Monster, address);
            if (obj == null) return State.Success;
            
            float x = _actor.transform.position.x +Random.Range(xMin, xMax);
            float y = _actor.GetFloorPos().y;
            obj.transform.position = new Vector3(x, y, 0);

            if (!obj.TryGetComponent(out Monster temp)) return State.Success;
            blackBoard.jururuBoss.summons.Add(temp);
            temp.AddEvent(EventType.OnDeath, Remove);
            temp.MoveToFloor();
            _actor.EffectSpawner.Spawn(
                Define.JururuBossEffect.Get(Define.JururuBossEffect.EffectType.JururuAttack5Teleport), temp.Position,false);
            
            TileSpriteEdgePosition tile =
                GameManager.Factory.Get<TileSpriteEdgePosition>(FactoryManager.FactoryType.Normal, "JururuSummonChain",
                    _actor.Position);
            tile.target = temp;
            chains.Add(temp,tile.gameObject);
            
            return State.Success;
        }
        
        void Remove(EventParameters x)
        {
            if (x.user is not Actor actor) return;
            
            blackBoard.jururuBoss.summons.Remove(actor);
            GameManager.Factory.Return(chains[actor]);
            x.user.EventManager.RemoveEvent(EventType.OnDeath,Remove);
            chains.Remove(actor);
        }
    }
}