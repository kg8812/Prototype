using System.Collections;
using Apis;
using UnityEngine;

public interface ICdActive
{
    public float CurCd { get; set; }
    public void SetCD();
    public void StartCd();
    public bool CheckActive();
    public void SetIconCdType(UI_AtkItemIcon icon); // 아이콘 설정
    public void Update(EventParameters parameters);
    public void Init();
}

public class NormalCd : ICdActive
{
    private readonly Skill skill;

    private float _curCd;

    private bool isCd;

    public NormalCd(Skill skill)
    {
        this.skill = skill;
        isCd = false;
    }

    public void SetCD()
    {
        CurCd = skill.Cd;
    }

    public void StartCd()
    {
        GameManager.instance.StartCoroutineWrapper(CooldownCoroutine());
    }

    public float CurCd
    {
        get => _curCd;
        set
        {
            _curCd = value;
            if (_curCd < 0) _curCd = 0;
        }
    }

    public bool CheckActive()
    {
        return skill.CurCd <= 0 && skill.CurDuration <= 0;
    }

    public void SetIconCdType(UI_AtkItemIcon icon)
    {
        if (icon == null) return;

        icon.ChangeType(new UI_AtkItemIcon.NormalCdUpdate(icon));
    }

    public void Update(EventParameters parameters)
    {
    }

    public void Init()
    {
    }

    private IEnumerator CooldownCoroutine()
    {
        if (!isCd)
        {
            isCd = true;
            while (CurCd > 0)
            {
                CurCd -= Time.deltaTime * (skill.cdRatio / 100);
                yield return new WaitForEndOfFrame();
            }

            isCd = false;
        }
    }
}

public class StackCd : ICdActive
{
    private float _curCd;

    private bool isStackCd;
    private readonly Skill skill;

    public StackCd(Skill skill)
    {
        this.skill = skill;
        isStackCd = false;
        CurCd = skill.Cd;
    }

    public void SetCD()
    {
    }

    public void StartCd()
    {
        if (skill.CurStack > 0)
        {
            skill.CurStack--;
            GameManager.instance.StartCoroutine(MinCooldownCoroutine());
        }
    }

    public float CurCd
    {
        get => _curCd;
        set
        {
            _curCd = value;
            if (_curCd < 0) _curCd = 0;
        }
    }

    public bool CheckActive()
    {
        return skill.CurStack > 0 && !isStackCd;
    }

    public void SetIconCdType(UI_AtkItemIcon icon)
    {
        if (icon == null) return;

        icon.ChangeType(new UI_AtkItemIcon.StackUpdate(icon));
    }

    public void Update(EventParameters parameters)
    {
        if (isStackCd) return;

        if (skill.CurStack >= skill.MaxStack)
        {
            CurCd = skill.Cd;
            return;
        }

        if (CurCd > 0)
        {
            CurCd -= Time.deltaTime * (skill.cdRatio / 100);
        }
        else
        {
            CurCd = skill.Cd;
            skill.CurStack += skill.StackGain;
        }
    }

    public void Init()
    {
        CurCd = skill.Cd;
    }


    private IEnumerator MinCooldownCoroutine()
    {
        if (isStackCd) yield break;

        isStackCd = true;
        var temp = CurCd;
        CurCd = skill.minStackCd;
        while (CurCd > 0)
        {
            CurCd -= Time.deltaTime;
            yield return null;
        }

        CurCd = temp;
        isStackCd = false;
    }
}

public class GaugeCd : ICdActive
{
    private float _curCd;
    private readonly Skill skill;

    public GaugeCd(Skill skill)
    {
        this.skill = skill;
    }

    public void SetCD()
    {
        CurCd = skill.Cd;
    }

    public void StartCd()
    {
        CurCd = skill.Cd;
    }

    public float CurCd
    {
        get => _curCd;
        set
        {
            _curCd = value;
            if (_curCd < 0) _curCd = 0;
        }
    }

    public bool CheckActive()
    {
        return skill.CurCd <= 0 && skill.CurDuration <= 0;
    }

    public void SetIconCdType(UI_AtkItemIcon icon)
    {
        if (icon == null) return;
        icon.ChangeType(new UI_AtkItemIcon.NormalCdUpdate(icon));
    }

    public void Update(EventParameters parameters)
    {
    }

    public void Init()
    {
    }
}