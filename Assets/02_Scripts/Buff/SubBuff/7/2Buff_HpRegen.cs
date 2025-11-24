using System.Collections;
using UnityEngine;

namespace Apis
{
    public class Buff_HpRegen : SubBuff
    {
        public Buff_HpRegen(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_HpRegen;


        Coroutine coroutine;
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
            if (time == 0)
            {
                actor.CurHp += amount[0];
                yield break;
            }
            float f = amount[0] * Time.deltaTime / time;

            while (curTime < time)
            {
                actor.CurHp += f;
                curTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

    }
}