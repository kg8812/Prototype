using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_SeguMechaGauge : UI_Ingame
{
    private RectTransform rect;

    private SeguMecha mecha;

    private Image image;

    public override void Init()
    {
        base.Init();
        image = GetComponentInChildren<Image>();
        rect = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void Init(SeguMecha _mecha)
    {
        mecha = _mecha;
        targetTrans = mecha.gaugePos;
    }
    protected override void PositioningFollower()
    {
        base.PositioningFollower();
        rect.anchoredPosition = calcPos;
    }

    protected override void Update()
    {
        base.Update();
        image.fillAmount = Mathf.Clamp01(mecha.curChargeTime / mecha.maxChargeTime);
    }
}
