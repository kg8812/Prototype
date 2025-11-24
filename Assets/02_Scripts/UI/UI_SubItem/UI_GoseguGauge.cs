using Apis;
using Default;
using UI;
using UnityEngine;
using UnityEngine.UI;


public class UI_GoseguGauge : UI_Base
{
    enum Images
    {
        Gauge
    }

    private GoseguActiveSkill _active;
    
    private Image _gaugeImage;
    
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        _gaugeImage = Get<Image>((int)Images.Gauge);
        
        UI_MainHud.Instance.setEvent.AddListener(x =>
        {
            x.OnActiveSkillChange += CheckSkill;
            CheckSkill(x.ActiveSkill);
        });
        
        UI_MainHud.Instance.afterSet.AddListener(x =>
        {
            SetSkill(x.ActiveSkill);
            x.OnActiveSkillChange += SetSkill;
        });
    }

    void CheckSkill(ActiveSkill skill)
    {
        _active = skill as GoseguActiveSkill;
        gameObject.SetActive(_active != null);
    }

    void SetSkill(ActiveSkill skill)
    {
        if (skill is GoseguActiveSkill active)
        {
            active.OnGaugeChange.RemoveListener(SetGauge);
            active.OnGaugeChange.AddListener(SetGauge);
            _gaugeImage.fillAmount =  Mathf.Clamp01(active.Gauge / active.MaxGauge);
        }
    }

    void SetGauge(float gauge)
    {
        _gaugeImage.fillAmount =  Mathf.Clamp01(gauge / _active.MaxGauge);
    }
}
