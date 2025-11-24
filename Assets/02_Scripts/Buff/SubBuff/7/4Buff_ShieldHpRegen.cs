using System.Collections;
using UnityEngine;

namespace Apis
{
    public class Buff_ShieldHpRegen : SubBuff
    {
        public Buff_ShieldHpRegen(Buff buff) : base(buff)
        {
        }
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

            float value = actor.Barrier * amount[0] / 100;
            if (time == 0)
            {
                actor.CurHp += value;
                yield break;
            }
            float f = value * Time.deltaTime / time;

            while (curTime < time)
            {
                actor.CurHp += f;
                curTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        public override SubBuffType Type => SubBuffType.Buff_ShieldHpRegen;
    }
}