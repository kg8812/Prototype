using System.Collections.Generic;
using Apis;
using Default;
using UnityEngine;


namespace Apis
{
    public class Debuff_Tied : Debuff_base
    {
        public override SubBuffType Type => SubBuffType.Debuff_Tied;

        public Debuff_Tied(Buff buff) : base(buff)
        {
        }

        private float curTime;
        private GameObject effect;

        public override void OnAdd()
        {
            base.OnAdd();
            curTime = 0;
            effect = GameManager.Factory.Get(FactoryManager.FactoryType.Effect, Define.PlayerEffect.ViichanThornForm,
                actor.Position);
            SpineUtils.AddBoneFollower(actor.Mecanim, "center", effect);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            GameManager.Factory.Return(effect);
        }

        public override void Update()
        {
            base.Update();
            if (curTime < 1)
            {
                curTime += Time.deltaTime;
            }
            else
            {
                curTime = 0;

                List<IOnHit> actors = actor.gameObject.GetTargetsInCircle(amount[1], LayerMasks.Enemy);
                actors.ForEach(x =>
                {
                    x.OnHit(new EventParameters(Utils.GetComponentInParentAndChild<IEventUser>(target))
                    {
                        atkData = new(){dmg = amount[0],groggyAmount = Mathf.RoundToInt(amount[2])},
                    });
                });
            }
        }
    }
}