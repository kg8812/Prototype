using System.Collections;
using UnityEngine;

namespace Apis
{
    public class Buff_LostHpRegen : SubBuff
    {
        Coroutine coroutine;

        public Buff_LostHpRegen(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_LostHpRegen;
        public override void OnAdd()
        {
            base.OnAdd();
            coroutine = GameManager.instance.StartCoroutine(HpRegen());
            
        }

        public override void OnRemove()
        {
            base.OnRemove();
            GameManager.instance.StopCoroutine(coroutine);
        }
        public override void PermanentApply()
        {
            base.PermanentApply();
            GameManager.instance.StartCoroutine(HpRegen());
        }
        IEnumerator HpRegen()
        {
            float time = duration;
            float curTime = 0;
            
            float healAmount = (actor.MaxHp - actor.CurHp) / 100 * amount[0];
            if (time == 0)
            {
                actor.CurHp += healAmount;
                yield break;
            }
            float a = healAmount * Time.deltaTime / time;

            while (curTime < time)
            {
                actor.CurHp += a;
                curTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            actor.RemoveSubBuff(buff, this);
        }
    }
}