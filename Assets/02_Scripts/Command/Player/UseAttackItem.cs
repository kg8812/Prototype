using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Command
{
    [CreateAssetMenu(fileName = "UseAttackItem", menuName = "ActorCommand/Player/UseAttackItem", order = 1)]
    public class UseAttackItem : PlayerCommand
    {
        [FormerlySerializedAs("Count")] [InfoBox("사용할 무장 인덱스 (0부터 시작)")]
        public int index;

        protected override void Invoke(Player go)
        {
            var item = AttackItemManager.CurrentItem;
            if (item != null && item.AtkSlotIndex != index)
            {
                go.OnAttackItemChange();
            }

            AttackItemManager.Attack(index);

        }

        public override bool InvokeCondition(Player player)
        {
            // Debug.Log(player.AbleAttack
            //           +" " + !player.OnFinalAttack
            //           +" " + player.attackStrategy.CheckAttackable(index)
            //           +" " + player.CoolDown.GetCd(EPlayerCd.DashToAttack)
            //           +" " + player.CoolDown.GetCd(EPlayerCd.JumpToAttack)
            //           +" " + player.CoolDown.GetCd(EPlayerCd.AirAttack));
            
            return player.AbleAttack
                   && player.attackStrategy.CheckAttackable(index)
                   && player.CoolDown.GetCd(EPlayerCd.DashToAttack)
                   && player.CoolDown.GetCd(EPlayerCd.JumpToAttack)
                   && player.CoolDown.GetAirAttackCd(index);
                //    && !player.OnFinalAttack
                //    && player.CoolDown.GetCd(EPlayerCd.AirAttack);
                   
        }
    }
}