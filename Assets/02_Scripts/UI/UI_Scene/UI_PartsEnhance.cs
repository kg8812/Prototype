using System;
using System.Collections.Generic;
using System.Linq;
using Apis.UI;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class UI_PartsEnhance : UI_Scene
{
    // enum Buttons
    // {
    //     Head,
    //     Body,
    //     Arm,
    //     Leg,
    //     Core,
    //     CloseButton,
    // }
    //
    // private static Dictionary<EnhanceParts, int> counts = new();
    //
    // public static Dictionary<EnhanceParts, int> Counts
    // {
    //     get => counts;
    //     set
    //     {
    //         if (value != null)
    //         {
    //             counts = new(value);
    //         }
    //         else
    //         {
    //             counts = new();
    //             foreach (EnhanceParts eType in Enum.GetValues(typeof(EnhanceParts)))
    //             {
    //                 counts.Add(eType, 0);
    //             }
    //         }
    //     }
    // }
    //
    // private Dictionary<EnhanceParts, List<Image>> imgs;
    //
    // private int EnhanceTypeIncrease(EnhanceParts eType)
    // {
    //     return eType switch
    //     {
    //         EnhanceParts.Coat => 10,
    //         EnhanceParts.Frame => 10,
    //         EnhanceParts.Core => 10,
    //         EnhanceParts.Lens => 10,
    //         EnhanceParts.Circut => 10,
    //         _ => 0
    //     };
    // }
    //
    // private ActorStatType EnhanceStatType(EnhanceParts eType)
    // {
    //     return eType switch
    //     {
    //         EnhanceParts.Coat => ActorStatType.Def,
    //         EnhanceParts.Frame => ActorStatType.MaxHp,
    //         EnhanceParts.Core => ActorStatType.Atk,
    //         EnhanceParts.Lens => ActorStatType.CritDmg,
    //         EnhanceParts.Circut => ActorStatType.Mental,
    //         _ => 0
    //     };
    // }
    //
    // public override void Init()
    // {
    //     base.Init();
    //     Bind<Button>(typeof(Buttons));
    //
    //     imgs = new();
    //     foreach (EnhanceParts eType in Enum.GetValues(typeof(EnhanceParts)))
    //     {
    //         // 무조건 EnhanceParts와 Buttons의 enum index가 같아야 함.
    //         Button newBtn = Get<Button>((int)eType);
    //         newBtn.onClick.AddListener(() => AddStat(EnhanceStatType(eType), EnhanceTypeIncrease(eType), eType));
    //         
    //         
    //         imgs.Add(eType, newBtn.GetComponentsInChildren<Image>().ToList());
    //     }
    //     SetImgsColor();
    //     
    //     
    //     Get<Button>((int)Buttons.CloseButton).onClick.AddListener(() => GameManager.UI.CloseUI(this));
    // }
    //
    // void SetImgsColor()
    // {
    //     foreach (EnhanceParts eType in Enum.GetValues(typeof(EnhanceParts)))
    //     {
    //         int eCount = counts[eType];
    //         for (int i = 0; i < imgs[eType].Count; i++)
    //         {
    //             imgs[eType][i].color =  i< eCount ? Color.red : Color.white;
    //         }
    //     }
    // }
    //
    // void AddStat(ActorStatType statType, float amount, EnhanceParts enhanceType)
    // {
    //     int count = counts[enhanceType] + 1;
    //     if (count >= imgs[enhanceType].Count) return;
    //     counts[enhanceType] = count;
    //     SetImgsColor();
    //     GameManager.instance.Player.AddStat(statType, amount, ValueType.Value);
    // }
}


public class UI_PartsEnhance2 : UI_Scene
{
    // [ShowInInspector] [SerializeField] private Dictionary<EnhanceParts, UI_PartsEnhanceItem> items;
    //
    // enum Texts
    // {
    //     EnhanceType,
    //     PointText,
    //     
    // }
    //
    // // 아직은 안쓰지만 나중에 쓸 가능성 있어서 남겨둠.
    // //private int curPoint;
    //
    //
    // protected override void Activated()
    // {
    //     base.Activated();
    //     //curPoint = 2;
    // }
    //
    //
}

public class UI_PartsEnhanceItem : MonoBehaviour
{
    // private readonly int _loopString = Shader.PropertyToID("_UnScaledTime");
    // [SerializeField] private UIAsset_Button enhanceBtn;
    // [SerializeField] private Image[] diamonds;
    // [SerializeField] private int[] costs;
    //
    // public int DiamondCnt { get; private set; }
    //
    // private bool _doHover;
    // private Material _hoverEffectMat;
    //
    // private void Awake()
    // {
    //     if (costs.Length != diamonds.Length)
    //     {
    //         Debug.LogError("파츠 강화 비용 개수가 달라짐.");
    //     }
    // }
    //
    // private void Start()
    // {
    //     enhanceBtn.stateChanged.AddListener(DiamondHoverEffect);
    // }
    //
    // private void Update()
    // {
    //     if (_doHover)
    //     {
    //         _hoverEffectMat.SetFloat(_loopString, Time.unscaledTime);
    //     }
    // }
    //
    // public void DiamondSet(int cnt)
    // {
    //     if (cnt > diamonds.Length)
    //     {
    //         Debug.LogError("가지고 있는 다이아몬드 개수보다 더많이 세팅함.");
    //         return;
    //     }
    //
    //     DiamondCnt = cnt;
    // }
    //
    //
    // private void CheckCanEnhance(int goldCnt)
    // {
    //     if (DiamondCnt < diamonds.Length || costs[DiamondCnt - 1] <= goldCnt)
    //     {
    //         
    //     }
    // }
    //
    //
    // private void DiamondHoverEffect(UIElementState el)
    // {
    //     if (el == UIElementState.Hover)
    //     {
    //         if (DiamondCnt < diamonds.Length)
    //         {
    //             // 현재 모은 파츠 강화보다 하나더 앞선 이미지에 hover effect
    //             _doHover = true;
    //             _hoverEffectMat = diamonds[DiamondCnt].material;
    //         }
    //     }
    //     else
    //     {
    //         _doHover = false;
    //         _hoverEffectMat = null;
    //     }
    // }
}