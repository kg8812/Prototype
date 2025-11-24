using System.Collections;
using UnityEngine;

namespace Apis
{
    public class Buff_MaxHpRegen : SubBuff
    {
        public Buff_MaxHpRegen(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_MaxHpRegen;


        Coroutine coroutine;
        public override void OnAdd()
        {
            base.OnAdd();
            coroutine = GameManager.instance.StartCoroutineWrapper(HpRegen());
        }
        public override void OnRemove()
        {
            base.OnRemove();

            GameManager.instance.StopCoroutineWrapper(coroutine);
        }
        public override void PermanentApply()
        {
            base.PermanentApply();
            GameManager.instance.StartCoroutineWrapper(HpRegen());

        }
        IEnumerator HpRegen()
        {
            float time = duration;
            float curTime = 0;

            float healAmount = actor.MaxHp / 100 * amount[0];

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
        }
    }
}