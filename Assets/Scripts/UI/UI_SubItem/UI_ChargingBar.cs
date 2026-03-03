using Apis;
using Default;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChargingBar : UI_Base
{
    enum Images
    {
        Gauge
    }

    protected Image gauge;
    protected ActiveSkill active;
    
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        gauge = Get<Image>((int)Images.Gauge);
        gauge.fillAmount = 0;
    }

    private void Update()
    {
        if (ReferenceEquals(active, null)) return;
        
        gauge.fillAmount = Mathf.Clamp01(active.CurChargeTime / active.ChargeTime);
    }
}
