using System.Collections.Generic;
using Apis;
using Apis.Managers;
using Save.Schema;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusMenu : SerializedMonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI soulText;
    public TextMeshProUGUI lobbySoulText;
    public TextMeshProUGUI activeNameText;
    public TextMeshProUGUI passiveNameText;
    public TextMeshProUGUI activeDescText;
    public TextMeshProUGUI passiveDescText;

    public Dictionary<PlayerType, Sprite> icons = new();
    public Dictionary<PlayerType, Sprite> portraits = new();
    public Image portrait;
    public Image characterIcon;
    private void OnEnable()
    {
        Player player = GameManager.instance.Player;

        if (player == null) return;

        int nameId = player.playerType switch
        {
            PlayerType.Ine => 1010011,
            PlayerType.Jingburger => 1010012,
            PlayerType.Lilpa => 1010013,
            PlayerType.Jururu => 1010014,
            PlayerType.Gosegu => 1010015,
            PlayerType.Viichan => 1010016,
            _ => 1010815
        };

        nameText.text = LanguageManager.Str(nameId);
        portrait.sprite = portraits[player.playerType];
        characterIcon.sprite = icons[player.playerType];
        soulText.text = GameManager.instance.Soul.ToString();
        lobbySoulText.text = GameManager.instance.LobbySoul.ToString();

        string noneText = LanguageManager.Str(1010041);
        activeNameText.text = ((player.ActiveSkill?.SkillName ?? 0) == 0) ? noneText : StrUtil.GetEquipmentName(player.ActiveSkill.SkillName);
        activeDescText.text = ((player.ActiveSkill?.Desc ?? 0) == 0) ? noneText : StrUtil.GetEquipmentName(player.ActiveSkill.SkillName);
        passiveNameText.text = ((player.PassiveSkill?.SkillName ?? 0) == 0) ? noneText : StrUtil.GetEquipmentName(player.PassiveSkill.SkillName);
        passiveDescText.text = ((player.PassiveSkill?.Desc ?? 0) == 0) ? noneText : StrUtil.GetEquipmentName(player.PassiveSkill.SkillName);
    }
}
