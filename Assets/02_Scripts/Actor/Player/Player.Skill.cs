using System;
using System.Collections.Generic;
using Apis;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

public partial class Player : IActiveSkillUser , IPassiveSkillUser
{
    private Grab grab;
    ActiveSkill activeSkill;
    PassiveSkill passiveSkill;

    public ActiveSkill curSkill { get; set; }
    
    public ActiveSkill ActiveSkill => activeSkill;

    private List<SkillAttachment> _activeAttachments;
    public List<SkillAttachment> ActiveAttachments => _activeAttachments ??= new();

    public PassiveSkill PassiveSkill => passiveSkill;

    
    Action<ActiveSkill> _onActiveSkillChange;
    Action<PassiveSkill> _onPassiveSkillChange;
    
    public event Action<ActiveSkill> OnActiveSkillChange
    {
        add
        {
            _onActiveSkillChange -= value;
            _onActiveSkillChange += value;
        }
        remove => _onActiveSkillChange -= value;
    }

    public event Action<PassiveSkill> OnPassiveSkillChange
    {
        add
        {
            _onPassiveSkillChange -= value;
            _onPassiveSkillChange += value;
        }
        remove => _onPassiveSkillChange -= value;
    }

    private ActiveSkill _baseActiveSkill;
    private PassiveSkill _basePassiveSkill;
    
    public void ChangeActiveSkill(ActiveSkill active)
    {
        if (active == activeSkill) return;
        
        activeSkill?.UnEquip();
        activeSkill = active;
        activeSkill?.Equip(this);
        SetMainSkillIcon();
        _onActiveSkillChange?.Invoke(active);
    }

    public void ResetActiveSkill()
    {
        if (activeSkill == _baseActiveSkill) return;
        
        activeSkill?.UnEquip();
        activeSkill = _baseActiveSkill;
        activeSkill?.Equip(this);
        SetMainSkillIcon();
        _onActiveSkillChange?.Invoke(activeSkill);
    }

    public void ChangePassiveSkill(PassiveSkill passive)
    {
        if (passiveSkill == passive) return;
        
        passiveSkill?.UnEquip();
        passiveSkill = passive;
        passiveSkill?.Equip(this);
        _onPassiveSkillChange?.Invoke(passive);
    }

    public void ResetPassiveSkill()
    {
        if (passiveSkill == _basePassiveSkill) return;
        
        passiveSkill?.UnEquip();
        passiveSkill = _basePassiveSkill;
        passiveSkill?.Equip(this);
        _onPassiveSkillChange?.Invoke(passiveSkill);
    }
    void SetMainSkillIcon()
    {
        var icon = UI_MainHud.Instance.mainSkillIcon;
        if (activeSkill == null)
        {
            icon.WhenItemIsNull();
        }
        else
        {
            icon.WhenItemIsSet();
        }
    }
}
