using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using UnityEngine;

namespace Apis
{
    public class DotDmgExtension : ProjectileExtension
    {
        public enum DotDmgTypes
        {
            Burn,Poison,Bleed
        }

        public DotDmgTypes Type;
        
        private void Awake()
        {
            projectile.AddEvent(EventType.OnAttackSuccess,ApplyDotDmg);
        }

        void ApplyDotDmg(EventParameters parameters)
        {
            if (parameters?.target is IBuffUser target)
            {
                SubBuffType type;
                switch (Type)
                {
                    case DotDmgTypes.Burn:
                        type = SubBuffType.Debuff_Burn;
                        break;
                    case DotDmgTypes.Bleed:
                        type = SubBuffType.Debuff_Bleed;
                        break;
                    case DotDmgTypes.Poison:
                        type = SubBuffType.Debuff_Poison;
                        break;
                    default:
                        return;
                }
                target.SubBuffManager.AddSubBuff(type,projectile._eventUser as Actor);
            }
        }
    }
}