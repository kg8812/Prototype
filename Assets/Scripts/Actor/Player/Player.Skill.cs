using System;
using System.Collections.Generic;
using Apis;
using Apis.UI;

public partial class Player : IActiveSkillUser, IPassiveSkillUser
{
    private List<SkillAttachment> _activeAttachments;

    private ActiveSkill _baseActiveSkill;
    private PassiveSkill _basePassiveSkill;


    private Action<ActiveSkill> _onActiveSkillChange;
    private Action<PassiveSkill> _onPassiveSkillChange;
    private Grab grab;

    public ActiveSkill curSkill { get; set; }

    public ActiveSkill ActiveSkill { get; private set; }

    public List<SkillAttachment> ActiveAttachments => _activeAttachments ??= new List<SkillAttachment>();

    public PassiveSkill PassiveSkill { get; private set; }

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

    public void ChangeActiveSkill(ActiveSkill active)
    {
        if (active == ActiveSkill) return;

        ActiveSkill?.UnEquip();
        ActiveSkill = active;
        ActiveSkill?.Equip(this);
        SetMainSkillIcon();
        _onActiveSkillChange?.Invoke(active);
    }

    public void ResetActiveSkill()
    {
        if (ActiveSkill == _baseActiveSkill) return;

        ActiveSkill?.UnEquip();
        ActiveSkill = _baseActiveSkill;
        ActiveSkill?.Equip(this);
        SetMainSkillIcon();
        _onActiveSkillChange?.Invoke(ActiveSkill);
    }

    public void ChangePassiveSkill(PassiveSkill passive)
    {
        if (PassiveSkill == passive) return;

        PassiveSkill?.UnEquip();
        PassiveSkill = passive;
        PassiveSkill?.Equip(this);
        _onPassiveSkillChange?.Invoke(passive);
    }

    public void ResetPassiveSkill()
    {
        if (PassiveSkill == _basePassiveSkill) return;

        PassiveSkill?.UnEquip();
        PassiveSkill = _basePassiveSkill;
        PassiveSkill?.Equip(this);
        _onPassiveSkillChange?.Invoke(PassiveSkill);
    }

    private void SetMainSkillIcon()
    {
        //var icon = UI_MainHud.Instance.mainSkillIcon;
        // if (ActiveSkill == null)
        //     icon.WhenItemIsNull();
        // else
        //{
            //     icon.WhenItemIsSet();
            // icon.SetIcon(ActiveSkill.SkillImage);
        //}
    }
}