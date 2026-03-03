using System.Collections.Generic;
using Default;
using Apis.UI;
using UnityEngine;

public partial class Player
{
    private static Dictionary<PlayerType, UnitData> _unitDatas;

    static Dictionary<PlayerType, UnitData> UnitDatas => _unitDatas ??= _unitDatas = new()
    {
        { PlayerType.Player1, ResourceUtil.Load<UnitData>(Define.PlayerData.Player1) },
    };
    // isClean
    // true:    데이터 로드하기 위해서는 순정 상태의 플레이어가 필요.
    // false:   게임 시작할때는 각종 효과나 기초 무기나 그런거 쥐어줘야 함.
    public static Player CreatePlayerByType(PlayerType character)
    {
        bool isCreated = false;
        Player player = GameManager.instance.Player;
        if (ReferenceEquals(null, player))
        {
            player = ResourceUtil.Instantiate("Player").GetComponent<Player>();
            isCreated = true;
        }

        player._playerType = character;
        //player.animator.SetInteger("PlayerType", (int)character);
        GameManager.instance.Player = player;
        player.SetUnitData(UnitDatas[character]);
        GameManager.instance.onPlayerChange.Invoke(player);
        player.ResetPlayerStatus();
        if (isCreated)
        {
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
