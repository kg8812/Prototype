using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public class DotDmgUpdate : IBuffUpdate
    {
        List<SubBuff> buffs;
        readonly Actor actor;
        float time;

        public DotDmgUpdate(List<SubBuff> buffList, Actor actor)
        {
            buffs = buffList;
            this.actor = actor;
        }

        public void Notify(List<SubBuff> value)
        {
            buffs = value;
        }

        public void Update()
        {
            time += Time.deltaTime;
            
            if (time > 1)
            {
                float amount = 0;
                for (int i = 0; i < buffs.Count; i++)
                {
                    if (buffs[i] is not Debuff_DotDmg dot) continue;
                    
                    amount += dot.Dmg;
                }
                
                actor.CurHp -= amount;
                time = 0;
            }
        }        
    }

    public class BuffNoUpdate : IBuffUpdate
    {
        public void Notify(List<SubBuff> value)
        {
        }
     

        public void Update()
        {
        }
    }
}