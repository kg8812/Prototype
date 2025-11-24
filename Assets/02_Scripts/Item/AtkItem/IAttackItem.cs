using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis;
using UnityEngine;

public enum AttackCategory
{
    One,Two,Three,Four,Five
}

public interface IAttackItem
{
    // public string Name { get; }
    public void BeforeAttack();
    public void UseAttack();    
    public bool TryAttack();
    public AttackCategory Category { get; }
    public void Equip(IMonoBehaviour user);
    public void UnEquip();
    void WhenIconIsSet(UI_AtkItemIcon icon);
    public void EndAttack();
    
    public UI_AtkItemIcon Icon { get; set; }
    public int AtkSlotIndex { get; set; }
    
    public void SetIcon(UI_AtkItemIcon icon)
    {
        Icon = icon;
        WhenIconIsSet(icon);
    }
}

public interface IAttackItemStat
{
    public float Atk { get; }
    public float BaseGroggyPower { get; }
    public float BodyFactor {get;}
    public float SpiritFactor {get;}
    public float FinesseFactor {get;}

}