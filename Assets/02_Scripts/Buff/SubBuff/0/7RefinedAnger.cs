using UnityEngine;

namespace Apis
{
    public class RefinedAnger : Debuff_base
    {
        public RefinedAnger(Buff buff) : base(buff)
        {
        }

        private CircleEffect effect;
        private Transform parent;
        
        public override void OnAdd()
        {
            base.OnAdd();
            parent = actor.transform.Find("RefineAngerEffects");
            if (parent == null)
            {
                parent = new GameObject("RefineAngerEffects").transform;
                parent.transform.SetParent(actor.transform);
            }
            
            effect = GameManager.Factory.Get<CircleEffect>(FactoryManager.FactoryType.Normal, "AngerEffect");
            effect.transform.SetParent(parent.transform);
            effect.Init(actor, 1, 50);

            var list = parent.transform.GetComponentsInChildren<CircleEffect>();
            
            for (int i = 0; i < list.Length; i++)
            {
                list[i].move.Degree = 360f / list.Length * i;
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            GameManager.Factory.Return(effect.gameObject);
            
            var list = parent.transform.GetComponentsInChildren<CircleEffect>();
            
            if (list.Length == 0)
            {
                if (parent != null)
                {
                    Object.Destroy(parent.gameObject);
                }
                return;
            }
            
            for (int i = 0; i < list.Length; i++)
            {
                list[i].move.Degree = 360f / list.Length * i;
            }

            
        }

        public override SubBuffType Type => SubBuffType.RefinedAnger;
    }
}