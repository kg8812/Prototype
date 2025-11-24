using System.Collections;
using Apis;
using UnityEngine;

public interface ICdActive
{
    public void SetCD();
    public void StartCd();
    public float CurCd { get; set; }
    public bool CheckActive();
    public void SetIconCdType(UI_AtkItemIcon icon); // 아이콘 설정
    public void Update(EventParameters parameters);
    public void Init();
}

public class NormalCd : ICdActive
{
    private readonly Skill skill;

    public NormalCd(Skill skill)
    {
        this.skill = skill;
        isCd = false;
    }

    private bool isCd;

    public void SetCD()
    {
        CurCd = skill.Cd;
    }
    public void StartCd()
    {
        GameManager.instance.StartCoroutineWrapper(CooldownCoroutine());
    }

    private float _curCd;

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

    IEnumerator CooldownCoroutine()
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

    public void Init()
    {
    }
}

public class StackCd : ICdActive
{
    private Skill skill;

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

    private bool isStackCd;

    private float _curCd;

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


    IEnumerator MinCooldownCoroutine()
    {
        if (isStackCd) yield break;

        isStackCd = true;
        float temp = CurCd;
        CurCd = skill.minStackCd;
        while (CurCd > 0)
        {
            CurCd -= Time.deltaTime;
            yield return null;
        }
        
        CurCd = temp;
        isStackCd = false;
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
}

public class GaugeCd : ICdActive
{
    private Skill skill;

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

    private float _curCd;

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