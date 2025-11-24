using DG.Tweening;
using Default;
using UnityEngine;

namespace Apis
{
    public class HunterStack : Debuff_base
    {
        
        public HunterStack(Buff buff) : base(buff)
        {
        }

        private SpriteRenderer effect;
        private int stack;

        public override void OnAdd()
        {
            base.OnAdd();
            
            Transform eff = actor.transform.Find("HunterStackEffect");

            if (eff == null)
            {
                eff = GameManager.Factory.Get(FactoryManager.FactoryType.Normal, "HunterStackEffect", actor.topPivot)
                    .transform;
                SpineUtils.AddCustomBoneFollower(actor.Mecanim,"center",eff.gameObject);
                CustomBoneFollower follower = Utils.GetOrAddComponent<CustomBoneFollower>(eff.gameObject);
                follower.offset = actor.topPivot;
            }

            eff.SetParent(actor.transform);
            effect = eff.GetComponent<SpriteRenderer>();

            stack = actor.SubBuffCount(Type);
            Sprite sprite = ResourceUtil.Load<Sprite>("Sign_hunterSign_" + stack);
            
            if (effect.sprite != null)
            {
                ResourceUtil.Release(effect.sprite);
            }
            
            effect.sprite = sprite;
            BuffDatabase.DataLoad.TryGetSubBuffIndex(Type, out int index);
            BuffDatabase.DataLoad.TryGetSubBuffOption(index, out var option);
            
            if(stack < option.maxStack)
            {
                GameManager.Sound.Play("lilpaSkill_addsign");
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
           
            stack = actor.SubBuffCount(Type);
            if (stack > 0)
            {
                effect.sprite = ResourceUtil.Load<Sprite>("Sign_hunterSign_" + stack);
            }
            else
            {
                GameManager.Factory.Return(effect.gameObject);
            }
        }

        public override void OnMaxStack()
        {
            base.OnMaxStack();
            string sfx = "lilpaSkill_allsign";
            GameManager.Sound.Play(sfx);
        }

        public override SubBuffType Type => SubBuffType.HunterStack;
    }
}