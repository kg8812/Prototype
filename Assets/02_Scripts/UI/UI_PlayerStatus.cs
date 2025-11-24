using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UI_PlayerStatus : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI spiritText;
    [SerializeField] private TextMeshProUGUI finesseText;
    
    [SerializeField] TextMeshProUGUI hp;
    [SerializeField] TextMeshProUGUI atk;
    [SerializeField] TextMeshProUGUI def;
    [SerializeField] TextMeshProUGUI speed;
    [SerializeField] TextMeshProUGUI critProb;
    [SerializeField] TextMeshProUGUI critDmg;
    [SerializeField] TextMeshProUGUI mental;
    [SerializeField] TextMeshProUGUI cdReduction;

    [SerializeField] private TextMeshProUGUI hp2;
    [SerializeField] TextMeshProUGUI atk2;
    [SerializeField] TextMeshProUGUI def2;
    [SerializeField] TextMeshProUGUI speed2;
    [SerializeField] TextMeshProUGUI critProb2;
    [SerializeField] TextMeshProUGUI critDmg2;
    [SerializeField] TextMeshProUGUI mental2;
    [SerializeField] TextMeshProUGUI cdReduction2;
    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    [Button]
    public void MoveToLeft(float amount)
    {
        hp.rectTransform.anchorMin = new Vector2(hp.rectTransform.anchorMin.x - amount, hp.rectTransform.anchorMin.y);
        atk.rectTransform.anchorMin = new Vector2(atk.rectTransform.anchorMin.x - amount, atk.rectTransform.anchorMin.y);
        def.rectTransform.anchorMin = new Vector2(def.rectTransform.anchorMin.x - amount, def.rectTransform.anchorMin.y);
        speed.rectTransform.anchorMin = new Vector2(speed.rectTransform.anchorMin.x - amount, speed.rectTransform.anchorMin.y);
        critProb.rectTransform.anchorMin = new Vector2(critProb.rectTransform.anchorMin.x - amount, critProb.rectTransform.anchorMin.y);
        critDmg.rectTransform.anchorMin = new Vector2(critDmg.rectTransform.anchorMin.x - amount, critDmg.rectTransform.anchorMin.y);
        mental.rectTransform.anchorMin = new Vector2(mental.rectTransform.anchorMin.x - amount, mental.rectTransform.anchorMin.y);
        cdReduction.rectTransform.anchorMin = new Vector2(cdReduction.rectTransform.anchorMin.x - amount, cdReduction.rectTransform.anchorMin.y);
        atk2.rectTransform.anchorMin = new Vector2(atk2.rectTransform.anchorMin.x - amount, atk2.rectTransform.anchorMin.y);
        def2.rectTransform.anchorMin = new Vector2(def2.rectTransform.anchorMin.x - amount, def2.rectTransform.anchorMin.y);
        speed2.rectTransform.anchorMin = new Vector2(speed2.rectTransform.anchorMin.x - amount, speed2.rectTransform.anchorMin.y);
        critProb2.rectTransform.anchorMin = new Vector2(critProb2.rectTransform.anchorMin.x - amount, critProb2.rectTransform.anchorMin.y);
        critDmg2.rectTransform.anchorMin = new Vector2(critDmg2.rectTransform.anchorMin.x - amount, critDmg2.rectTransform.anchorMin.y);
        mental2.rectTransform.anchorMin = new Vector2(mental2.rectTransform.anchorMin.x - amount, mental2.rectTransform.anchorMin.y);
        cdReduction2.rectTransform.anchorMin = new Vector2(cdReduction2.rectTransform.anchorMin.x - amount, cdReduction2.rectTransform.anchorMin.y);
    }
    private void OnEnable()
    {
        bodyText.text = $"{player.Body}";
        spiritText.text = $"{player.Spirit}";
        finesseText.text = $"{player.Finesse}";
        hp.text = $"{player.StatManager.BaseStat.Get(ActorStatType.MaxHp)}";
        hp2.text = $"(+{Mathf.RoundToInt(player.MaxHp - player.StatManager.BaseStat.Get(ActorStatType.MaxHp))})";
        atk.text = $"{player.StatManager.BaseStat.Get(ActorStatType.Atk)}";
        atk2.text = $"(+{Mathf.RoundToInt(player.Atk - player.StatManager.BaseStat.Get(ActorStatType.Atk))})";
        def.text = $"{player.StatManager.BaseStat.Get(ActorStatType.Def)}";
        def2.text = $"(+{Mathf.RoundToInt(player.Def - player.StatManager.BaseStat.Get(ActorStatType.Def))})";
        speed.text = $"{player.StatManager.BaseStat.Get(ActorStatType.MoveSpeed)}";
        speed2.text = $"(+{Mathf.RoundToInt(player.MoveSpeed - player.StatManager.BaseStat.Get(ActorStatType.MoveSpeed))})";
        critProb.text = $"{player.StatManager.BaseStat.Get(ActorStatType.CritProb)}";
        critProb2.text = $"(+{Mathf.RoundToInt(player.CritProb - player.StatManager.BaseStat.Get(ActorStatType.CritProb))})";
        critDmg.text = $"{player.StatManager.BaseStat.Get(ActorStatType.CritDmg)}";
        critDmg2.text = $"(+{Mathf.RoundToInt(player.CritDmg - player.StatManager.BaseStat.Get(ActorStatType.CritDmg))})";
        mental.text = $"{player.StatManager.BaseStat.Get(ActorStatType.Mental)}";
        mental2.text = $"(+{Mathf.RoundToInt(player.Mentality - player.StatManager.BaseStat.Get(ActorStatType.Mental))})";
        cdReduction.text = $"{player.StatManager.BaseStat.Get(ActorStatType.CDReduction)}";
        cdReduction2.text = $"(+{Mathf.RoundToInt(player.CDReduction - player.StatManager.BaseStat.Get(ActorStatType.CDReduction))})";
    }
}