using System;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis;
using Default;
using NewNewInvenSpace;
using Save.Schema;
using UI;
using UnityEngine;

public partial class Player
{
    private static Dictionary<PlayerType, UnitData> _unitDatas;

    static Dictionary<PlayerType, UnitData> UnitDatas => _unitDatas ??= _unitDatas = new()
    {
        { PlayerType.Ine, ResourceUtil.Load<UnitData>(Define.PlayerData.IneData) },
        { PlayerType.Jingburger, ResourceUtil.Load<UnitData>(Define.PlayerData.JingburgerData) },
        { PlayerType.Lilpa, ResourceUtil.Load<UnitData>(Define.PlayerData.LilpaData) },
        { PlayerType.Jururu, ResourceUtil.Load<UnitData>(Define.PlayerData.JururuData) },
        { PlayerType.Gosegu, ResourceUtil.Load<UnitData>(Define.PlayerData.GoseguData) },
        { PlayerType.Viichan, ResourceUtil.Load<UnitData>(Define.PlayerData.ViichanData) },
    };
    // isClean
    // true:    데이터 로드하기 위해서는 순정 상태의 플레이어가 필요.
    // false:   게임 시작할때는 각종 효과나 기초 무기나 그런거 쥐어줘야 함.
    public static Player CreatePlayerByType(PlayerType character, bool isClean = false)
    {
        bool isCreated = false;
        Player player = GameManager.instance.Player;
        if (ReferenceEquals(null, player))
        {
            player = ResourceUtil.Instantiate("Player").GetComponent<Player>();
            isCreated = true;
            if (!isClean)
            {
                //Weapon wp = GameManager.Item.GetWeapon(StartingWeapon(character));
                //InvenManager.instance.AttackItem.MoveInvenType(0, InvenType.Storage, InvenType.Equipment);
            }
        }

        player.playerType = character;
        player.animator.SetInteger("PlayerType", (int)character);
        GameManager.instance.Player = player;
        player.SetUnitData(UnitDatas[character]);
        GameManager.instance.onPlayerChange.Invoke(player);
        UI_MainHud.Instance.ChangePortrait();
        player.ResetPlayerStatus();
        if (isCreated)
        {
            player.gameObject.SetActive(false);
            GameManager.instance.OnPlayerCreated.Invoke(player);
        }

        return player;
    }

    #region 공격 이벤트 관련
    private List<AttackEvent> _AttackEvents = null;
    public List<AttackEvent> AttackEvents {
        get {
            if(_AttackEvents != null) return _AttackEvents;

            var container = ResourceUtil.Load<AttackEventContainer>("AttackEvents");

            if(container == null) { 
                Debug.LogError("Invalid Attack Events");
                return null;
            }

            _AttackEvents = container.AttackEvents;

            return _AttackEvents;
        }
    }
    #endregion
}